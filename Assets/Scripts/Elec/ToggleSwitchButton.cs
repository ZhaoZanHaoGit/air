using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ToggleSwitchButton : MonoBehaviour
{
    public Transform buttonIns;
    public Vector3 upPos,downPos;
    public DynamicSwitch Switch;
    bool currentstate;
    private void OnMouseUp()
    {
        Switch.SetSwitchState(!Switch.isClosed);
        currentstate = Switch.isClosed;
        if (currentstate)
        {
            buttonIns.DOLocalMove(downPos, 0.2f);
        }
        else
        {
            buttonIns.DOLocalMove(upPos, 0.2f);
        }
    }
}
