using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{

    public float torque = 40;
    public float friction = 1000;
    public float damping = 20;

    public float suspensionHeight = 1;
    public float suspensionStrength = 100;
    public float turnAmt = 20;
    public float steerGrav = 20;

    public float radius;
    
    // FOR CONTROLLING
    public float accel;
    public float steer;
    public float brake;

    private GameObject _carObject;
    private Rigidbody _carRigidbody;
    private Vector3 _wheelObjPos;
    private Transform _wheelMeshTransform;

    private float _grip;
    private float _velocity;
    private float _steerInterp;

    // Start is called before the first frame update
    void Start()
    {
        _carObject = transform.parent.gameObject;
        _carRigidbody = _carObject.GetComponent<Rigidbody>();
        _wheelMeshTransform = transform.GetChild(0);
        _wheelObjPos = _wheelMeshTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

        // GET GRIP
        
        RaycastHit hit;
        Ray downRay = new Ray(transform.position, -transform.up);

        _grip = 0;
        
        // Steer wheel
        _steerInterp = Mathf.Lerp(_steerInterp, steer, steerGrav * Time.deltaTime / Mathf.Sqrt(_carRigidbody.velocity.sqrMagnitude + 1));
        transform.localRotation = Quaternion.Euler(0, _steerInterp * turnAmt, 0);
        
        // Spin the wheel mesh and distance if no ground
        _wheelMeshTransform.Rotate(_velocity * Time.deltaTime, 0, 0);
        _wheelMeshTransform.localPosition = _wheelObjPos - suspensionHeight * Vector3.up;

        if (Physics.Raycast(downRay, out hit))
        {
            if (hit.distance < suspensionHeight)
            {
                Vector3 contactPoint = transform.position - hit.distance * transform.up;
                Vector3 carVelAtWheel = _carRigidbody.GetPointVelocity(contactPoint);
                
                // MOVE WHEEL INTO PLACE + SPIN
                _wheelMeshTransform.localPosition = _wheelObjPos - hit.distance * Vector3.up;

                DoPhysics(hit, carVelAtWheel);

                // Visual wheel rotation

                _velocity = transform.InverseTransformVector(carVelAtWheel).z * radius * 200 * Mathf.PI;

                return;

            }

            return;

        }

        // RUN THIS STUFF IF RAYCAST DOESN'T HIT

    }

    private void DoPhysics(RaycastHit hit, Vector3 carVelAtWheel)
    {
        
        // FIX DELTA TIME STUFF

        // Get grip
        _grip = Mathf.Pow(1 - (hit.distance / suspensionHeight), 2);

        // Calculate damping counterforce
        Vector3 counterForce = Vector3.Dot(carVelAtWheel, transform.up) * _carRigidbody.mass * -damping * hit.normal * Time.deltaTime;
                
        // Do normal force
        _carRigidbody.AddForceAtPosition(_grip * suspensionStrength * hit.normal + counterForce, transform.position);
        
        // Do side force TODO: use brake to interp between this and just pure braking
        float sideForce = Vector3.Dot(transform.right, carVelAtWheel);
        _carRigidbody.AddForceAtPosition(- sideForce/Mathf.Pow(Mathf.Abs(sideForce), .1f) * friction * transform.right, transform.position);

        // TODO: CHECK IF THE WHEEL IS TOUCHING ANOTHER DYNAMIC OBJECT
        // Forward force
        _carRigidbody.AddForceAtPosition((1 - Mathf.Pow(1 - _grip, 3)) * accel * torque * transform.forward * Time.deltaTime, transform.position);
    }

    public void Stop()
    {
        accel = 0;
        steer = 0;
        brake = 0;
    }

}
