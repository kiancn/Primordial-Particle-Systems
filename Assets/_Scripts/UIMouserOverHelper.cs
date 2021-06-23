using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public class UIMouserOverHelper : MonoBehaviour
{

    [SerializeField] private UnityEvent onMouseEnter;
    [SerializeField] private UnityEvent onMouseExit;
    [SerializeField] private RectTransform rectTransform;
    
    private bool _isActive = false;

    private void Start()
    {
        if (rectTransform == null)
        {
            rectTransform = gameObject.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                Debug.Log("No rect transform found, please put this script on a ui element to be useful. Exceptions will explode now.");        
            }
        }
    }

    /// calculate the occupied extreme coordinates (side-lines) of the panel
    /// if mouse position is within coordinates, react. 
    private void FixedUpdate()
    {
        var mouseIsInsideRect = IsMouseOverRect();
        
        if (mouseIsInsideRect && !_isActive)
        {
            _isActive = true;
            onMouseEnter.Invoke();
            return;
        }

        if (!mouseIsInsideRect && _isActive)
        {
            _isActive = false;
            onMouseExit.Invoke();
        }
    }

    private bool IsMouseOverRect()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition);
    }

    
}
