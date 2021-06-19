using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion2DTopdown : MonoBehaviour
{
    [SerializeField] private float timeToLive = 0.81f;

    [SerializeField] private GameObject explosionObject; // this should be set to the object itself

    [SerializeField] private float timeSinceEnable = 0;

    private void OnEnable()
    {
        if (explosionObject == null)
        {
            Debug.Log("You forgot to set the explosion object. Restart scene, adjust prefab to continue");
        }
    }

    private void FixedUpdate()
    {
        timeSinceEnable += Time.deltaTime;

        if (timeSinceEnable > timeToLive)
        {
            gameObject.GetComponent<Collider2D>().enabled = false;

            // Debug.Log(gameObject.name + " is being destroyed.");
            Destroy(explosionObject);
            
        }
    }
}