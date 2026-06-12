using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class text2camera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 始终面向摄像机
        // transform.LookAt(Camera.main.transform.position);
        transform.forward = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z);
    }
}
