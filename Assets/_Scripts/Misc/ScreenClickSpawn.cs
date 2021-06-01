using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenClickSpawn : MonoBehaviour
{
   [SerializeField] private Camera _camera;
   [SerializeField] private GameObject prefab;
   [SerializeField] private GameObject prespawnedPrefab;
   [SerializeField] private KeyCode assistKey = KeyCode.LeftShift;
   
    // Start is called before the first frame update
    void Start()
    {
        _camera = FindObjectOfType<Camera>();
        prespawnedPrefab = Instantiate(prefab);
        prespawnedPrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(assistKey) && Input.GetMouseButtonDown(0))
        {
          Vector3[] pn =  GetMousePositionAndNormal();
          // Debug.Log($"Position {pn[0]}");
         var go = Instantiate(prespawnedPrefab, new Vector3(pn[0].x, pn[0].y, 0), Quaternion.identity);
         go.SetActive(true);
        }    
    }


    /* Calculates */
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
