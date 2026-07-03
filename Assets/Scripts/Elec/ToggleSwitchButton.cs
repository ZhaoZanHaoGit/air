using DG.Tweening;
using UnityEngine;

public class ToggleSwitchButton : MonoBehaviour
{
    public Transform buttonIns;
    public Vector3 upPos, downPos;
    public DynamicSwitch Switch;

    private void Start()
    {
        // 根据 DynamicSwitch 的初始闸刀状态，同步按钮视觉位置
        if (Switch != null && buttonIns != null)
        {
            buttonIns.localPosition = Switch.IsClosed ? downPos : upPos;
        }
    }

    private void OnMouseUp()
    {
        if (Switch == null) return;

        Switch.SetSwitchState(!Switch.IsClosed);

        if (Switch.IsClosed)
        {
            buttonIns.DOLocalMove(downPos, 0.2f);
        }
        else
        {
            buttonIns.DOLocalMove(upPos, 0.2f);
        }
    }
}
