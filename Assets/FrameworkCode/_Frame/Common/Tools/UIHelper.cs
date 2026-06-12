using UnityEngine;
using UnityEngine.EventSystems;

public class UIHelper : DDOLSingleton<UIHelper>
{
    public override void Init()
    {

    }
    public bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        System.Collections.Generic.List<RaycastResult> results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}