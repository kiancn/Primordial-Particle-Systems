using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraMotivity : MonoBehaviour
{
    [SerializeField] private KeyCode yPlusKey = KeyCode.W;
    [SerializeField] private KeyCode yMinusKey = KeyCode.S;
    [SerializeField] private KeyCode xPlusKey = KeyCode.D;
    [SerializeField] private KeyCode xMinusKey = KeyCode.A;

    [SerializeField] private float cameraMoveSpeedY = 3f;
    [SerializeField] private float cameraMoveSpeedX = 3f;

    [SerializeField] private Camera _camera;

    [SerializeField] private bool followCamera = true;
    [SerializeField] private float followSpeed = 3f;

    [SerializeField] private GameObject followObject;

    [SerializeField] private bool hardFollow = false;

    [SerializeField] private float cameraZoomMinimum = 0.5f;
    [SerializeField] private float cameraZoomMaximum = 10.5f;


    // Update is called once per frame
    void OnGUI()
    {
        if (Input.mouseScrollDelta != Vector2.zero)
        {
            bool zoomingIn = Input.mouseScrollDelta.y > 0;
            if (!zoomingIn && _camera.orthographicSize < cameraZoomMaximum)
            {
                // _camera.orthographicSize += .1f;
                _camera.orthographicSize = math.lerp(_camera.orthographicSize, cameraZoomMaximum, .016f);
            }
            else if (zoomingIn && _camera.orthographicSize > cameraZoomMinimum)
            {
                // _camera.orthographicSize -= .1f;
                _camera.orthographicSize = math.lerp(_camera.orthographicSize, cameraZoomMinimum, 0.016f);
            }

        }

        var current = Event.current;

        // checking 
        if (current.isKey)
        {
            // if not follow camera, move camera on defined key strokes only
            if (current.keyCode == yPlusKey)
            {
                _camera.transform.Translate(0f, cameraMoveSpeedY * Time.deltaTime, 0f);
            }

            if (current.keyCode == yMinusKey)
            {
                _camera.transform.Translate(0f, -cameraMoveSpeedY * Time.deltaTime, 0f);
            }

            if (current.keyCode == xPlusKey)
            {
                _camera.transform.Translate(cameraMoveSpeedX * Time.deltaTime, 0f, 0f);
            }

            if (current.keyCode == xMinusKey)
            {
                _camera.transform.Translate(-cameraMoveSpeedX * Time.deltaTime, 0f, 0f);
            }
        } // checking if object to follow is null (we know followCamera is true);
        else if (followObject && followObject != null)
        {
            var folPos = followObject.transform.position;
            if (hardFollow)
            {
                // doing a hard follow
                _camera.transform.SetPositionAndRotation(new Vector3(folPos.x, folPos.y, _camera.transform.position.z),
                    _camera.transform.rotation);
            }
            else
            {
                folPos.z = _camera.transform.position.z;

                Vector3 lerpedMove = Vector3.Lerp(_camera.transform.position, folPos, followSpeed * Time.deltaTime);
                _camera.transform.SetPositionAndRotation(lerpedMove, Quaternion.identity);
            }
        }
    }
}