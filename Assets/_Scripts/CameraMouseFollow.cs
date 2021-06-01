using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouseFollow : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private int stepsBetween = 100;
    [SerializeField] private int fixedUpdatesBetweenPossibleMoves = 60;

    private int currentStep;
    private Vector3 cameraIdealPosition;


    // Start is called before the first frame update
    void Start()
    {
        _camera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentStep++;

        if (currentStep >= stepsBetween)
        {
            Vector3 basePos = GetMousePosition();
            
            cameraIdealPosition = new Vector3(basePos.x,basePos.y,_camera.transform.position.z);
            currentStep = 0;
        }

        if (cameraIdealPosition != _camera.transform.position)
        {
            var curPos = _camera.transform.position;
            _camera.transform.position = Vector3.Lerp(curPos, cameraIdealPosition, Time.deltaTime);
        }
    }

    Vector3 GetMousePosition()
    {
        Vector3 returnData = new Vector3();
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, 15))
        {
            returnData = new Vector3(hit.point.x, hit.point.y, 0);
        }

        return returnData;
    }
}