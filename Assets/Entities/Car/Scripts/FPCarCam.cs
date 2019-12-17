using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCarCam : MonoBehaviour
{

    public Rigidbody carRigidbody;
    public float bodyRadius;
    public float sideLimit;
    public float forwardLimit;
    public float backLimit;
    public Vector2 gravity;
    public Vector2 accelScale;

    private Vector2 _pos = new Vector2(0, 0);
    private Vector2 _vel = new Vector2(0, 0);
    private Vector3 _startPos;
    private Vector3 _lastVel;

    void Start()
    {
        _startPos = transform.localPosition;
        _lastVel = carRigidbody.velocity;
    }
    
    void Update()
    {
        Vector3 newVel = carRigidbody.transform.InverseTransformVector(carRigidbody.GetPointVelocity(transform.position));
        newVel.Scale(accelScale);
        Vector3 accel = (newVel - _lastVel);
        _lastVel = newVel;

        
        Debug.Log(accel);

        _vel += new Vector2(accel.x, accel.z);
        
        _vel -= Time.deltaTime * Vector2.Scale(_pos, gravity);

        _vel *= .9f;

        _pos += _vel * Time.deltaTime;

        ClampMovement();
        
        // Set camera transform based on funny stuff

        transform.localRotation = Quaternion.Euler(_pos.y, _pos.x, 0);
        transform.localPosition = _startPos;
        transform.Translate(transform.up * bodyRadius);
    }

    void ClampMovement()
    {
        if (Mathf.Abs(_pos.x) > sideLimit)
        {
            _pos.x = Mathf.Sign(sideLimit) * sideLimit;
            _vel.x *= -.7f;
        }

        if (_pos.y > forwardLimit)
        {
            _pos.y = forwardLimit;
            _vel.y *= -.7f;
        }
        else if (_pos.y < - backLimit)

        {
            _pos.y = forwardLimit;
            _vel.y *= -.7f;
        }
    }
    
}
