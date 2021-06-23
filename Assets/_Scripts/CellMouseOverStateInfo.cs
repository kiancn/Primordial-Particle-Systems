using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMouseOverStateInfo : MonoBehaviour
{
    [SerializeField] private ParticleInfoPanel infoPanel;
    [SerializeField] private SPPCell cell;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        infoPanel = FindObjectOfType<ParticleInfoPanel>();
        if (cell == null)
        {
            cell = GetComponent<SPPCell>();
        }
        
        if(!cell || !infoPanel){Debug.Log("Autodetecting Panel and Cell/Particle failed. This is critical!");}
    }

    private void OnMouseOver()
    {
        infoPanel.ShowParticleInfoPanel(cell);
    }

    private void OnMouseExit()
    {
        infoPanel.HideParticleInfoPanel();
    }
}