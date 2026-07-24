using DG.Tweening;
using UnityEngine;

public class ToggleSwitchButton : MonoBehaviour
{
    public Transform buttonIns;
    public Vector3 upPos, downPos;
    public DynamicSwitch Switch1, Switch2;

    private void Start()
    {
        // 根据 DynamicSwitch 的初始闸刀状态，同步按钮视觉位置
        if (Switch1 != null && buttonIns != null)
        {
            buttonIns.localPosition = Switch1.IsClosed ? downPos : upPos;
        }
    }

    private void OnMouseUp()
    {
        if (Switch1 == null) return;

        Switch1.SetSwitchState(!Switch1.IsClosed);

        if (Switch1.IsClosed)
        {
            buttonIns.DOLocalMove(downPos, 0.2f);
        }
        else
        {
            buttonIns.DOLocalMove(upPos, 0.2f);
        }
        if (Switch2 == null) return;
        Switch2.SetSwitchState(!Switch2.IsClosed);
    }
}
