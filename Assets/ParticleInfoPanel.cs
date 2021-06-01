using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticleInfoPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    
    [SerializeField] private Text baseRotationText;
    [SerializeField] private Text perNeighborRotationText;
    [SerializeField] private Text neighborhoodRadiusText;
    [SerializeField] private Text velocityText;
    [SerializeField] private Text currentNeighborsText;
    [SerializeField] private Text particleStateText;

    
    public void ShowParticleInfoPanel(SPPCell cell)
    {
        SPPFieldManager manager = cell.Manager;
        panel.SetActive(true);
        baseRotationText.text = manager.FixedRotation.ToString();
        perNeighborRotationText.text = manager.PerNeighborRotation.ToString();
        neighborhoodRadiusText.text = $"{manager.NeighborhoodRadius}";
        velocityText.text = $"{manager.Velocity}";
        currentNeighborsText.text = cell.currentNumberOfNeighbors.ToString();
        particleStateText.text = ((char) (65 + cell.CellGrade)).ToString();
    }

    public void HideParticleInfoPanel()
    {
        panel.SetActive(false);
    }
    
}
