using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("鼠标进入");
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("鼠标移出");
    }
}
