using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Purpose of the class is to serve as an adjustable - straight-flying bullet
public class Bullet2DTopDown : MonoBehaviour
{
    [field: SerializeField] public float Speed { get; set; }
    [SerializeField] private Transform bulletTransform;

    [SerializeField] private GameObject impactObject;

    [SerializeField] private float timeToLive = 3.2f;

    [SerializeField] private Rigidbody2D body;
    
    [SerializeField] private Player2DMoverTopdown playerMover;

    private float timeSinceEnable = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (impactObject == null)
        {
            Debug.LogWarning("No impact object assigned to bullet; this is bad. Destroying this bullet now.");
            Destroy(this);
        }

        playerMover = FindObjectOfType<Player2DMoverTopdown>();

        if (playerMover == null)
        {
            Debug.LogWarning("Could not find a Player2DMoverTopdown object not found in scene.");
        }
        else
        {
            Speed = playerMover.CurrentMoveSpeed + Speed;
        }

        impactObject = Instantiate(impactObject, bulletTransform.position, bulletTransform.rotation);
        impactObject.SetActive(false);

        bulletTransform = this.transform;

        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var dT = Time.deltaTime;

        timeSinceEnable += dT;

        var fwAtSpeed = bulletTransform.up * (Speed * dT);
        // bulletTransform.Translate();

        body.MovePosition(bulletTransform.position + fwAtSpeed);
        // bulletTransform.Translate(fwAtSpeed);

        if (timeSinceEnable > timeToLive)
        {
            SpawnImpactObject_DisableSelf();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var otherGameObject = other.gameObject;

        if (otherGameObject.transform != body.transform && otherGameObject.GetComponent<Explosion2DTopdown>() == null)
        {
            SpawnImpactObject_DisableSelf();
        }
    }

    private void SpawnImpactObject_DisableSelf()
    {
     //   Debug.Log("SpawnImpactObject_DisableSelf");
        
        impactObject.transform.position = bulletTransform.position;
        impactObject.transform.rotation = bulletTransform.rotation;
        impactObject.SetActive(true);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}