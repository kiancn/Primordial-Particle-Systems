using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEditorBrushShapesUtility : MonoBehaviour
{
    [SerializeField] private List<GameObject> brushes = new List<GameObject>();

    [SerializeField] private GameObject currentlyActiveBrush;


 public void OnUIButtonClickBrushChange(int brushListIndex)
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