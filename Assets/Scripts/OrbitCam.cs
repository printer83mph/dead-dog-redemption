﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCam : PrintCam
{
    
    public Transform target;

    public float sensitivity = 100;
    public float maxXAngle = 85;

    public bool chase;

    public Vector3 offset;
    public float distance = 5;

    private float _yRot = 0;
    private float _xRot = 0;

    void Update()
    {

        if (enabled)
        {
            _yRot += sensitivity * Time.deltaTime * Input.GetAxis("Mouse X");
            _yRot %= 360;

            _xRot -= sensitivity * Time.deltaTime * Input.GetAxis("Mouse Y");
            _xRot = Mathf.Clamp(_xRot, -maxXAngle, maxXAngle);

            transform.rotation = Quaternion.Euler(_xRot, _yRot + (chase ? target.transform.eulerAngles.y : 0), 0);
            transform.position = target.transform.position + offset - distance * transform.forward;
        }
        
    }

    public override void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _xRot = 0;
        _yRot = target.rotation.eulerAngles.y;
    }

    public override void OnDisable()
    {
        return;
    }
    
}
