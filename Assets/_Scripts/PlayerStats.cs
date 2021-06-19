using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* Handles stats and damage, fires events on stats change.
 Crucially, it handles win and lose conditions. 
 This class is a prototype mishmash. */
public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int baseFood = 255;
    [field: SerializeField] public int CurrentFood { get; private set; }
    [SerializeField] private int maxHullPoints = 180;
    [field: SerializeField] public int CurrentHullPoints { get; private set; }
    [SerializeField] private int maxBullets = 120;
    [field: SerializeField] public int CurrentBullets { get; private set; }

    [field: SerializeField] public int PlayerLevel { get; private set; } = 0;

    [SerializeField] public float foodDrainInterval = 4f;
    [field: SerializeField] public float CurrentTimeSinceLastFoodDrain { get; set; }

    [SerializeField] public int particlesForWinState0 = 50;
    [SerializeField] public int particlesForWinState1 = 50;
    [SerializeField] public int particlesForWinState2 = 50;
    [SerializeField] public int particlesForWinState3 = 50;
    [SerializeField] public int particlesForWinState4 = 50;
    [SerializeField] public int particlesForWinState5 = 50;

    [SerializeField] private PlayerInventory _inventory;

    [SerializeField] public UnityEvent OnLoseGameEvent;
    [field: SerializeField] public UnityEvent OnWinGameEvent;

    [SerializeField] private UnityEvent<int> onCollisionEvent;

    /* events point to resource-point changes; the ui prefabs  */
    [SerializeField] private UnityEvent<int> foodPointChangeEvent;
    [SerializeField] private UnityEvent<int> bulletPointChangeEvent;
    [SerializeField] private UnityEvent<int> hullPointChangeEvent;
    [SerializeField] private UnityEvent<int> levelChangeChangeEvent;

    [SerializeField] private float currentGameTime;

    [SerializeField] private GameObject onDestructiveHullContactPrefab;
    [SerializeField] private GameObject onDestructiveHullContactPreSpawn;

    public bool  GameDecided { get; set; } = false;

    // Start is called before the first frame update
    void OnEnable()
    {
        onDestructiveHullContactPreSpawn = Instantiate(onDestructiveHullContactPrefab,
            new Vector3(-100, -100, 0), Quaternion.identity);
        onDestructiveHullContactPreSpawn.SetActive(false);

        CurrentFood = baseFood;
        CurrentHullPoints = maxHullPoints;
        CurrentBullets = maxBullets / 2;

        // these events will be the point text updates and possinbly some glamour 
        foodPointChangeEvent.Invoke(baseFood);
        hullPointChangeEvent.Invoke(maxHullPoints);
        bulletPointChangeEvent.Invoke(CurrentBullets);
        levelChangeChangeEvent.Invoke(PlayerLevel);

        currentGameTime = 0f;
        GameDecided = false;
    }

    private void FixedUpdate()
    {
        CurrentTimeSinceLastFoodDrain += Time.deltaTime;
        if (CurrentTimeSinceLastFoodDrain > foodDrainInterval)
        {
            FoodPointChange(-1);
            CurrentTimeSinceLastFoodDrain = 0f;
        }

        currentGameTime += Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        CurrentHullPoints--;
        onCollisionEvent.Invoke(CurrentHullPoints);

        ContactPoint2D[] contactPoints = new ContactPoint2D[10];

        other.GetContacts(contactPoints);

        foreach (var point in contactPoints)
        {
            var impactObject = Instantiate(onDestructiveHullContactPreSpawn, point.point, Quaternion.identity);
            impactObject.SetActive(true);
        }
    }


    public void FoodPointChange(int change)
    {
        CurrentFood += change;
        foodPointChangeEvent.Invoke(CurrentFood);
    }

    public void HullPointChange(int change)
    {
        CurrentHullPoints += change;
        hullPointChangeEvent.Invoke(CurrentHullPoints);
    }

    public void BulletNumberChange(int change)
    {
        CurrentBullets += change;
        bulletPointChangeEvent.Invoke(CurrentBullets);
    }

    public void PlayerLevelChangeEvent(int change)
    {
        PlayerLevel += change;
        levelChangeChangeEvent.Invoke(PlayerLevel);
    }
}