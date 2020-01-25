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

    public float walkSpeed = 6;
    public float jumpStrength = 100f;
    public float maxVerticalRotation = 85f;
    public float sensitivity = 100f;
    public float gravity = 15f;
    public float airControl = 3f;
    public float groundControl = 10f;

    private float _walkSpeed;
    private float _vertSlopeSpeed;
    private bool _wasGrounded;
    private float _verticalAngle = 0;
    private Vector3 _velocity = new Vector3(0,0, 0);

    private CharacterController _controller;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        SetWalkSpeed(walkSpeed);
        
        SetActive(active);
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;
        // Camera controls
        LookHorizontal();
        LookVertical();

    }

    private void FixedUpdate()
    {
        if (!active) return;
        // Move player
        Move();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (Vector3.Angle(hit.normal, transform.up) <= _controller.slopeLimit)
        {
            return;
        }
        Vector3 hitVel = Vector3.Dot(_velocity, hit.normal) * hit.normal;
        _velocity -= hitVel;
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

        var walkVec = transform.TransformVector(PrintUtil.InputAxisTransform(horInput, vertInput));

        if (_controller.isGrounded) {
            DoGroundPhys(walkVec);
        }
        else
        {
            DoAirPhys(walkVec);
        }

        _wasGrounded = _controller.isGrounded;

        _controller.Move(_velocity * Time.fixedDeltaTime);
    }

    private void DoGroundPhys(Vector3 walkVec)
    {
        // _velocity = walkVec;
        _velocity.x = Mathf.Lerp(_velocity.x, walkVec.x * walkSpeed, groundControl * Time.fixedDeltaTime);
        _velocity.z = Mathf.Lerp(_velocity.z, walkVec.z * walkSpeed, groundControl  * Time.fixedDeltaTime);
        // make vel y based on the velocity + angle
        _velocity.y = -_vertSlopeSpeed;
        if (Input.GetButtonDown("Jump")) {
            _velocity.y = jumpStrength;
        }
    }

    private void DoAirPhys(Vector3 walkVec)
    {
        if (_wasGrounded)
        {
            _controller.Move(new Vector3(0, _vertSlopeSpeed * Time.fixedDeltaTime, 0));
            _velocity.y = Mathf.Max(0, _velocity.y);
        }
        _velocity.y -= gravity * Time.deltaTime;

        // how far we can pull velocity back to cancel it.
        float maxPullBack = Vector2.Dot(new Vector2( walkVec.x, walkVec.z), new Vector2(_velocity.x, _velocity.z));
        if (maxPullBack < 0)
        {
            Vector3 walkVecAlignment = Mathf.Clamp(airControl * Time.fixedDeltaTime,0, -maxPullBack) * walkVec;
            _velocity += walkVecAlignment;
        }
        else
        {
            // nudge player forward if holding direction
        }
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

    public void SetWalkSpeed(float value)
    {
        walkSpeed = value;
        _vertSlopeSpeed = value * Mathf.Tan(Mathf.Deg2Rad * _controller.slopeLimit);
        Debug.Log("Vert slope speed is " + _vertSlopeSpeed);
    }
    
}
