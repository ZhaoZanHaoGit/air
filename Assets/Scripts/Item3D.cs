using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item3D : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public UI_TestPanelItemBTN itemUI;
    public Item itemSelf;
    /// <summary>
    /// 是否已经被创建
    /// </summary>
    public bool hasCreate=false;
    // 修改委托，增加位置参数
    public delegate void OnObjectSelectedEvent(GameObject gameObject, Vector2 screenPos);
    public static event OnObjectSelectedEvent OnObjectSelected;

    public void OnPointerUp(PointerEventData eventData)
    {
        // eventData.pointerPress 是最初按下时的物体
        // eventData.rawPointerPress 是射线直接命中的最底层物体

        if (eventData.rawPointerPress != this.gameObject)
        {
            // 如果命中的不是父物体自己（比如命中的是子物体），则直接返回
            Debug.Log("点击到了子物体 " + eventData.rawPointerPress.name + "，拦截父物体逻辑");
            return;
        }
        // 增加调试日志
        Debug.Log($"[WebGL Test] Clicked on: {gameObject.name} at {eventData.position}");
        // 传递当前物体以及点击的屏幕坐标
        OnObjectSelected?.Invoke(gameObject, eventData.position);
    }
    public void SetDescribe(Image image,TextMeshProUGUI text  )
    {
        image.sprite= itemSelf.itemImage;
        text.text = itemSelf.itemDescription;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 必须保留以确保 PointerUp 生效
    }
}
