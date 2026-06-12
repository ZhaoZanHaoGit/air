using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 该脚本实现展示模型，this script is for controlling object rotation and zoom
/// </summary>
public class ObjectShowContorller : MonoBehaviour
{
    // 旋转速度
    public float rotationSpeed = 20.0f;
    // 缩放速度
    public float zoomSpeed = 0.5f;
    // 最小缩放比例
    public float minZoom = 0.1f;
    // 最大缩放比例
    public float maxZoom = 5.0f;

    // 鼠标按下的初始位置
    private Vector3 lastMousePosition;
    // 是否正在拖动
    private bool isDragging = false;

    void Update()
    {
        // 检查是否在UI上

        if (EventSystem.current.IsPointerOverGameObject()&& !GetHoveredUIObject("ModelTexture"))
        {
            
            isDragging = false;
            return;
        }

        // 检测鼠标左键按下
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        // 检测鼠标左键松开
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // 拖动旋转
        if (isDragging)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            float rotationX = delta.y * rotationSpeed * Time.deltaTime;
            float rotationY = -delta.x * rotationSpeed * Time.deltaTime;

            // 绕X轴旋转（上下）
            transform.Rotate(Vector3.right, rotationX, Space.World);
            // 绕Y轴旋转（左右）
            transform.Rotate(Vector3.up, rotationY, Space.World);

            lastMousePosition = Input.mousePosition;
        }

        // 鼠标滚轮缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            Vector3 scale = transform.localScale;
            float scaleChange = scroll * zoomSpeed;
            scale += Vector3.one * scaleChange;

            // 限制缩放范围
            scale = Vector3.Max(scale, Vector3.one * minZoom);
            scale = Vector3.Min(scale, Vector3.one * maxZoom);

            transform.localScale = scale;
        }
    }

    private bool GetHoveredUIObject(string goName)
    {
        
        // 创建 PointerEventData
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        // 存储 Raycast 结果
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // 返回第一个检测到的 UI 对象
        foreach (RaycastResult result in results)
        {
            if (result.gameObject != null&& result.gameObject.name== goName)
            {
                return true;
            }
        }

        return false;
    }
}