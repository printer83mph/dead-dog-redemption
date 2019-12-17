using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveCar : MonoBehaviour
{
    public float enterExitCooldown = 1;
    public PlayerMovement player;
    public CarBase myCar;

    public float angleLimit = 70;
    public float distLimit = 3;

    private bool _interactable = true;
    private bool _inCar = false;
    private Camera _camera;
    private OrbitCam _carCam;

    void Start()
    {
        _camera = player.GetComponentInChildren<Camera>();
        _carCam = myCar.GetComponentInChildren<OrbitCam>();
    }
    
    void Update()
    {
        if (_interactable && Input.GetKeyDown("e"))
        {
            if (_inCar)
            {
                ExitCar();
            }
            else
            {
                Vector3 diffVector = myCar.transform.position - _camera.transform.position;
                Vector3 funnyVec = _camera.transform.rotation * Vector3.forward;
                bool enterable = Vector3.Angle(diffVector, funnyVec) < angleLimit && Vector3.SqrMagnitude(diffVector) < Mathf.Pow(distLimit, 2);
                if (enterable) EnterCar();
            }
        }
    }

    private void EnterCar()
    {
        myCar.control();
        player.gameObject.SetActive(false);
        _inCar = true;
        
        // activate camera
        _carCam.Control();
        
        DoCooldown();
    }

    private void ExitCar()
    {
        // Move into position
        player.gameObject.SetActive(true);
        myCar.unControl();
        
        // move player
        player.transform.position = myCar.transform.position + Vector3.up * 2;
        _inCar = false;
        
        // deactivate camera
        _carCam.Uncontrol();

        DoCooldown();
    }

    private void DoCooldown()
    {
        _interactable = false;
        Debug.Log("doing cooldown");
        Invoke(nameof(CoolOver), enterExitCooldown);
    }

    private void CoolOver()
    {
        Debug.Log("cooldown over");
        _interactable = true;
    }

}