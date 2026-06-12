using Hypertonic.GridPlacement.GridInput;
using UnityEngine;

public class Camera3DZoomController : MonoBehaviour
{
    public UniversalInputDefinition inputDef; // 蓝色配置文件
    public float zoomSpeed = 20f;
    public float minHeight = 5f;  // 最小高度限制
    public float maxHeight = 100f; // 最大高度限制

    [Header("UI Movement Settings")]
    public float moveSpeed = 25f; // 按钮控制的移动速度

    [Header("Mouse Drag Settings")]
    public bool enableMouseDrag = true; // 是否启用中键拖拽

    private Camera cam;
    private Plane groundPlane;

    // 记录四个方向的移动状态
    private bool isMovingForward, isMovingBackward, isMovingLeft, isMovingRight;

    // 记录鼠标中键拖拽的状态
    private bool isDragging = false;
    private Vector3 dragStartWorldPos;

    void Awake()
    {
        cam = GetComponent<Camera>();
        // 创建一个位于 Y=0 处的水平面作为参考
        groundPlane = new Plane(Vector3.up, Vector3.zero);
    }

    void Update()
    {
        HandleZoom();
        HandleMouseDrag(); // 处理鼠标中键拖拽
        HandleUIMovement();
    }

    // ========== 1. 原有的缩放逻辑 ==========
    private void HandleZoom()
    {
        float delta = inputDef.GetZoomDelta();
        if (Mathf.Abs(delta) < 0.0001f) return;

        Vector2 screenPivot = inputDef.GetZoomScreenPivot();

        // 缩放前：获取屏幕点对应的世界坐标锚点
        Vector3 worldPivotBefore = GetGroundIntersection(screenPivot);

        // 执行缩放：沿相机前进方向移动
        Vector3 zoomDir = transform.forward;
        Vector3 moveAmount = zoomDir * delta * zoomSpeed;

        // 预判高度限制
        float nextHeight = (transform.position + moveAmount).y;
        if (nextHeight > minHeight && nextHeight < maxHeight)
        {
            transform.position += moveAmount;
        }

        // 缩放后：再次获取同一点的地面交点并补偿位移
        Vector3 worldPivotAfter = GetGroundIntersection(screenPivot);

        // 将相机水平移动，抵消中心点偏移
        Vector3 offset = worldPivotBefore - worldPivotAfter;
        transform.position += offset;
    }

    // ========== 2. 新增：鼠标中键拖拽逻辑 ==========
    private void HandleMouseDrag()
    {
        if (!enableMouseDrag) return;

        // 2 代表鼠标中键 (0:左键, 1:右键, 2:中键)
        if (Input.GetMouseButtonDown(2))
        {
            isDragging = true;
            // 记录按下瞬间，鼠标指针在地面上的世界坐标
            dragStartWorldPos = GetGroundIntersection(Input.mousePosition);
        }

        if (isDragging && Input.GetMouseButton(2))
        {
            // 当前帧鼠标指针在地面上的世界坐标
            Vector3 dragCurrentWorldPos = GetGroundIntersection(Input.mousePosition);

            // 计算位移差（起始点 - 当前点）
            // 这样可以让鼠标“抓住”地面，鼠标动到哪，地面跟着到哪
            Vector3 delta = dragStartWorldPos - dragCurrentWorldPos;

            // 保持 Y 轴不动，只在 XZ 平面上平移相机
            delta.y = 0;

            transform.position += delta;
        }

        if (Input.GetMouseButtonUp(2))
        {
            isDragging = false;
        }
    }

    // ========== 3. 原有的 UI 移动逻辑 ==========
    private void HandleUIMovement()
    {
        // 如果正在拖拽鼠标，可以考虑禁用 UI/键盘按钮移动，避免冲突
        if (isDragging) return;

        // 如果没有任何按钮被按下，则直接返回
        if (!isMovingForward && !isMovingBackward && !isMovingLeft && !isMovingRight) return;

        // 获取相机在水平面（XZ平面）上的前向和右向向量，确保移动不会改变相机高度
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 moveDir = Vector3.zero;

        // 根据按下的按钮叠加移动方向
        if (isMovingForward) moveDir += forward;
        if (isMovingBackward) moveDir -= forward;
        if (isMovingLeft) moveDir -= right;
        if (isMovingRight) moveDir += right;

        // 归一化以防止斜向同时按（比如左上）时移动速度过快
        if (moveDir.magnitude > 0.1f)
        {
            moveDir.Normalize();
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
    }

    // 辅助函数：计算屏幕点与地面的交点
    private Vector3 GetGroundIntersection(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        if (groundPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }
        return ray.GetPoint(10);
    }


    // ========== 4. 供 UI 按钮 EventTrigger 调用的公开方法 ==========

    // 向前
    public void OnPressForward() => isMovingForward = true;
    public void OnReleaseForward() => isMovingFalseOrTrue(false);
    private void isMovingFalseOrTrue(bool val) => isMovingForward = val; // 保持原有逻辑的简洁性

    public void OnReleaseForward_Clean() => isMovingForward = false;

    // 恢复你原有的干净 Lambda
    public void OnReleaseBackward() => isMovingBackward = false;
    public void OnPressBackward() => isMovingBackward = true;

    public void OnPressLeft() => isMovingLeft = true;
    public void OnReleaseLeft() => isMovingLeft = false;

    public void OnPressRight() => isMovingRight = true;
    public void OnReleaseRight() => isMovingRight = false;
}