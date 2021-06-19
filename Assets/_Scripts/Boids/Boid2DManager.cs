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

public class Boid2DManager : MonoBehaviour
{
    #region Variables

    // objects will eventually be searched by name
    [SerializeField] private string boidLayerName = "Boids";
    [SerializeField] private string obstableLayerName = "Obstables";
    [SerializeField] private string sPPCellLayerName = "Cells";
    [SerializeField] private LayerMask boidLayerMask;
    [SerializeField] private LayerMask obstableLayerMask;
    [SerializeField] private LayerMask obstableAndCellsLayerMask;
    [SerializeField] private float cohesionWeight = 1.5f;
    [SerializeField] private float avoidanceWeight = 2f;
    [SerializeField] private float flockCenterWeight = 1f;
    [SerializeField] private float ratioFlockToCenter = 0.8f;
    [SerializeField] private float ratioFlockToLocalCenter = 0.5f;

    [SerializeField] public Flock2DSettings settings;

    [SerializeField] private List<Boid2DAgent> agentList;

    // [SerializeField] private int numberOfBoids;
    private Boid2DAgent[] agents;
    private Transform[] agentTransforms;
    private Boid2DAgent.Boid2D[] _boidStructs;


    [NativeDisableUnsafePtrRestriction]
    private TransformAccessArray _transformAccessArray; // used in job to read/write transforms

    private NativeArray<Boid2DAgent.Boid2D> _boidStructsNArray;

    /*private NativeArray<float3> _rotationMoveResults; // */
    private NativeArray<float3> _rotationResults; // 


    private JobHandle handleMoveJob;
    private JobHandle handleRuleJob;

//    private float obstableSearchRadius = 6;


    [SerializeField] private int countBetweenUpdates = 6;
    private int currentCountBetweenUpdates = 0;
    private bool jobOngoing; // signifies if a job is currently being processed.

    private bool handleMoveJobOngoing;
    private bool coreRulesJobOngoing;
    private NativeArray<float3> _currentSubFlockCenters;

    #endregion

    #region Jobs

    [BurstCompile]
    /* the job to calculate the changes is going to be an ijob, and through it is singlethreaded, it is still much faster*/
    public struct CoreRuleJob : IJob
    {
        private NativeArray<Boid2DAgent.Boid2D> boids;
        private NativeArray<float3> rotationAdjustments; // used fpr calculated rotations
        private NativeArray<float3> currentSubFlockCenters; // used fpr calculated rotations

        public CoreRuleJob(NativeArray<Boid2DAgent.Boid2D> boids, NativeArray<float3> rotationAdjustments,
            NativeArray<float3> currentSubFlockCenters,
            Boid2DManager.Flock2DSettings settings)
        {
            this.boids = boids;
            this.rotationAdjustments = rotationAdjustments;
            this.currentSubFlockCenters = currentSubFlockCenters;
        }

        public void Execute()
        {
            for (int i = 0; i < boids.Length; i++)
            {
                var curBoid = boids[i];

                int numberOfNeighbors = 0;
                int numberOfObstacles = 0;
                float3 rotationAdjustment = float3.zero;
                float3 avoidanceMove = float3.zero;

                for (int j = 0; j < boids.Length; j++)
                {
                    if (curBoid.position.Equals(boids[j].position))
                    {
                        continue;
                    }

                    // unscaled direction from boid to target
                    float3 heading = boids[j].position - curBoid.position;

                    // squared length between boid and other.
                    float sqrDistance = heading.x * heading.x + heading.y * heading.y + heading.z * heading.z;

                    /* bail if thing is too far */
                    if (sqrDistance > curBoid.neighborhoodRadius * curBoid.neighborhoodRadius)
                    {
                        continue;
                    }

                    // this is a neighbor, else an obstable
                    if (sqrDistance > curBoid.avoidanceRadius * curBoid.avoidanceRadius)
                    {
                        /* cohesion */
                        rotationAdjustment += heading * heading / sqrDistance;
                        // cohesionMove += boids[i].direction;
                        currentSubFlockCenters[i] += boids[j].position;
                        numberOfNeighbors++;
                    }
                    else
                    {
                        /* avoidance */
                        avoidanceMove += (boids[j].position - curBoid.position) / math.sqrt(sqrDistance);
                        // avoidanceMove -= heading;
                        numberOfObstacles++;
                    }
                }

                if (numberOfNeighbors > 0)
                {
                    rotationAdjustment /= numberOfNeighbors;
                    currentSubFlockCenters[i] /= numberOfNeighbors;
                }

                if (numberOfObstacles > 0)
                {
                    avoidanceMove /= numberOfObstacles;
                }

                rotationAdjustments[i] += avoidanceMove;
                rotationAdjustments[i] += rotationAdjustment;
            }
        }
    }

    [BurstCompile]
    // purpose of jobs is to effectuate changes calculated in rule job
    public struct MoveBoidsJob : IJobParallelForTransform
    {
        private NativeArray<Boid2DAgent.Boid2D> boids; // used for calculated moves

        private NativeArray<float3> rotationAdjustments; // used fpr calculated rotations
        private NativeArray<float3> currentSubFlockCenters; // used fpr calculated rotations
        private float deltaTime;
        private Boid2DManager.Flock2DSettings _settings;
        private float ratioFlockToCenter;
        private float ratioFlockToLocalCenter;

        public MoveBoidsJob(NativeArray<Boid2DAgent.Boid2D> boids,
            NativeArray<float3> rotationAdjustments, float deltaTime, NativeArray<float3> currentSubFlockCenters,
            Boid2DManager.Flock2DSettings settings, float ratioFlockToCenter, float ratioFlockToLocalCenter)
        {
            this.boids = boids;
            this.rotationAdjustments = rotationAdjustments;
            this.deltaTime = deltaTime;
            this.currentSubFlockCenters = currentSubFlockCenters;
            _settings = settings;
            this.ratioFlockToCenter = ratioFlockToCenter;
            this.ratioFlockToLocalCenter = ratioFlockToLocalCenter;
        }

        public void Execute(int index, TransformAccess transform)
        {
            // var angleToCentre = Vector2.SignedAngle((Vector3) boids[index].position, _settings.flockCenter);
            var angleLocalToCentre =
                Vector2.SignedAngle((Vector3) transform.position, (Vector3) currentSubFlockCenters[index]);


            var neighborhoodAdjustmentAngle =
                Vector2.SignedAngle((Vector3) boids[index].direction, (Vector3) rotationAdjustments[index]);

            
            var angleToMainCentre = Vector2.SignedAngle((Vector3) transform.position, (Vector3) _settings.flockCenter);
            
            float targetAngle; 
            if (neighborhoodAdjustmentAngle == 0)
            {
                targetAngle = transform.rotation.eulerAngles.z + angleToMainCentre;
            }
            else
            {
            
                targetAngle= transform.rotation.eulerAngles.z + math.lerp(neighborhoodAdjustmentAngle, angleLocalToCentre, ratioFlockToLocalCenter);

                 targetAngle = math.lerp(targetAngle, transform.rotation.eulerAngles.z+angleToMainCentre, ratioFlockToCenter);    
            }
            

            var targetRotationEuler = new float3(0, 0, targetAngle);

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                quaternion.Euler(targetRotationEuler),
                boids[index].turnRate * deltaTime);

            transform.position = transform.position +
                                 (Vector3) boids[index].direction * boids[index].currentSpeed * deltaTime;
        }
    }

    private JobHandle GetHandleForCoreRuleJob()
    {
        // Debug.Log("Creating HandleForRulesImplementationJob. Biip.");
        return new CoreRuleJob(_boidStructsNArray, _rotationResults, _currentSubFlockCenters, settings)
            .Schedule();
    }

    private JobHandle CreateHandleForMoveBoidsJob()
    {
        //  Debug.Log("Creating HandleForRulesImplementationJob. Biip.");
        return new MoveBoidsJob(_boidStructsNArray, _rotationResults, Time.deltaTime, _currentSubFlockCenters,
                settings,ratioFlockToCenter,ratioFlockToLocalCenter)
            .Schedule(_transformAccessArray);
    }

    private void ReadyDataForJobs()
    {
        // Debug.Log("Readying Data for jobs. Biip.");
        _transformAccessArray = new TransformAccessArray(agentTransforms);
        _boidStructsNArray = new NativeArray<Boid2DAgent.Boid2D>(_boidStructs, Allocator.TempJob);
        _rotationResults = new NativeArray<float3>(_boidStructs.Length, Allocator.TempJob);
        _currentSubFlockCenters = new NativeArray<float3>(_boidStructs.Length, Allocator.TempJob);
    }

    private void DisposeForAllJobs()
    {
        _transformAccessArray.Dispose();
        _boidStructsNArray.Dispose();
        _rotationResults.Dispose();
        _currentSubFlockCenters.Dispose();
    }

    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.Full);
        Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.Full);
        Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.Full);
        Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.Full);

        boidLayerMask = LayerMask.GetMask(boidLayerName);
        obstableLayerMask = LayerMask.GetMask(obstableLayerName);
        obstableAndCellsLayerMask = LayerMask.GetMask(obstableLayerName, sPPCellLayerName);

        settings = new Boid2DManager.Flock2DSettings(new Vector3(), new Vector3(), cohesionWeight, avoidanceWeight,
            flockCenterWeight);

        agentList = new List<Boid2DAgent>();

        agents = new Boid2DAgent[0];
        _boidStructs = new Boid2DAgent.Boid2D[0];

        currentCountBetweenUpdates = 0;

        Debug.Log("I just awoke!");

        jobOngoing = false;
        handleMoveJobOngoing = false;
        coreRulesJobOngoing = false;

        RefreshDataArrays();
    }

    // Update is called once per frame
    void Update()
    {
        if (handleMoveJobOngoing)
        {
            handleMoveJob.Complete();
            handleMoveJobOngoing = false;

            DisposeForAllJobs();
        }

        RefreshDataArrays();
        if (agents.Length > 0 && !handleMoveJobOngoing && !coreRulesJobOngoing)
        {
            ReadyDataForJobs();

            handleRuleJob = GetHandleForCoreRuleJob();
            coreRulesJobOngoing = true;
        }
    }

    private void LateUpdate()
    {
        if (coreRulesJobOngoing && handleRuleJob.IsCompleted)
        {
            handleRuleJob.Complete();
            coreRulesJobOngoing = false;

            handleMoveJob = CreateHandleForMoveBoidsJob();
            handleMoveJobOngoing = true;
        }
    }


    #region Boids Tracking Section

    private void RefreshDataArrays()
    {
        // Debug.Log("RefreshDataArrays START");
        agents = agentList.ToArray();

        agentTransforms = Array.ConvertAll<Boid2DAgent, Transform>(agents, agent => agent.transform);

        _boidStructs = Array.ConvertAll<Boid2DAgent, Boid2DAgent.Boid2D>(agents, agent => agent.GetStruct());
        // Debug.Log("RefreshDataArrays FINISH");
    }

    public void AddBoid(Boid2DAgent boid)
    {
        if (!agentList.Contains(boid))
        {
            // Debug.Log($"{gameObject.name} : Manager: Adding boid {boid.name}");

            agentList.Add(boid);

            // Debug.Log($"AgentList Count: {agentList.Count.ToString()}");

            RefreshDataArrays();
        }
    }

    public void RemoveBoid(Boid2DAgent boid)
    {
        // Debug.Log($"{gameObject.name} : Manager: removing boid {boid.name}");

        agentList.Remove(boid);

        RefreshDataArrays();
    }

    // updates struct data 

    #endregion

    private void OnDestroy() // this needs to check if any jobs are ongoing and wait for the release of associated data
    {
        DisposeForAllJobs();
    }

    [Serializable]
    [BurstCompile]
    public struct Flock2DSettings
    {
        public Vector3 flockCenter;
        public Vector3 flockTarget;
        public float cohesionWeight;
        public float avoidanceWeight;
        public float flockCenterWeight;

        public Flock2DSettings(Vector3 flockCenter, Vector3 flockTarget, float cohesionWeight, float avoidanceWeight,
            float flockCenterWeight)
        {
            this.flockCenter = flockCenter;
            this.flockTarget = flockTarget;
            this.cohesionWeight = cohesionWeight;
            this.avoidanceWeight = avoidanceWeight;
            this.flockCenterWeight = flockCenterWeight;
        }
    }
}