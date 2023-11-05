using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObject : MonoBehaviour
{

    void Awake()
    {   
        Camera cam = Camera.main;
        Vector3 dirToLook = -cam.transform.forward;
        this.transform.up = dirToLook;
        this.transform.forward = -cam.transform.up;
    }
}
