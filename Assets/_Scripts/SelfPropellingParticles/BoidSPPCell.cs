using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/*  */
public class BoidSPPCell : MonoBehaviour
{
    [SerializeField] private SPPFieldManager _manager;
    [SerializeField] private SpriteRenderer _renderer;
    private Collider2D thisCollider;
    private Transform thisTransform;

     
    
    public SpriteRenderer Renderer => _renderer;
    public Collider2D ThisCollider => thisCollider;
    public Transform ThisTransform => thisTransform;

    private void OnEnable()
    {
        _manager = Object.FindObjectOfType<SPPFieldManager>();
        _renderer = GetComponent<SpriteRenderer>();
        thisCollider = GetComponent<Collider2D>();
        thisTransform = transform;

        if (_manager != null)
        {
            //_manager.Cells.Add(this);
        }
    }

}