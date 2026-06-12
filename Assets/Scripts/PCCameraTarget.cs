using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCCameraTarget : MonoBehaviour
{
    private Vector3 startPosition;

    public Vector3 StartPosition
    {
        get
        {
            return startPosition;
        }
    }

    private void Awake()
    {
        startPosition = transform.position;
    }
   

}
