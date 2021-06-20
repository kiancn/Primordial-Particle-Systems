using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIMouserOverHelper : MonoBehaviour
{

    [SerializeField] private UnityEvent onMouseEnter;
    [SerializeField] private UnityEvent onMouseExit;
    
    private bool _IsActive = false;
    
    void OnGUI()
    {
        // if (EventSystem.current.IsPointerOverGameObject() && !_IsActive)
        // {
        //     _IsActive = true;
        //     
        //     onMouseEnter.Invoke();
        //     Debug.Log($"Mouse registered entering UI element ' {gameObject.name} '");
        //     return;
        // }
        //
        // if(_IsActive)
        // {
        //     onMouseExit.Invoke();
        //     _IsActive = false;
        //     Debug.Log($"Mouse registered exiting UI element ' {gameObject.name} '");
        // }
    }

    private void OnMouseEnter()
    {
        onMouseEnter.Invoke();
    }

    private void OnMouseExit()
    {
        onMouseEnter.Invoke();
    }
    //
    // private bool IsMouseOverUILayerSelected()
    // {
    //     PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
    //     pointerEventData.
    // }
    //
}
