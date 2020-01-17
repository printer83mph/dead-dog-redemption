using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{

    public float torque = 8;
    public float staticFrictionFactor = 4;
    public float dynamicFrictionFactor = 4;
    public float slidePower = .2f;
    public float staticFrictionBreakpoint = 6;
    public float damping = .5f;
    public float brakeFactor = 1;
    
    public float suspensionHeight = .5f;
    public float suspensionStrength = 10;
    public float turnAmt = 25;
    public float steerGrav = 5;

    public float radius;
    
    // FOR CONTROLLING
    public float accel;
    public float steer;
    public float brake;

    private GameObject _carObject;
    private Rigidbody _carRigidbody;
    private Vector3 _wheelMeshPos;
    private Transform _wheelMeshTransform;

    private float _lastHeight;
    private float _grip;
    private float _velocity;
    private float _steerInterp;

    // Start is called before the first frame update
    void Start()
    {
        _carObject = transform.parent.gameObject;
        _carRigidbody = _carObject.GetComponent<Rigidbody>();
        _wheelMeshTransform = transform.GetChild(0);
        _wheelMeshPos = _wheelMeshTransform.localPosition;
        _lastHeight = suspensionHeight;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // GET GRIP
        
        RaycastHit hit;
        Ray downRay = new Ray(transform.position, -transform.up);

        _grip = 0;
        
        // Steer wheel TODO: improve this
        if (steer != _steerInterp)
        {
            float speedCalc = Mathf.Pow(Mathf.Abs(_carRigidbody.velocity.z), .2f);
            _steerInterp = PrintUtil.LinearInterp(_steerInterp, steer / Mathf.Max(speedCalc, 1), steerGrav * Time.fixedDeltaTime / speedCalc);
            transform.localRotation = Quaternion.Euler(0, _steerInterp * turnAmt, 0);
        }

        // Spin the wheel mesh and distance if no ground
        _wheelMeshTransform.Rotate(_velocity * Time.deltaTime, 0, 0);

        if (Physics.Raycast(downRay, out hit))
        {
            float wheelFall = Mathf.Lerp(_lastHeight, suspensionHeight, Time.fixedDeltaTime * 3);
            
            if (hit.distance < wheelFall)
            {
                Vector3 contactPoint = transform.position - hit.distance * transform.up;
                Vector3 localVelocity = LocalVelocity();
                
                DoGroundPhysics(localVelocity, hit);
                UpdateWheelMesh(localVelocity, hit.distance);

            }

        }

        _lastHeight = suspensionHeight;

        _wheelMeshTransform.localPosition = Vector3.Lerp(_wheelMeshTransform.localPosition, _wheelMeshPos - suspensionHeight * Vector3.up, Time.fixedDeltaTime * 3);

        _velocity = Mathf.Lerp(_velocity, 0, Time.deltaTime * .2f);
        // RUN THIS STUFF IF RAYCAST DOESN'T HIT

    }

    private Vector3 LocalVelocity() =>
        transform.InverseTransformVector(_carRigidbody.GetPointVelocity(transform.position));

    private void DoGroundPhysics(Vector3 localVelocity, RaycastHit hit)
    {

        // Get grip
        _grip = Mathf.Pow(1 - (hit.distance / suspensionHeight), 2);

        // Calculate damping counterforce
        Vector3 counterForce = - localVelocity.y * _carRigidbody.mass * damping * hit.normal;
                
        // Do normal force
        _carRigidbody.AddForceAtPosition(_grip * _carRigidbody.mass * suspensionStrength * hit.normal + counterForce, transform.position);
        
        // Do side force
        float frictionAmt = - localVelocity.x;

        float compNum = Math.Abs(frictionAmt) / staticFrictionBreakpoint;

        float realForce;
        if (compNum < 1)
        {
            // Linear accel
            realForce = Mathf.Sign(frictionAmt) * compNum * staticFrictionFactor;
        }
        else
        {
            // Exponential accel
            realForce = Mathf.Pow(Math.Abs(frictionAmt), slidePower) * Mathf.Sign(frictionAmt) * dynamicFrictionFactor;
        }
        
        Vector3 brakeForce = new Vector3(localVelocity.x, 0, localVelocity.z) * - brakeFactor;

        brakeForce /= Mathf.Pow(brakeForce.magnitude, .5f);

        // if (Mathf.Abs(realForce) > Mathf.Abs(maxForce)) realForce = maxForce;
        
        Vector3 bigLerp = Vector3.Lerp(realForce * transform.right, transform.TransformVector(brakeForce), brake);
        _carRigidbody.AddForceAtPosition( bigLerp, transform.position, ForceMode.Acceleration);

        // TODO: CHECK IF THE WHEEL IS TOUCHING ANOTHER DYNAMIC OBJECT
        // TODO: integrate braking into forward/backward force
        // Forward force
        Vector3 forwardForce = (1 - Mathf.Pow(1 - _grip, 3)) * accel * torque * transform.forward;
        _carRigidbody.AddForceAtPosition(Vector3.Lerp(forwardForce, Vector3.zero, brake), transform.position, ForceMode.Acceleration);

    }

    private void UpdateWheelMesh( Vector3 localVelocity, float hitDistance )
    {
        // MOVE WHEEL INTO PLACE + SPIN
        _wheelMeshTransform.localPosition = _wheelMeshPos - hitDistance * Vector3.up;
                
        // Visual wheel rotation

        _velocity = localVelocity.z * radius * 200 * Mathf.PI;

        _lastHeight = hitDistance;

        return;
    }

    public void Stop()
    {
        accel = 0;
        steer = 0;
        brake = 1f;
    }

}
