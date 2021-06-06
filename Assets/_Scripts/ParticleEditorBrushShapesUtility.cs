using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEditorBrushShapesUtility : MonoBehaviour
{
    [SerializeField] private List<GameObject> brushes = new List<GameObject>();

    [SerializeField] private GameObject currentlyActiveBrush;

    [SerializeField] private const KeyCode option_0 = KeyCode.Alpha1;
    [SerializeField] private const KeyCode option_1 = KeyCode.Alpha2;
    [SerializeField] private const KeyCode option_2 = KeyCode.Alpha3;
    [SerializeField] private const KeyCode option_3 = KeyCode.Alpha4;
    [SerializeField] private const KeyCode option_4 = KeyCode.Alpha5;

    private void OnGUI()
    {
        var curKey = Event.current.keyCode;

        switch (curKey)
        {
            case option_0: EffectBrushChange(0); break;
            case option_1: EffectBrushChange(1); break;
            case option_2: EffectBrushChange(2); break;
            case option_3: EffectBrushChange(3); break;
            case option_4: EffectBrushChange(4); break;
            default : break;
        }
    }

    public void OnUIButtonClickBrushChange(int brushListIndex)
    {
        EffectBrushChange(brushListIndex);
    }

    private void EffectBrushChange(int brushListIndex)
    {
        if (brushListIndex > brushes.Count)
        {
            Debug.Log("Less brushes assigned than desired, there are " + brushes.Count +
                      " and desired brush index was " + brushListIndex
                      + "\nI'm bailing out.");
            return;
        }


        if (currentlyActiveBrush != null)
        {
            // do disable of current brush then enable of desired
            currentlyActiveBrush.SetActive(false);
        }

        if (brushes[brushListIndex] != null)
        {
            currentlyActiveBrush = brushes[brushListIndex];
            currentlyActiveBrush.SetActive(true);
        }
    }
}