using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCarCam : PrintCam
{

    public float sensitivity;
    public Rigidbody carRigidbody;
    public float bodyRadius;
    public float sideLimit;
    public float forwardLimit;
    public float backLimit;
    public float bounce;
    public Vector2 gravity;
    public Vector2 accelScale;

    private Vector2 _pos = new Vector2(0, 0);
    private Vector2 _vel = new Vector2(0, 0);
    private Vector3 _startPos;
    private Vector3 _lastVel;
    private float _rotY;

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

        _vel += new Vector2(accel.x * accelScale.x, accel.z * accelScale.y);
        
        _vel -= Time.deltaTime * Vector2.Scale(_pos, gravity);

        _vel *= .9f;

        _pos += _vel * Time.deltaTime;

        ClampMovement();
        
        // Set camera transform based on funny stuff

        transform.localRotation = Quaternion.Euler(- _pos.y, 0, _pos.x);
        transform.localPosition = _startPos;
        transform.Translate(Vector3.up * bodyRadius);
        
        // Mouse movement
        _rotY += Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
        _rotY %= 360;
        
        transform.Rotate(0, _rotY, 0);
    }

    void ClampMovement()
    {
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
        _rotY = 0;
        // TODO: reset stuff
    }

    public override void OnDisable()
    {
        return;
    }
    
}
