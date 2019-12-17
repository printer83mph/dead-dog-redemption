using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController), typeof(PlayerMovement))]
public class WalkAnimationController : MonoBehaviour
{

    private CharacterController _controller;
    private PlayerMovement _playerMovement;
    
    public Camera theCamera;

    public float bobbingThreshold = .1f;

    private Vector3 _speedInterp = new Vector3(0,0,0);
    private float _jumpInterp = 1;
    private float _defaultWalkSpeed;

    public Vector3 camOffset = new Vector3(0,.8f,0);

    public float bobbingSpeed = 0.18f;
    public float bobbingHeight = 0.2f;
    public float bobbingWidth = 0.2f;

    private float distTraveled = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _playerMovement = GetComponent<PlayerMovement>();
        _defaultWalkSpeed = _playerMovement.walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHeadBob();
    }

    void UpdateHeadBob()
    {
        // Get actual local velocity
        Vector3 localVel = transform.InverseTransformDirection(_controller.velocity);
        float curSpeed = (new Vector2(localVel.x, localVel.z)).magnitude / _defaultWalkSpeed;
        
        // Interpolate local velocity
        _speedInterp = Vector3.Lerp(_speedInterp, localVel / _defaultWalkSpeed, Time.deltaTime * 4);
        float speedInterpMag = _speedInterp.magnitude;
        
        // Interpolate being grounded or not
        _jumpInterp = Mathf.Lerp(_jumpInterp, _controller.isGrounded ? 1 : 0, Time.deltaTime * 4);
        
        // Reset dist traveled if speed below threshold
        if (speedInterpMag < bobbingThreshold)
        {
            distTraveled = 0.0f;
        }
        else
        {
            distTraveled += Time.deltaTime * curSpeed;
        }

        // Offsets
        float offsetX = (float)Math.Sin(distTraveled * bobbingSpeed) * bobbingWidth * _speedInterp.z * _jumpInterp;
        float offsetY = (float)Math.Abs( Math.Cos(distTraveled * bobbingSpeed) ) * 2 * bobbingHeight * speedInterpMag * _jumpInterp;
        Vector3 realOffset = new Vector3(offsetX, offsetY, 0 ) + camOffset;

        // cam.transform.localPosition = Vector3.forward * new Vector3((float)offsetX, 0, (float)offsetZ);
        theCamera.transform.localPosition = realOffset;
    }

    float GetSpeed()
    {
        // Should be 1 at base walk speed
        return (new Vector2(_controller.velocity.x, _controller.velocity.z)).magnitude / _defaultWalkSpeed;
    }
}
