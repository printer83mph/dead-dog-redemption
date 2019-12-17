using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    public Camera fpCam;
    
    public float walkSpeed = 6f;
    public float strafeSpeed = 6f;
    public float jumpStrength = 100f;
    public float maxVerticalRotation = 85f;
    public float rotateSpeed = 2f;
    public float focusRotateSpeed = 0.5f;
    public float gravity = 15f;
    public float airControl = 1f;

    private float _verticalAngle = 0;
    private float _verticalSpeed = 0;
    private Vector3 _movement = new Vector3(0,0,0);

    private CharacterController _controller;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Camera controls
        LookHorizontal();
        LookVertical();

        // Move player
        Move();
    }

    void LookHorizontal()
    {
        float angle = Input.GetAxis("Mouse X");
        angle *= (Input.GetKey(KeyCode.LeftControl)) ? focusRotateSpeed : rotateSpeed;

        transform.Rotate(0, angle, 0);
    }

    void LookVertical()
    {
        _verticalAngle -= Input.GetAxis("Mouse Y") * ((Input.GetKey(KeyCode.LeftControl)) ? focusRotateSpeed : rotateSpeed);
        _verticalAngle = Mathf.Clamp(_verticalAngle, -maxVerticalRotation, maxVerticalRotation);
        fpCam.transform.localRotation = Quaternion.Euler(_verticalAngle, 0, 0);
    }

    void Move()
    {
        var vertInput = Input.GetAxis("Vertical");

        var horInput = Input.GetAxis("Horizontal");
        var walkVec = new Vector3(horInput * (float)Math.Sqrt(1 - Math.Pow(vertInput, 2) / 2) * strafeSpeed, 0, vertInput * (float)Math.Sqrt(1 - Math.Pow(horInput, 2) / 2) * walkSpeed);

        if (_controller.isGrounded) {
             _verticalSpeed = -1;
             if (Input.GetButtonDown("Jump")) {
                 _verticalSpeed = jumpStrength;
             }
        }
        else
        {
            walkVec = Vector3.Lerp(_movement, walkVec, airControl * Time.deltaTime);
        }
        
//        _movement = Vector3.Lerp(_movement, walkVec, Time.deltaTime * 10);
        _movement = walkVec;

        _verticalSpeed -= gravity * Time.deltaTime;
        _movement.y = _verticalSpeed;

        _controller.Move(transform.rotation * _movement * Time.deltaTime);
    }
}
