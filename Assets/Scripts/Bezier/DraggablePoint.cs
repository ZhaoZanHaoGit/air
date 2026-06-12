using UnityEngine;

[RequireComponent(typeof(Collider))] // 必须要有碰撞体才能检测到鼠标点击
public class DraggablePoint : MonoBehaviour
{
    private Camera mainCamera;
    private float zDistance;

    void Start()
    {
        mainCamera = Camera.main;
    }

    // 当鼠标按下时记录物体到摄像机的Z轴距离
    void OnMouseDown()
    {
        zDistance = mainCamera.WorldToScreenPoint(transform.position).z;
    }

    // 当鼠标拖拽时实时更新位置
    void OnMouseDrag()
    {
        Vector3 mouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistance);
        transform.position = mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }
}