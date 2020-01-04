using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    public Camera fpCam;

    public bool active = true;
    
    public float walkSpeed = 6f;
    public float strafeSpeed = 6f;
    public float jumpStrength = 100f;
    public float maxVerticalRotation = 85f;
    public float sensitivity = 100f;
    public float gravity = 15f;
    public float airControl = 3f;
    public float groundControl = 10f;

    private float _verticalAngle = 0;
    private Vector3 _velocity = new Vector3(0,0, 0);

    private CharacterController _controller;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        
        SetActive(active);
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            // Camera controls
            LookHorizontal();
            LookVertical();

            // Move player
            Move();
        }
        
    }

    void LookHorizontal()
    {
        float hor = Input.GetAxis("Mouse X");

        transform.Rotate(0, hor * sensitivity * Time.deltaTime, 0);
    }

    void LookVertical()
    {
        _verticalAngle -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        _verticalAngle = Mathf.Clamp(_verticalAngle, -maxVerticalRotation, maxVerticalRotation);
        fpCam.transform.localRotation = Quaternion.Euler(_verticalAngle, 0, 0);
    }

    void Move()
    {
        // Get inputs
        var vertInput = Input.GetAxis("Vertical");
        var horInput = Input.GetAxis("Horizontal");

        var walkVec = transform.TransformVector(PrintUtil.InputAxisTransform(horInput, vertInput) * walkSpeed);

        if (_controller.isGrounded) {
            // _velocity = walkVec;
            _velocity.x = Mathf.Lerp(_velocity.x, walkVec.x, groundControl * Time.deltaTime);
            _velocity.z = Mathf.Lerp(_velocity.z, walkVec.z, groundControl * Time.deltaTime);
            _velocity.y = -1f;
            if (Input.GetButtonDown("Jump")) {
                _velocity.y = jumpStrength;
            }
        }
        else
        {
            _velocity.y -= gravity * Time.deltaTime;
            _velocity.x = Mathf.Lerp(_velocity.x, walkVec.x, airControl * Time.deltaTime);
            _velocity.z = Mathf.Lerp(_velocity.z, walkVec.z, airControl * Time.deltaTime);
        }


        _controller.Move(_velocity * Time.deltaTime);
    }

    public void SetActive(bool isActive)
    {
        SetCollision(isActive);
        
        if (isActive)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            active = true;
        }
        else
        {
            active = false;
        }
    }

    private void SetCollision(bool collide)
    {

        _controller.enabled = collide;

        Collider[] cols = GetComponents<Collider>();

        foreach (Collider col in cols) col.enabled = collide;
    }
    
}
