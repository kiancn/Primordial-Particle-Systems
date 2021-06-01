using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Destroys only cells on collision
public class ProjectileOnColliderDestroyer : MonoBehaviour
{
    [SerializeField] private GameObject echoObject;

    // Start is called before the first frame update
    private void OnEnable()
    {
        echoObject = Instantiate(echoObject, gameObject.transform.position, gameObject.transform.rotation);
        echoObject.SetActive(false);
        Debug.Log("Destroyer enabled!");
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        
        var otherGameObject = other.gameObject;
  

        if (otherGameObject.GetComponent<SPPCell>() != null)
        {

            otherGameObject.SetActive(false);

            Instantiate(echoObject, otherGameObject.transform.position, otherGameObject.transform.rotation);
 
            Destroy(otherGameObject);
        }
    }
}
