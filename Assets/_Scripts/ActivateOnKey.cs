using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnKey : MonoBehaviour
{
    [SerializeField] private KeyCode activationKey = KeyCode.H;
    [SerializeField] private GameObject objectToToggle;

    private bool activated = false;

    private void Start()
    {
        activated = objectToToggle.activeInHierarchy;
    }

    private void Update()
    {
        if (Input.GetKeyDown(activationKey))
        {
            activated = !activated;
            objectToToggle.SetActive(activated);
        }
    }
}