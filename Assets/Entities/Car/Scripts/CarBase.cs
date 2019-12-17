using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBase : MonoBehaviour
{
    
    public Vector3 gravCenterOffset;
    public bool startActive;
    
    private Wheel[] _wheels;

    private bool _controlled = false;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = gravCenterOffset;
        _wheels = GetComponentsInChildren<Wheel> ();

        if (startActive)
        {
            control();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_controlled)
        {
            foreach (Wheel wheel in _wheels)
            {
                wheel.SetControl(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
            }
        }
    }

    public void control()
    {
        _controlled = true;
    }

    public void unControl()
    {
        _controlled = false;
        foreach (Wheel wheel in _wheels)
        {
            wheel.SetControl(0, 0);
        }
    }
}
