using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBase : MonoBehaviour
{
    
    public Vector3 gravCenterOffset;
    public bool active;
    public PrintCam myCamera;
    
    private Wheel[] _wheels;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = gravCenterOffset;
        _wheels = GetComponentsInChildren<Wheel> ();

        SetActive(active);
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            foreach (Wheel wheel in _wheels)
            {
                wheel.SetControl(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
            }
        }
    }

    public void SetActive(bool isActive)
    {
        if (isActive)
        {
            active = true;
            myCamera.SetEnabled(true);
        }
        else
        {
            active = false;
            foreach (Wheel wheel in _wheels)
            {
                wheel.SetControl(0, 0);
            }

            myCamera.SetEnabled(false);
        }
    }
    
}
