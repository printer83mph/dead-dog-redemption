using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCam : MonoBehaviour
{
    
    public Transform target;

    public float sensitivity = 100;
    public float maxXAngle = 85;

    public bool controlled;

    public Vector3 offset;
    public float distance = 5;

    private float _yRot = 0;
    private float _xRot = 0;

    void Update()
    {

        if (controlled)
        {
            _yRot += sensitivity * Time.deltaTime * Input.GetAxis("Mouse X");
            _yRot %= 360;

            _xRot -= sensitivity * Time.deltaTime * Input.GetAxis("Mouse Y");
            _xRot = Mathf.Clamp(_xRot, -maxXAngle, maxXAngle);

            transform.rotation = Quaternion.Euler(_xRot, _yRot + target.transform.eulerAngles.y, 0);
            transform.position = target.transform.position + offset - distance * transform.forward;
        }
        
    }

    public void Control()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _xRot = 0;
        _yRot = 0;
        enabled = true;
        controlled = true;
    }
    
    public void Uncontrol()
    {
        enabled = false;
        controlled = false;
    }
}
