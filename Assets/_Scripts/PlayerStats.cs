using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* Handles stats and damage, fires events on stats change. This class is a prototype mishmash*/
public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int baseFood = 255;
    [field: SerializeField] public int CurrentFood { get; private set; }
    [SerializeField] private int maxHullPoints = 180;
    [field: SerializeField] public int CurrentHullPoints { get; private set; }
    [SerializeField] private int maxBullets = 120;
    [field: SerializeField] public int CurrentBullets { get; private set; }

    [SerializeField] public float foodDrainInterval = 4f;
    [SerializeField] private float currentTimeSinceLastFoodDrain;

    [SerializeField] private int particlesForWin = 50;

    [SerializeField] private PlayerInventory _inventory;

    [SerializeField] private UnityEvent onLoseGameEvent;
    [SerializeField] private UnityEvent onWinGameEvent;

    [SerializeField] private UnityEvent<int> onCollisionEvent;

    /* events to a on point changes; the ui prefabs  */
    [SerializeField] private UnityEvent<int> foodPointChangeEvent;
    [SerializeField] private UnityEvent<int> bulletPointChangeEvent;
    [SerializeField] private UnityEvent<int> hullPointChangeEvent;

    [SerializeField] private float currentGameTime;

    [SerializeField] private GameObject onDestructiveHullContactPrefab;
    [SerializeField] private GameObject onDestructiveHullContactPreSpawn;

    private bool gameDecided = false;

    // Start is called before the first frame update
    void OnEnable()
    {
        onDestructiveHullContactPreSpawn = Instantiate(onDestructiveHullContactPrefab,
            new Vector3(-100, -100, 0), Quaternion.identity);
        onDestructiveHullContactPreSpawn.SetActive(false);

        CurrentFood = baseFood;
        CurrentHullPoints = maxHullPoints;
        CurrentBullets = maxBullets / 2;

        foodPointChangeEvent
            .Invoke(baseFood); // these events will be the point text updates and possinbly some glamour 
        hullPointChangeEvent.Invoke(maxHullPoints);
        bulletPointChangeEvent.Invoke(CurrentBullets);

        currentGameTime = 0f;
        gameDecided = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameDecided && CheckWinCondition())
        {
            gameDecided = true;
            Debug.Log("Player Won!");
            onWinGameEvent.Invoke();
            return;
        }

        if (!gameDecided && CheckLosingConditions())
        {
            gameDecided = true;
            Debug.Log("Player did of Hunger or hull damage.");
            onLoseGameEvent.Invoke();
        }
    }

    private void FixedUpdate()
    {
        currentTimeSinceLastFoodDrain += Time.deltaTime;
        if (currentTimeSinceLastFoodDrain > foodDrainInterval)
        {
            FoodPointChange(-1);
            currentTimeSinceLastFoodDrain = 0f;
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

    /* checks for win conditions and returns true, if the game is won*/
    private bool CheckWinCondition()
    {
        bool particleDGoalReached = _inventory.state3Cells >= particlesForWin;
        bool particleEGoalReached = _inventory.state4Cells >= particlesForWin;
        bool particleFGoalReached = _inventory.state5Cells >= particlesForWin;

        return particleDGoalReached && particleEGoalReached && particleFGoalReached;
    }

    /* checks for losing conditions and returns true, if the game is lost*/
    private bool CheckLosingConditions()
    {
        bool shipWrecked = CurrentHullPoints <= 0;
        bool playerDiedOfHunger = CurrentFood <= 0;


        return shipWrecked || playerDiedOfHunger;
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
}