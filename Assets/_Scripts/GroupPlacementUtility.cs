using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupPlacementUtility : MonoBehaviour
{
    [SerializeField] private List<Transform> placementPoints = new List<Transform>();
    private Transform thisTransform;
    
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject prespawnedPrefab;
    [SerializeField] private KeyCode assistKey = KeyCode.LeftShift;
    
    [SerializeField] private bool useAssistKey = true;

    [SerializeField] private string spawnParentName = "Cell Parent";
    private Transform spawnParentTransform;

    public static bool PlacementAllowed { get; set; } = true;

    // Start is called before the first frame update
    void Start()
    {
        spawnParentTransform = GameObject.Find(spawnParentName).transform;
        
        thisTransform = gameObject.transform;
        _camera = FindObjectOfType<Camera>();
        prespawnedPrefab = Instantiate(prefab);
        prespawnedPrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        var mousePos = GetMousePositionAndNormal();

        thisTransform.position = mousePos[0];
        
        if (PlacementAllowed)
        {
            if (!useAssistKey && Input.GetMouseButtonDown(0) || useAssistKey && Input.GetKey(assistKey) && Input.GetMouseButtonDown(0))
            {
                foreach (var point in placementPoints)
                {
                    var newGo = Instantiate(prespawnedPrefab, new Vector3(point.position.x, point.position.y, 0),
                        Quaternion.identity, spawnParentTransform);
                    newGo.SetActive(true);
                }
            }

        }
    }

    Vector3[] GetMousePositionAndNormal()
    {
        Vector3[] returnData = new Vector3[] { Vector3.zero, Vector3.zero }; //0 = spawn poisiton, 1 = surface normal
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, 15))
        {
            returnData[0] = hit.point;
            returnData[1] = hit.normal;
        }

        return returnData;
    }
}
