using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BoidAgent : MonoBehaviour
{
    [BurstCompile]
    public struct BoidStruct
    {
        public float3 position;
        public float3 direction;
        public quaternion rotation;
        public float currentSpeed;
        public float turnRate;
        public float neighborhoodRadius;
        public float avoidanceRadius;
        
        
        public BoidStruct(Vector3 position, Vector3 direction,quaternion rotation, float currentSpeed,float turnRate, float neighborhoodRadius, float avoidanceRadius)
        {
            this.position = position; 
            this.direction = direction;
            this.rotation = rotation;
            this.currentSpeed = currentSpeed;
            this.turnRate = turnRate;
            this.neighborhoodRadius = neighborhoodRadius;
            this.avoidanceRadius = avoidanceRadius;
        }
    }

    public BoidStruct getStruct()
    {
        thisTransform = gameObject.transform;
        
        // var newBoidStruct = new BoidStruct();
        // newBoidStruct.position = thisTransform.position;
        // newBoidStruct.direction = thisTransform.up;
        // newBoidStruct.currentSpeed = currentSpeed;
        // newBoidStruct.turnRate = turnRate;
        // newBoidStruct.neighborhoodRadius = neighborHoodRadius;
        //
        // return newBoidStruct;
        return new BoidStruct(thisTransform.position, thisTransform.up,thisTransform.rotation, currentSpeed,turnRate,neighborHoodRadius,avoidanceRadius);
    }

    [SerializeField] private float avoidanceRadius = .5f;
    [SerializeField] private float neighborHoodRadius = 3f;

    /* Some behavior variables are boid-specific, others are defined via the manager.
     All time-dependent variables are; x per second (unless otherwise stated in var name)*/
    [SerializeField] private float maxSpeed = 3.5f;
    [SerializeField] private float turnRate = 1f;

    [SerializeField] private BoidManager manager;

    /* current speed of boid per second */
    [SerializeField] private float currentSpeed;

    [SerializeField] private SpriteRenderer _renderer;
    private PolygonCollider2D thisCollider;
    private Transform thisTransform;
    
    public SpriteRenderer Renderer => _renderer;
    public Collider2D ThisCollider => thisCollider;
    public Transform ThisTransform => thisTransform;

    public Vector3 Position => thisTransform.position;

    public bool Registered { get; set; } = false;

    private void Init()
    {
        if (manager == null)
        {
            manager = Object.FindObjectOfType<BoidManager>();
        }

        if (manager == null)
        {
            Debug.Log("Boid did not find manager, this means stasis. Oh gawd! Disabling."); 
            gameObject.SetActive(false);
            return;
        }
        
        manager.AddBoid(this);

        _renderer = GetComponent<SpriteRenderer>();
        thisCollider = GetComponent<PolygonCollider2D>();
        thisTransform = transform;
    }

    private void Start()
    {
        Registered = false;
        if (gameObject.activeInHierarchy)
        {
            Init();
        }
    }

    private void OnEnable()
    {
        if (!Registered)
        {
            Init();
        }
    }

    private void OnDisable()
    {
        manager.RemoveBoid(this);
    }

    private void OnDestroy()
    {
        manager.RemoveBoid(this);
    }
}
