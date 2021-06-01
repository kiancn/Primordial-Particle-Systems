using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Purpose of class is to provide at helper to activite/deactive gameobjects on keystroke (or from event)*/
public class UIActivityUtility : MonoBehaviour
{
    [SerializeField] private GameObject element;

    [SerializeField] private KeyCode activationKey = KeyCode.Alpha1;

    [SerializeField] private float fastestSwitchPossibleSeconds = 0.2f;

    private float timeSinceSwitch = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        timeSinceSwitch += Time.deltaTime;

        if (timeSinceSwitch > fastestSwitchPossibleSeconds && Input.GetKeyDown(activationKey))
        {
            SwitchElementActivity();
            timeSinceSwitch = 0f;
        }
    }

    public void SwitchElementActivity()
    {

        if (element == null)
        {
            Debug.Log("UIActivityUtility: You forgot to assign a gameobject");
            return;
        }

        element.SetActive(!element.activeSelf);
    }
}
