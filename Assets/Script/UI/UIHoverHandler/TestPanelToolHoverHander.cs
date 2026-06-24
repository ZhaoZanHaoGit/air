using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestPanelToolHoverHander : UIHoverHandler
{
    public GameObject passObj;
    public Transform iconImage;
    public override void OnPointerEnter(PointerEventData eventData)
    {
        passObj.SetActive(true);
        iconImage.DOScale(Vector3.one * 1.1f, 0.2f).SetEase(Ease.OutBack);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        passObj.SetActive(false);
        iconImage.DOScale(Vector3.one, 0.2f).SetEase(Ease.InBack);
    }
}
