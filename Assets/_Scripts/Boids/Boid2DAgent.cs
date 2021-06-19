using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

public class Boid2DAgent : MonoBehaviour
{
    [SerializeField] private string managerName = "Boid2D Manager";
    private Boid2DManager _manager;

    /* boid individual settings */
    // possibly this should be Vector3, but of gadw, the conversions

    [SerializeField] private float currentSpeed;
    [SerializeField] private float maxSpeed;

    [SerializeField] private float turnRate;

    [SerializeField] private float neighborhoodRadius;
    [SerializeField] private float avoidanceRadius;

    private Transform thisTransform;


    public bool Registered { get; set; } = false;

    [BurstCompile]
    public struct Boid2D
    {
        public float3 position; // will convert to v3 x, y, 0
        public float3 direction; // direction is z rotation

        public float currentSpeed;
        public float maxSpeed;

        public float turnRate;

        public float neighborhoodRadius;
        public float avoidanceRadius;

        public float3 currentSubFlockCenter;


        public Boid2D(float3 position, float3 direction, float currentSpeed, float maxSpeed, float turnRate,
            float neighborhoodRadius, float avoidanceRadius)
        {
            this.position = position;
            this.direction = direction;
            this.currentSpeed = currentSpeed;
            this.maxSpeed = maxSpeed;
            this.turnRate = turnRate;
            this.neighborhoodRadius = neighborhoodRadius;
            this.avoidanceRadius = avoidanceRadius;
            currentSubFlockCenter = float3.zero;
        }
    }

    public Boid2D GetStruct()
    {
       
        return new Boid2D(thisTransform.position, thisTransform.up, currentSpeed, maxSpeed, turnRate,
            neighborhoodRadius, avoidanceRadius);
    }

    private void Start()
    {
        Registered = false;
        if (gameObject.activeInHierarchy)
        {
            Debug.Log($"{gameObject.name} :: registering :: START");
            Init();
        }
    }

    private void OnEnable()
    {
        if (!Registered)
        {
            Debug.Log($"{gameObject.name} :: registering :: OnEnable");
            Init();
        }
    }

    /* Initializes boid on request
     Initializes components on boid and connection to manager */
    private void Init()
    {
        Debug.Log($"Initializing Boid {this.name}");
        if (!Registered)
        {
            if (_manager == null)
            {
                GameObject possibleManager = GameObject.Find(managerName);
                if (possibleManager != null)
                {
                    var possibleManagerByClass = possibleManager.GetComponent<Boid2DManager>();
                    if (possibleManagerByClass != null)
                    {
                        _manager = possibleManagerByClass;
                    }
                }
            }

            if (_manager == null)
            {
                Debug.Log("Manager not assigned and not found by name. Setting inactive.");
                gameObject.SetActive(false);
                return;
            }
            else
            {
                Debug.Log($"Manager: '{_manager.name}' found.");
            }

            thisTransform = gameObject.transform;

            _manager.AddBoid(this);
            Registered = true;
        }
    }

    private void OnDisable()
    {
        if (_manager != null)
        {
            _manager.RemoveBoid(this);
            Registered = false;
            Debug.Log($"Removing '{gameObject.name}' from manager '{_manager.name}'.");
        }
    }

    private void OnDestroy()
    {
        if (_manager != null)
        {
            _manager.RemoveBoid(this);
        }
    }
}