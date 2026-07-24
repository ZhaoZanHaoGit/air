using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CongitionCam_PC : simpleMouseLook
{
    public GameObject CameraTarget;
    private new void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        init(CameraTarget);
        enabled = true;
    }
}
