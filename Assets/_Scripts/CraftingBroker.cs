using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* Class instance takes care of handling requests for food points, hull points or bullets - and grant them if possible.
 Crafting is accessible via both keyboard shortcuts and key presses.
 This class is very rigid at the moment, and should be rewritten if project is continued after exam.
 */
public class CraftingBroker : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerStats stats;

    [SerializeField] private KeyCode craftFoodPointsKey = KeyCode.Alpha1;
    [SerializeField] private KeyCode craftHullPointsKey = KeyCode.Alpha2;
    [SerializeField] private KeyCode craftBulletsKey = KeyCode.Alpha3;

    [SerializeField] private UIInventoryCellTextUpdater textUpdater;

    [SerializeField] private UnityEvent onCraftingFail;
    [SerializeField] private UnityEvent onCraftingSuccessFood;
    [SerializeField] private UnityEvent onCraftingSuccessHull;
    [SerializeField] private UnityEvent onCraftingSuccessBullets;
    
    // Update is called once per frame
    void Update()
    {
        /* gonna pull for the keys */
        if (Input.GetKeyDown(craftFoodPointsKey))
        {
            AttemptToCraftFoodPoints();
        }

        if (Input.GetKeyDown(craftHullPointsKey))
        {
            AttemptToCraftHullPoints();
        }

        if (Input.GetKeyDown(craftBulletsKey))
        {
            AttemptToCraftBullets();
        }
    }

    public void AttemptToCraftFoodPoints()
    {
        /* check if enough points are available */
        bool sufficientResources = inventory.state0Cells >= 2 && inventory.state1Cells >= 2;

        /* if points are not available, bail */
        if (!sufficientResources)
        {
            onCraftingFail.Invoke();
            return;
        }

        /* remove points from inventory */
        inventory.state0Cells -= 2; // remember A is 0
        inventory.state1Cells -= 2; // .. B is 1

        textUpdater.UpdateCell1Text(inventory.state0Cells); // this is bad naming, this logic is solid
        textUpdater.UpdateCell2Text(inventory.state1Cells); // again

        /* add points to player stats */

        stats.FoodPointChange(6);
        
        onCraftingSuccessFood.Invoke();
    }

    /* 20 shiphull points cost 5 C cells and 2 D cells */
    public void AttemptToCraftHullPoints()
    {
        /* check if enough points are available */
        bool sufficientResources = inventory.state2Cells >= 5;

        /* if points are not available, bail */
        if (!sufficientResources)
        {
            onCraftingFail.Invoke();
            return;
        }

        /* remove points from inventory */
        inventory.state2Cells -= 5;

        /* update canvas text */
        textUpdater.UpdateCell3Text(inventory.state2Cells); //

        /* add points to player stats */
        stats.HullPointChange(25);

        onCraftingSuccessHull.Invoke();
    }
    
/* Crafting bullets cost 1B 2C 1D */
    public void AttemptToCraftBullets()
    {
        /* check if enough points are available */
        bool sufficientResources = inventory.state1Cells >= 1 && inventory.state2Cells >= 2 && inventory.state3Cells >=1;
        /* if points are not available, bail */
        if (!sufficientResources)
        {
            onCraftingFail.Invoke();
            return;
        }

        /* remove points from inventory */
        inventory.state1Cells -= 1;
        inventory.state2Cells -= 2;
        inventory.state3Cells -= 1;
        
        /* update canvas text */
        textUpdater.UpdateCell2Text(inventory.state1Cells);
        textUpdater.UpdateCell3Text(inventory.state2Cells);
        textUpdater.UpdateCell4Text(inventory.state3Cells);

        /* add points to player stats */
        stats.BulletNumberChange(30);
        
        onCraftingSuccessBullets.Invoke();
    }
}