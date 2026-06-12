using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UniversalDraggablePoint : MonoBehaviour
{
    private Camera mainCamera;

    // 标记当前是否正在被拖拽
    private bool isDragging = false;

    // 记录物体到摄像机的深度，防止拖拽时物体乱飞
    private float zDistance;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("找不到主摄像机！请确保你的摄像机标签(Tag)设置为 MainCamera。");
        }
    }

    void Update()
    {
        // 1. 按下瞬间（兼容鼠标左键点击 和 手机手指触碰屏幕）
        if (Input.GetMouseButtonDown(0))
        {
            // 从摄像机向我们点击的屏幕坐标发射一条隐形射线
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 物理系统检测：如果射线打到了东西
            if (Physics.Raycast(ray, out hit))
            {
                // 判断打到的东西是不是当前挂载脚本的物体
                if (hit.transform == transform)
                {
                    isDragging = true;
                    // 记录物体当前的屏幕Z轴深度
                    zDistance = mainCamera.WorldToScreenPoint(transform.position).z;
                }
            }
        }

        // 2. 持续按压/滑动时（兼容鼠标拖拽 和 手机手指滑动）
        if (isDragging && Input.GetMouseButton(0))
        {
            // 获取当前鼠标/手指的屏幕坐标，并带上固定的Z轴深度
            Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistance);

            // 将屏幕坐标转换回 3D 世界坐标并赋值给物体
            transform.position = mainCamera.ScreenToWorldPoint(screenPos);
        }

        // 3. 松开瞬间（兼容松开鼠标 和 手指离开屏幕）
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }
}
