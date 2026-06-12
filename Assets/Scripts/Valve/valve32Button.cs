using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class valve32Button : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public ManualValve32 valve;
    public float openPos = 0.01f, ClosePos = 0f;

    public void OnPointerUp(PointerEventData eventData)
    {
        valve.isPressed = !valve.isPressed;
        if (valve.isPressed)
            transform.parent.localPosition = new Vector3(ClosePos, 0, 0);
        else

            transform.parent.localPosition = new Vector3(openPos, 0, 0);

        /*
        valve.isPressed=false;
        transform.parent.localPosition = new Vector3(openPos, 0, 0);
        */
    }

    public void OnPointerDown(PointerEventData eventData)
    {/*
        valve.isPressed = true;
        transform.parent.localPosition = new Vector3(ClosePos, 0, 0);*/
    }
}
