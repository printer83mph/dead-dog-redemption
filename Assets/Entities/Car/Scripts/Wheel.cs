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

    private GameObject _carObject;
    private Rigidbody _carRigidbody;
    private Vector3 _wheelObjPos;
    private Transform _wheelMeshTransform;

    private float _grip;
    private float _velocity;
    private float _accel;
    private float _steer;

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
        
        // ROTATE WHEEL
        transform.localRotation = Quaternion.Euler(0, _steer * turnAmt, 0);
        
        // DISTANCE WHEEL IF NO GROUND
        _wheelMeshTransform.localPosition = _wheelObjPos - suspensionHeight * Vector3.up;

        if (Physics.Raycast(downRay, out hit))
        {
            if (hit.distance < suspensionHeight)
            {
                
                Vector3 contactPoint = transform.position - hit.distance * transform.up;

                // MOVE WHEEL INTO PLACE + SPIN
                _wheelMeshTransform.localPosition = _wheelObjPos - hit.distance * Vector3.up;
                _wheelMeshTransform.Rotate(_velocity * Time.deltaTime, 0, 0);

                // SET GRIP
                _grip = Mathf.Pow(1 - (hit.distance / suspensionHeight), 2);

                // ADD DAMPING TO CAR BASED ON WHEEL GRIPS
                Vector3 carVelAtWheel = _carRigidbody.GetPointVelocity(contactPoint);
                Vector3 counterForce = Vector3.Dot(carVelAtWheel, transform.up) * _carRigidbody.mass * -damping * Time.deltaTime * hit.normal;
                
                // ADD UPWARD TO CAR
                _carRigidbody.AddForceAtPosition(_grip * suspensionStrength * Time.deltaTime * hit.normal + counterForce,
                    transform.position);
                
                // ADD SIDE FORCE
                float sideForce = Vector3.Dot(transform.right, carVelAtWheel);

                // PUSH CAR IN RIGHT DIRECTION - bugged???
                _carRigidbody.AddForceAtPosition(- sideForce/Mathf.Pow(Mathf.Abs(sideForce), .5f) * friction * Time.deltaTime * transform.right, transform.position);

                // ADD CHUGGA CHUGGA
                // if (powered) _velocity += Input.GetAxis("Vertical") * 10;
                //
                // ADD GROUND FORCE TO WHEEL
                // TODO: CHECK IF THE WHEEL IS TOUCHING ANOTHER OBJECT
                // Vector3 wheelVel = _velocity * transform.forward;
                // _velocity = Mathf.Lerp(_velocity, Vector3.Dot(_carRigidbody.velocity, transform.forward), friction);
                // _carRigidbody.AddForceAtPosition(_grip * friction * _velocity * transform.forward, transform.position);

                // ADD FORWARD FORCE TO CAR
                _carRigidbody.AddForceAtPosition(_grip * Time.deltaTime * _accel * torque * transform.forward, transform.position);
                
                // TODO: add braking and wheel velocity
            }
        }

    }

    public void SetControl(float accel, float steer)
    {
        _accel = accel;
        _steer = steer;
    }

}
