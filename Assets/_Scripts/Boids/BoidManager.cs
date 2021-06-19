using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using Random = Unity.Mathematics.Random;

public class BoidManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private string boidLayerName = "Boids";
    [SerializeField] private string obstableLayerName = "Obstables";
    [SerializeField] private string sPPCellLayerName = "Cells";
    [SerializeField] private LayerMask boidLayerMask;
    [SerializeField] private LayerMask obstableLayerMask;
    [SerializeField] private LayerMask obstableAndCellsLayerMask;

    [SerializeField] private FlockSettings settings = new FlockSettings(new Vector3(), new Vector3());

    [SerializeField] private List<BoidAgent> agentList;

    // [SerializeField] private int numberOfBoids;
    private BoidAgent[] agents;
    private Transform[] agentTransforms;
    private BoidAgent.BoidStruct[] _boidStructs;

    [NativeDisableUnsafePtrRestriction] private TransformAccessArray _accessArray;
    [NativeDisableUnsafePtrRestriction] private NativeArray<BoidAgent.BoidStruct> _boidStructsNArray;


    private NativeArray<Quaternion> _rotationResults;


    private JobHandle handleMoveJob;
    private JobHandle handleRotationJob;

    private float obstableSearchRadius = 6;

    #endregion

    #region Flock Settings Struct Section

    [Serializable]
    [BurstCompile]
    public struct FlockSettings
    {
        public Vector3 flockCenter;
        public Vector3 flockTarget;

        public FlockSettings(Vector3 flockCenter, Vector3 flockTarget)
        {
            this.flockCenter = flockCenter;
            this.flockTarget = flockTarget;
        }
    }

    #endregion

    #region Jobs Definition Section

    [BurstCompile]
    public struct MoveBoidJob : IJobParallelForTransform
    {
        private float deltaTime;
        private float speed;
        private NativeArray<BoidAgent.BoidStruct> boids;
        private NativeArray<Quaternion> newRotations;


        public MoveBoidJob(NativeArray<BoidAgent.BoidStruct> boids, NativeArray<Quaternion> newRotations, float dT,
            float speed)
        {
            this.boids = boids;
            deltaTime = dT;
            this.speed = speed;
            this.newRotations = newRotations;
        }

        public void Execute(int index, TransformAccess transform)
        {

            
            float3 newPosition = boids[index].position +
                                  boids[index].direction * boids[index].currentSpeed * deltaTime;
            
            
            transform.rotation = newRotations[index];
            transform.position = newPosition;
            
            if (transform.position.y > 5)
            {
                float3 pos = transform.position;

                pos.y = -5;
                pos.x = math.sin(deltaTime * 20)*3;

                transform.position = pos;
            }
        }
    }

    [BurstCompile]
    public struct BoidRotationJob : IJob, IDisposable
    {
        [NativeDisableUnsafePtrRestriction] public NativeArray<BoidAgent.BoidStruct> boids;
        public FlockSettings settings;
        public float deltaTime;
        public NativeArray<Quaternion> rotationResults;

        public BoidRotationJob( NativeArray<Quaternion> rotationResults,
            NativeArray<BoidAgent.BoidStruct> boids,
            FlockSettings settings, float deltaTime)
        {
            // this.obstacles = obstacles;
            this.rotationResults = rotationResults;
            this.boids = boids;
            this.settings = settings;
            this.deltaTime = deltaTime;
        }

        public void Execute()
        {
            int numberOfNeighbors = 0;
            float3 neighCenter = float3.zero;

            float obstacleCount = 0;
            float3 avoidanceDirection = float3.zero;

            for (int i = 0; i < boids.Length; i++)
            {
                for (int bIndex = 0; bIndex < boids.Length; bIndex++)
                {
                    if (boids[i].position.Equals(boids[bIndex].position))
                    {
                        continue;
                    }

                    float distanceToBoid = math.distance(boids[i].position, boids[bIndex].position);
                    
                    if(distanceToBoid>boids[i].neighborhoodRadius){continue;}

                    /* check for avoidance and neighbor */
                    if (distanceToBoid < boids[i].avoidanceRadius)
                    {
                        float3 directionAwayFromObstable =
                           boids[i].position - boids[bIndex].position;
                        avoidanceDirection += directionAwayFromObstable;
                        obstacleCount++;
                        continue;
                    }

                    if (distanceToBoid > boids[i].avoidanceRadius)
                    {
                        neighCenter += boids[bIndex].position;
                        numberOfNeighbors++;
                    }
                }

                /* normalize neighborhood center values */
                neighCenter = neighCenter / numberOfNeighbors;
                // offset from agent postion
                neighCenter -= boids[i].position;

                avoidanceDirection = avoidanceDirection / obstacleCount;
                /* normalize avoidance values */
                // avoidanceDirection = Vector3.Normalize(avoidanceDirection / obstacleCount);
                avoidanceDirection -= boids[i].position;

                float3 combinedDirection = (neighCenter + avoidanceDirection) / 2 + (float3)Vector3.Normalize((float3)settings.flockCenter-boids[i].position);

                Quaternion newBoidDirection = math.nlerp(boids[i].rotation,
                    quaternion.Euler(combinedDirection), deltaTime * boids[i].turnRate);
             
                rotationResults[i] = newBoidDirection;
            }
        }

        public void Dispose()
        {
        }
    }

    private JobHandle CreateMoveBoidJob(Transform[] agentTransforms)
    {

     //   _boidStructsNArray = new NativeArray<BoidAgent.BoidStruct>(_boidStructs, Allocator.TempJob);
        _accessArray = new TransformAccessArray(agentTransforms);

        return new MoveBoidJob(_boidStructsNArray, _rotationResults, Time.deltaTime, 2).Schedule(_accessArray);
    }

    private JobHandle CreateRotationJob()
    {
        _boidStructsNArray = new NativeArray<BoidAgent.BoidStruct>(_boidStructs, Allocator.TempJob);
        _rotationResults = new NativeArray<Quaternion>(_boidStructs.Length, Allocator.TempJob);

        return new BoidRotationJob(_rotationResults, _boidStructsNArray, settings, Time.deltaTime).Schedule();
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.Full);
        // Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.Full);
        // Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.Full);
        // Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.Full);

        boidLayerMask = LayerMask.GetMask(boidLayerName);
        obstableLayerMask = LayerMask.GetMask(obstableLayerName);
        obstableAndCellsLayerMask = LayerMask.GetMask(obstableLayerName, sPPCellLayerName);

        agentList = new List<BoidAgent>();

        agents = new BoidAgent[0];

    }


    private int countBetweenUpdates = 6;
    private int currentCountBetweenUpdates = 0;

    // FixedUpdate is called 60 times per second
    void Update()
    {
        currentCountBetweenUpdates++;
        if (currentCountBetweenUpdates > countBetweenUpdates)
        {
            // fixedUpdateNow = true;

            if (agents.Length > 0)
            {
                RefeshDataArray();
                handleRotationJob = CreateRotationJob();
            }
        }

    }

    private void LateUpdate()
    {
        if (currentCountBetweenUpdates > countBetweenUpdates)
        {
            if (agents.Length > 0)
            {
                handleRotationJob.Complete();

                // _boidStructsNArray.Dispose();

                handleMoveJob = CreateMoveBoidJob(agentTransforms);
                handleMoveJob.Complete();

                _accessArray.Dispose();
                _boidStructsNArray.Dispose();
                _rotationResults.Dispose();
            }

            currentCountBetweenUpdates = 0;
        }

    }

    #region Boids Tracking Section

    public void AddBoid(BoidAgent boid)
    {
        agentList.Add(boid);
        agents = agentList.ToArray();
        RefeshDataArray();
        boid.Registered = true;
    }

    public void RemoveBoid(BoidAgent boid)
    {
        agentList.Remove(boid);
        agents = agentList.ToArray();
        RefeshDataArray();
        boid.Registered = false;
    }

    private void RefeshDataArray()
    {
        agentTransforms = Array.ConvertAll<BoidAgent, Transform>(agents, (agent => agent.ThisTransform));
        _boidStructs = Array.ConvertAll<BoidAgent, BoidAgent.BoidStruct>(agents, agent => agent.getStruct());
    }

    #endregion
}