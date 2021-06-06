using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SPPUIPanelTextUpdater : MonoBehaviour
{
    [FormerlySerializedAs("fieldManager")] [SerializeField] private SPPFieldManager sppFieldManager;

    [SerializeField] private Text cellsInTotalText;

    [SerializeField] private Text baseRotText;
    [SerializeField] private Text perNeighborRotText;
    [SerializeField] private Text neighborhoodRadiusText;
    [SerializeField] private Text velocityText;

    [SerializeField] private float textUpdateIntervalSeconds = .4f;


    private float _timeCounter = 0;

    // Called when object becomes active.
    void OnEnable()
    {
        UpdateVelocityText(0); // none of the methods use the argued value; it needs to be there to for me to be able to hook into the unityevent, that triggers these methods at runtime.
        UpdateBaseRotationText(0);
        UpdateNeighborhoodRadiusText(0);
        UpdatePerNeighborRotText(0);
    }

    // Update is called 60 times each second
    void FixedUpdate()
    {
        _timeCounter += .016f;

        if (_timeCounter > textUpdateIntervalSeconds)
        {
            cellsInTotalText.text = sppFieldManager.Cells.Length.ToString();
            _timeCounter = 0f;
        }
    }

    public void UpdateBaseRotationText(float value)
    {
        baseRotText.text = Math.Round(sppFieldManager.FixedRotation, 2).ToString();
        // baseRotText.text = Math.Round(value, 2).ToString();
    }

    public void UpdatePerNeighborRotText(float value)
    {
        perNeighborRotText.text = Math.Round(sppFieldManager.PerNeighborRotation, 2).ToString();
        // perNeighborRotText.text = Math.Round(value, 2).ToString();
    }

    public void UpdateVelocityText(float value)
    {
        velocityText.text = Math.Round(sppFieldManager.Velocity, 2).ToString();
    }

    public void UpdateNeighborhoodRadiusText(float value)
    {
        neighborhoodRadiusText.text = Math.Round(sppFieldManager.NeighborhoodRadius, 2).ToString();
    }
}