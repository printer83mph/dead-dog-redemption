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
        
        // disable player
        player.transform.SetParent(myCar.transform);
        player.transform.localPosition = Vector3.zero;
        
        myCar.SetActive(true);
        player.SetActive(false);
        
        _inCar = true;
        
        DoCooldown();
    }

    private void ExitCar()
    {

        // move player
        player.transform.parent = null;
        player.transform.position = myCar.transform.position + Vector3.up * 2;
        player.transform.rotation = Quaternion.Euler(0, myCar.myCamera.transform.eulerAngles.y, 0);
        Debug.Log("Player moved");
        
        player.SetActive(true);
        myCar.SetActive(false);
        
        _inCar = false;

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