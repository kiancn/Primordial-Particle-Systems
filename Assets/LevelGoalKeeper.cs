using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalKeeper : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private List<LevelParticleGoals> levelGoals;

    [SerializeField] private float pointWithdrawalRateInSeconds = 0.2f;

    /* */
    [Serializable]
    public struct LevelParticleGoals
    {
        public int[] particlesForWinState;
    }


    public void AdjustPointsAndGoalForNextLevel()
    {
        WithdrawPointsForLevel(playerStats.PlayerLevel);
    }

    private void SetGoalsForLevel(int playerStatsPlayerLevel)
    {
        playerStats.particlesForWinState0 = levelGoals[playerStatsPlayerLevel].particlesForWinState[0];
        playerStats.particlesForWinState1 = levelGoals[playerStatsPlayerLevel].particlesForWinState[1];
        playerStats.particlesForWinState2 = levelGoals[playerStatsPlayerLevel].particlesForWinState[2];
        playerStats.particlesForWinState3 = levelGoals[playerStatsPlayerLevel].particlesForWinState[3];
        playerStats.particlesForWinState4 = levelGoals[playerStatsPlayerLevel].particlesForWinState[4];
        playerStats.particlesForWinState5 = levelGoals[playerStatsPlayerLevel].particlesForWinState[5];
    }


    public void WithdrawPointsForLevel(int levelGoalsListIndex)
    {
        if (levelGoals != null && levelGoalsListIndex >= 0 && levelGoalsListIndex < levelGoals.Count)
        {
            StartCoroutine(WithdrawPointsIncrementally(levelGoalsListIndex));
        }
    }

    private IEnumerator WithdrawPointsIncrementally(int levelGoalsIndex)
    {
        LevelParticleGoals goal = levelGoals[levelGoalsIndex];
        int withdrawals = 0;
        int particleTypeCurrent = 0;

        int[] currentParticleInventory =
        {
            playerInventory.state0Cells,
            playerInventory.state1Cells,
            playerInventory.state2Cells,
            playerInventory.state3Cells,
            playerInventory.state4Cells,
            playerInventory.state5Cells
        };

        // returns false when player inventory does not allow more withdrawals, enough withdrawals have been made.
        bool CheckWithdrawPointsOfType(int particleType, int withDrawnSoFar,
            int particleOfTypeInInventory, LevelParticleGoals goal) // 0 is A
        {
            // any further checking is unnessecary if the particle type has no goal this level
            if (goal.particlesForWinState[particleType] > 0)
            {
                // enough particles of this type have been withdrawn
                if (withDrawnSoFar >= goal.particlesForWinState[particleType]) { return false; }

                // not more points to withdraw
                if (particleOfTypeInInventory < 1) { return false; }

                // return true because not enough points have been withdrawn, and player has particles of the type in inventory.
                return true;
            }

            return false; // returning false, because particle type has no 
        }

        bool WithdrawPoints(int particleType, int particlesInInventory)
        {
            if (CheckWithdrawPointsOfType(particleType, withdrawals, particlesInInventory, goal))
            {
                playerInventory.RemoveResource(particleType);
                currentParticleInventory[particleType]--;
                withdrawals++;
                return true;
            }

            return false;
        }
        
        for (int particleType = 0; particleType < currentParticleInventory.Length; particleType++)
        {
            // goal = levelGoals[levelGoalsIndex];
            withdrawals = 0;
            while (WithdrawPoints(particleType, currentParticleInventory[particleType]))
            {
                yield return new WaitForSecondsRealtime(pointWithdrawalRateInSeconds);
            }

            // levelGoalsIndex++;
            particleTypeCurrent++;
        }
        
        playerStats.PlayerLevelChangeEvent(1);
        SetGoalsForLevel(playerStats.PlayerLevel);
    }
    
        // Update is called once per frame
    void FixedUpdate()
    {
        if (!playerStats.GameDecided && CheckWinCondition())
        {
            playerStats.GameDecided = true;
            Debug.Log("Player Won!");
            // playerStats.playerLevel++;
            playerStats.OnWinGameEvent.Invoke();
            return;
        }

        if (!playerStats.GameDecided && CheckLosingConditions())
        {
            playerStats.GameDecided = true;
            Debug.Log("Player died of hunger or hull damage.");
            playerStats.OnLoseGameEvent.Invoke();
        }
    }



    /* checks for win conditions and returns true, if the game is won*/
    private bool CheckWinCondition()
    {
        bool particleAGoalReached = playerInventory.state0Cells >= playerStats.particlesForWinState0;
        bool particleBGoalReached = playerInventory.state1Cells >= playerStats.particlesForWinState1;
        bool particleCGoalReached = playerInventory.state2Cells >= playerStats.particlesForWinState2;
        bool particleDGoalReached = playerInventory.state3Cells >= playerStats.particlesForWinState3;
        bool particleEGoalReached = playerInventory.state4Cells >= playerStats.particlesForWinState4;
        bool particleFGoalReached = playerInventory.state5Cells >= playerStats.particlesForWinState5;

        bool conditionsReached =particleAGoalReached && particleBGoalReached && particleCGoalReached &&
                       particleDGoalReached && particleEGoalReached && particleFGoalReached;

        // if (gameLost)
        // {
        //     Debug.Log("Game is not won yet.");
        // }

        return conditionsReached;

        // return particleAGoalReached && particleBGoalReached && particleCGoalReached &&
        //        particleDGoalReached && particleEGoalReached && particleFGoalReached;
    }

    /* checks for losing conditions and returns true, if the game is lost*/
    private bool CheckLosingConditions()
    {
        bool shipWrecked = playerStats.CurrentHullPoints <= 0;
        bool playerDiedOfHunger = playerStats.CurrentFood <= 0;


        return shipWrecked || playerDiedOfHunger;
    }
}