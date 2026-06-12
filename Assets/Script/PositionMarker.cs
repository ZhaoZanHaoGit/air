using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionMarker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnClicked()
    {
        var cam = Camera.main.transform;
        var y = transform.position.y;
        if (y > 100.0f)
        {
            y -= 200.0f;
        }
        cam.position = new Vector3(transform.position.x, y + 13.0f, transform.position.z);
        cam.rotation = transform.rotation;
        Camera.main.orthographic = false;
    }
}
