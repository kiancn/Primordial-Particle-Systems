using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* Switches canvas on and off, and handles pausing intelligently */
public class MenuSwitcherPausing : MonoBehaviour
{
    [SerializeField] private bool useKeyActivation = true;
    
    [SerializeField] private KeyCode activationKey = KeyCode.Escape;

    [SerializeField] private ScenePause scenepauser;

    [SerializeField] private GameObject menuCanvasObject;

    [SerializeField] private bool pausedBeforeMenuActivation;

    [SerializeField] private UnityEvent onEventActivate;
    [SerializeField] private UnityEvent onEventDeactivate;

    // Update is called once per frame
    void Update()
    {
        if (useKeyActivation && Input.GetKeyDown(activationKey))
        {
            // switch canvas off if on
            SwitchMenuState();
        }
    }

    [SerializeField] private bool pauseOnActivate = true;

    public void SwitchMenuState()
    {
        if (!menuCanvasObject.activeInHierarchy && pauseOnActivate)  
        {
            if (scenepauser.pausedNow)
            {
                pausedBeforeMenuActivation = true;
                menuCanvasObject.SetActive(true);
                scenepauser.externallyLocked = true;
            }
            else
            {
                scenepauser.PauseSwitch();
                scenepauser.externallyLocked = true;
                pausedBeforeMenuActivation = false;
                menuCanvasObject.SetActive(true);
            }
            onEventActivate.Invoke();
            return;
        }
        else if(menuCanvasObject.activeInHierarchy && pauseOnActivate)
        {
            if (pausedBeforeMenuActivation)
            {
                scenepauser.externallyLocked = false;
                menuCanvasObject.SetActive(false);
            }
            else
            {
                menuCanvasObject.SetActive(false);
                scenepauser.externallyLocked = false;
                scenepauser.PauseSwitch();
            }
            onEventDeactivate.Invoke();
            pausedBeforeMenuActivation = scenepauser.pausedNow;
            return;
        }
        
        /* no pause on activate sections, annoyingly close to identical */
        
        if (!menuCanvasObject.activeInHierarchy)  
        {
            if (scenepauser.pausedNow)
            {
                pausedBeforeMenuActivation = true;
                menuCanvasObject.SetActive(true);
            }
            else
            {
                menuCanvasObject.SetActive(true);
            }
            onEventActivate.Invoke();
        }
        else if(menuCanvasObject.activeInHierarchy)
        {
            if (pausedBeforeMenuActivation)
            {
                menuCanvasObject.SetActive(false);
            }
            else
            {
                menuCanvasObject.SetActive(false);
            }
            
            pausedBeforeMenuActivation = scenepauser.pausedNow;

            onEventDeactivate.Invoke();            
            
        }
    }
}