using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalInfoUpdater : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerStats stats;

    [SerializeField] private LevelGoalUIPanelTextHolder particleATexts;
    [SerializeField] private LevelGoalUIPanelTextHolder particleBTexts;
    [SerializeField] private LevelGoalUIPanelTextHolder particleCTexts;
    [SerializeField] private LevelGoalUIPanelTextHolder particleDTexts;
    [SerializeField] private LevelGoalUIPanelTextHolder particleETexts;
    [SerializeField] private LevelGoalUIPanelTextHolder particleFTexts;

    [SerializeField] private int updatesBetweenTextRefresh = 20;

    private int updatesSinceTextRefresh;

    // Update is called once per frame
    void FixedUpdate()
    {
        updatesSinceTextRefresh++;
        if (updatesSinceTextRefresh >= updatesBetweenTextRefresh)
        {
            updatesSinceTextRefresh = 0;

            updateText(particleATexts,inventory.state0Cells,stats.particlesForWinState0);
            updateText(particleBTexts,inventory.state1Cells,stats.particlesForWinState1);
            updateText(particleCTexts,inventory.state2Cells,stats.particlesForWinState2);
            updateText(particleDTexts,inventory.state3Cells,stats.particlesForWinState3);
            updateText(particleETexts,inventory.state4Cells,stats.particlesForWinState4);
            updateText(particleFTexts,inventory.state5Cells,stats.particlesForWinState5);
        }
    }

    private void updateText(LevelGoalUIPanelTextHolder particleTexts,int have,int goal)
    {
        particleTexts.textHave.text = have.ToString();
        particleTexts.textGoal.text = goal.ToString();
        int needed = goal - have;
        particleTexts.textNeed.text = needed <= 0 ? "" : needed.ToString();
        particleTexts.successObject.SetActive(needed <= 0);
    }
}