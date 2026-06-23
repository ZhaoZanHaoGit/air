using UnityEngine;
using UnityEngine.EventSystems;

public class TestPanelImageHoverHander : UIHoverHandler
{
    public GameObject passObj;
    public override void OnPointerEnter(PointerEventData eventData)
    {
        passObj.SetActive(true);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        passObj.SetActive(false);
    }
}
