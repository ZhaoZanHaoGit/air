using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragUI : MonoBehaviour, IDragHandler, IPointerDownHandler
{
   [SerializeField] private RectTransform dragTarget;
    [SerializeField] private Canvas canvas;
 
    private void Awake()
    {
        if (dragTarget == null) dragTarget = transform.GetComponent<RectTransform>();
        if (canvas == null) canvas = CanvasController.Instance.GetComponent<Canvas>();
    }
 
    public void OnDrag(PointerEventData eventData)
    {
        // 移动拖拽框的位置
        dragTarget.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
 
    public void OnPointerDown(PointerEventData eventData)
    {
        // 把当前选中的拖拽框显示在最前面
        dragTarget.SetAsLastSibling();
    }
}
