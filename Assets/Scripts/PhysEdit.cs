using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysEdit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Time.fixedDeltaTime = 1.0f / Screen.currentResolution.refreshRate;
        // Time.fixedDeltaTime = .1f;
    }

}
