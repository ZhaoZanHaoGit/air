using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MinimapCamera : MonoBehaviour
{
    public Camera mainC;
    private float hightValue;
    void Start()
    {
        hightValue = transform.position.y;

        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(mainC.transform.position.x, hightValue, mainC.transform.position.z);
        //Camera.main.transform.rotation.y
        transform.rotation = Quaternion.Euler(90, Camera.main.transform.eulerAngles.y, 0);
    }
}
