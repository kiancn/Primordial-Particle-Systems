using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player2DMoverTopdown : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    [SerializeField] private Rigidbody2D body;
    
    [SerializeField] private GameObject _playerObject;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed; // angle/sencond

    [SerializeField] private KeyCode keyRotationLeft = KeyCode.LeftArrow;
    [SerializeField] private KeyCode keyRotationRight = KeyCode.RightArrow;
    [SerializeField] private KeyCode keyMoveForward = KeyCode.UpArrow;
    [SerializeField] private KeyCode keyMoveBackward = KeyCode.DownArrow;

    [SerializeField] private KeyCode boosterButton = KeyCode.Space;

    [SerializeField] private float movementSpeedIncrementStep = 0.016f;
    [SerializeField] private float movementSpeedDecrementStep = 0.064f;

    [field: SerializeField] public float CurrentMoveSpeed { get; private set; } = 0;

    [SerializeField] private bool movingImpulseNow = false;
   
    // Start is called before the first frame update
    void Start()
    {
        if (_playerObject == null || _playerTransform == null)
        {
            Debug.LogWarning("You forgot to assign a player object to control. Exceptions will explode.");
            
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        float deltaPosition = 0;
        float deltaTime = Time.deltaTime;

        var mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);

     
            var lookDir = new Vector2(mousePos.x, mousePos.y) - body.position;

            var angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            body.rotation = angle;
      

        if (Input.GetKey(keyMoveForward))
        {
            deltaPosition = 1 ;
            movingImpulseNow = true;
        }
        else if (Input.GetKey(keyMoveBackward))
        {
            deltaPosition = -1 ;
            movingImpulseNow = true;
        }
        else
        {
            movingImpulseNow = false;
        }


        if (movingImpulseNow)
        {
            CurrentMoveSpeed = Mathf.Clamp(CurrentMoveSpeed + movementSpeedIncrementStep*deltaPosition,
                -moveSpeed, moveSpeed);
        }
        else
        {
            if (CurrentMoveSpeed > 0)
            {
                CurrentMoveSpeed = Mathf.Clamp(CurrentMoveSpeed -  movementSpeedDecrementStep, 
                    -moveSpeed, moveSpeed);
            } else if (CurrentMoveSpeed < 0)
            {
                CurrentMoveSpeed = Mathf.Clamp(CurrentMoveSpeed + movementSpeedDecrementStep,
                    -moveSpeed, moveSpeed);
            }
        }

        // _playerTransform.Rotate(0,0,deltaRotation);
        body.MovePosition(_playerTransform.position + _playerTransform.right * (CurrentMoveSpeed * deltaTime));
  
        // _playerTransform.Translate(_playerTransform.right*deltaTime);
      
    }
}