using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse2D : MonoBehaviour
{
    [SerializeField] private GameObject movedObject;
    [SerializeField] private Camera camera;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        movedObject.transform.position = mousePos;
    }
}