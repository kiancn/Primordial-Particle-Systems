using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/*  */
public class SPPCell : MonoBehaviour
{
    [field: SerializeField] public SPPFieldManager Manager { get; private set; }
    [SerializeField] private SpriteRenderer _renderer;
    private Collider2D thisCollider;
    private Transform thisTransform;

    public int currentNumberOfNeighbors = 0;

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
                Manager = manager;
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

        // if (Manager != null)
        // {
        //     SPPCell[] newCells = new SPPCell[Manager.Cells.Length+1];
        //     
        //     Manager.Cells.CopyTo(newCells,0);
        //     newCells[newCells.Length-1] = this;
        //
        //     Manager.Cells = newCells;
        //     
        //     //Manager.Cells.Add(this);
        // }
    }

    private void OnDisable()
    {
     //   if (Manager != null)
        {
       //     Manager.Cells = Array.FindAll(Manager.Cells, (x) => { return this != x; }).ToArray();
                //Manager.Cells.Remove(this);
        }
    }

    private void OnDestroy()
    {
     //   OnDisable();
    }
}