using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/*  */
public class SPPCell : MonoBehaviour
{
    [SerializeField] private SPPFieldManager _manager;
    [SerializeField] private SpriteRenderer _renderer;
    private Collider2D thisCollider;
    private Transform thisTransform;
    
    [SerializeField] private String managerName;

    [field: SerializeField] public int CellGrade { get; set; }

    public SpriteRenderer Renderer => _renderer;
    public Collider2D ThisCollider => thisCollider;
    public Transform ThisTransform => thisTransform;

    private void OnEnable()
    {
        
        // _manager = Object.FindObjectOfType<SPPFieldManager>();

        GameObject managerCarrier = GameObject.Find(managerName);
        if (managerCarrier != null)
        {
            SPPFieldManager manager = managerCarrier.GetComponent<SPPFieldManager>();
            if (manager != null)
            {
                _manager = manager;
            }
            else
            {
                Debug.Log("No Manager Assigned to Cell, this is bad. Self-destructing.");
                Destroy(this);
            }
            
        }
        else
        {
            Debug.Log("No Manager Object found by name, this is bad. Self-destructing.");
            Destroy(this);
        }

        _renderer = GetComponent<SpriteRenderer>();
        thisCollider = GetComponent<Collider2D>();
        thisTransform = transform;

        if (_manager != null)
        {
            _manager.Cells.Add(this);
        }
    }

    private void OnDisable()
    {
        if (_manager != null)
        {
            _manager.Cells.Remove(this);
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}