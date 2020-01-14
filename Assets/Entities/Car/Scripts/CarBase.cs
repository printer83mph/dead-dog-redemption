using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBase : MonoBehaviour
{
    
    public Vector3 gravCenterOffset;
    public bool active;
    public PrintCam tPCamera;
    public PrintCam fPCamera;
    public float camDelay;
    
    private Wheel[] _wheels;
    private bool _thirdPerson;
    private bool _switchable = true;

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
        if (!active) return;
        
        foreach (Wheel wheel in _wheels)
        {
            wheel.accel = Input.GetAxis("Vertical");
            wheel.steer = Input.GetAxis("Horizontal");
        }

        if (Input.GetKeyDown("c") && _switchable)
        {
            DoCooldown();
            SetTP(!_thirdPerson);
        }
    }

    private void DoCooldown()
    {
        _switchable = false;
        Debug.Log("doing cam cooldown");
        Invoke(nameof(CoolOver), camDelay);
    }

    private void CoolOver()
    {
        Debug.Log("cam cooldown over");
        _switchable = true;
    }

    public void SetActive(bool isActive)
    {
        if (isActive)
        {
            active = true;
        }
        else
        {
            active = false;
            foreach (Wheel wheel in _wheels)
            {
                wheel.Stop();
            }
        }
        SetCamActive(isActive);
    }

    private void SetCamActive(bool isActive)
    {
        if (isActive)
        {
            fPCamera.SetEnabled(true);
            tPCamera.SetEnabled(true);
        
            if (_thirdPerson) fPCamera.SetEnabled(false);
            else tPCamera.SetEnabled(false);
        }
        else
        {
            fPCamera.SetEnabled(false);
            tPCamera.SetEnabled(false);
        }
    }

    private void SetTP(bool thirdPerson)
    {
        _thirdPerson = thirdPerson;
        
        // To do stuff in the right order
        
        if (thirdPerson) tPCamera.SetEnabled(true);
        fPCamera.SetEnabled(!thirdPerson);
        
        if (thirdPerson) return;
        
        tPCamera.SetEnabled(false);
    }
    
}
