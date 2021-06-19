using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids2DFlockSettingsAdjuster : MonoBehaviour
{
    [SerializeField] private Boid2DManager manager;

    [SerializeField] private Transform flockTargetTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        manager.settings = new Boid2DManager.Flock2DSettings(flockTargetTransform.position,flockTargetTransform.position,manager.settings.cohesionWeight,manager.settings.avoidanceWeight,manager.settings.flockCenterWeight); 
    }
}
