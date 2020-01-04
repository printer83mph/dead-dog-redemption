using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PrintCam : MonoBehaviour
{

    private Camera _camera;
    
    public void Start()
    {
        _camera = GetComponent<Camera>();
    }

    public void SetEnabled(bool isEnabled)
    {
        if (isEnabled) OnEnable();
        else OnDisable();
        
        _camera.enabled = isEnabled;
    }

    public virtual void OnEnable() {}

    public virtual void OnDisable() {}

}
