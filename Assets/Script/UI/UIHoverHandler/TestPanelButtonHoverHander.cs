using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestPanelButtonHoverHander : UIHoverHandler
{
    private new Transform transform;
    private void Awake()
    {
        transform = GetComponent<Transform>();
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one * 1.1f, 0.2f);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one, 0.2f);
    }
}
