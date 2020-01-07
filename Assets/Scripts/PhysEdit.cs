using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysEdit : MonoBehaviour
{
    public bool matchFramerate;
    public float frameRate = 60;
    
    // Start is called before the first frame update
    void Start()
    {
        if (matchFramerate) Time.fixedDeltaTime = 1.0f / Screen.currentResolution.refreshRate;
        else Time.fixedDeltaTime = 1f / frameRate;
    }

}
