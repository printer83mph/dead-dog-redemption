using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCarCam : PrintCam
{

    public float sensitivity = 100;
    public float maxXRotation = 180;
    public float minXRotation = -180;
    public float turnMoveScale = .001f;
    
    public Rigidbody carRigidbody;
    public float bodyRadius = .5f;
    public float sideLimit = 15;
    public float forwardLimit = 20;
    public float backLimit = 5;
    public float bounce = .2f;
    public Vector2 gravity = new Vector2(10, 10);
    public Vector2 accelScale = new Vector2(10,10);

    private Vector2 _pos = new Vector2(0, 0);
    private Vector2 _vel = new Vector2(0, 0);
    private Vector3 _startPos;
    private Vector3 _lastVel;
    private float _yRot;

    public override void OnStart()
    {
        _startPos = transform.localPosition;
        _lastVel = carRigidbody.velocity;
    }
    
    void Update()
    {
        if (!enabled) return;
        
        Vector3 newVel = carRigidbody.transform.InverseTransformVector(carRigidbody.GetPointVelocity(transform.position));
        Vector3 accel = (newVel - _lastVel);
        _lastVel = newVel;
        
        // Debug.Log(accel);

        _vel += new Vector2(- accel.x * accelScale.x, - accel.z * accelScale.y);
        
        _vel -= Time.deltaTime * Vector2.Scale(_pos, gravity);

        _vel *= .9f;

        _pos += _vel * Time.deltaTime;

        ClampMovement();
        
        // Set camera transform based on funny stuff

        transform.localRotation = Quaternion.Euler(_pos.y, 0, _pos.x);
        transform.localPosition = _startPos;
        transform.Translate(Vector3.up * bodyRadius);

        DoMouseControl();
    }

    private void DoMouseControl()
    {
        // Mouse movement
        _yRot += Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
        _yRot %= 360;

        _yRot = Mathf.Clamp(_yRot, minXRotation, maxXRotation);
        
        transform.Translate(_yRot * turnMoveScale, 0, 0);
        
        transform.Rotate(0, _yRot, 0);
    }

    void ClampMovement()
    {
        // Limit rotation + do bounce
        if (Mathf.Abs(_pos.x) > sideLimit)
        {
            _pos.x = Mathf.Sign(_pos.x) * sideLimit;
            _vel.x *= bounce;
        }

        if (_pos.y > forwardLimit)
        {
            _pos.y = forwardLimit;
            _vel.y *= bounce;
        }
        else if (_pos.y < - backLimit)

        {
            _pos.y = - backLimit;
            _vel.y *= bounce;
        }
    }
    
    public override void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _yRot = 0;
        // TODO: reset stuff
    }

    public override void OnDisable()
    {
        return;
    }
    
}
