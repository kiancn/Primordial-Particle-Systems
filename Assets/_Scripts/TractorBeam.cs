using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class TractorBeam : MonoBehaviour
{
    [SerializeField] private GameObject overloadPrefab;

    [SerializeField] private Transform zeroPoint;

    public SPPCell detectedCell;

    [SerializeField] private float absorbtionDistance = 0.1f;
    [SerializeField] private float beamTractionSpeed = 5f;

    [SerializeField] private PlayerInventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<PlayerInventory>();
        if (inventory == null)
        {
            Debug.Log("Something went terrinly wrong, no inventory found. Disabling.");
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var cell = other.GetComponent<SPPCell>();
        if (cell != null)
        {
            if (Vector2.Distance(cell.ThisTransform.position, zeroPoint.position) < absorbtionDistance)
            {
                inventory.AddResource(cell.CellGrade);
                Destroy(other.gameObject);
            }
            else
            {
                var oTransform = other.transform;
                var newPos = Vector3.MoveTowards(oTransform.position, zeroPoint.position,
                    beamTractionSpeed * Time.deltaTime);
                oTransform.SetPositionAndRotation(newPos,oTransform.rotation);
            }
        }
    }
}