using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWithinDiameter : MonoBehaviour
{
    [SerializeField] private GameObject thingToPlace;

    [SerializeField] private float radius;
    [SerializeField] private bool randomWithinDiameter = true;

    [SerializeField] private float secondsBetweenSpawns = 1;

    private float secondsSinceLastSpawn;

    [SerializeField] private Transform ownTransform;

    [SerializeField] private int spawnNumberLimit = 500;

    private int spawnNumberCurrent;


    void OnEnable()
    {
        spawnNumberCurrent = 0;
        
        if (thingToPlace == null)
        {
            Debug.LogWarning(
                "No thing to place assigned. Very bad things would happen, if I didn't go inactive. Please assign spawnable object.");
            gameObject.SetActive(false);
            return;
        }

        ownTransform = gameObject.transform;

        thingToPlace = Instantiate(thingToPlace, new Vector3(130, 130, 0), Quaternion.identity);
        thingToPlace.SetActive(false);

        secondsSinceLastSpawn = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        secondsSinceLastSpawn = secondsSinceLastSpawn + Time.deltaTime;

        if (secondsSinceLastSpawn > secondsBetweenSpawns && spawnNumberCurrent < spawnNumberLimit)
        {
            var circRandPos = Random.insideUnitCircle * (radius * Random.value);
            var newObject = Instantiate(thingToPlace,
                new Vector3(circRandPos.x - ownTransform.position.x, circRandPos.y - ownTransform.position.y, 0),
                Quaternion.identity);
            newObject.SetActive(true);
            secondsSinceLastSpawn = 0f;

            spawnNumberCurrent++;
        }
    }
}