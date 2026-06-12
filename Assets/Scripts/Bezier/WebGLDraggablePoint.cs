using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WebGLDraggablePoint : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private float zDistance;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 1. 统一输入状态变量
        bool inputDown = false;
        bool inputHeld = false;
        bool inputUp = false;
        Vector3 inputScreenPos = Vector3.zero;

        // 2. 优先检测真实的手机触摸事件
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            inputScreenPos = touch.position;

            if (touch.phase == TouchPhase.Began) inputDown = true;
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) inputHeld = true;
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) inputUp = true;
        }
        // 3. 如果没有触摸，则使用鼠标事件 (PC端)
        else
        {
            inputScreenPos = Input.mousePosition;
            inputDown = Input.GetMouseButtonDown(0);
            inputHeld = Input.GetMouseButton(0);
            inputUp = Input.GetMouseButtonUp(0);
        }

        // 4. 执行核心拖拽逻辑 (与之前完全相同，只是替换了输入源)
        if (inputDown)
        {
            Ray ray = mainCamera.ScreenPointToRay(inputScreenPos);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
            {
                isDragging = true;
                zDistance = mainCamera.WorldToScreenPoint(transform.position).z;
            }
        }

        if (isDragging && inputHeld)
        {
            // 记录拖拽前的高度，确保不会因为浮点数计算产生漂移
            float fixedY = transform.position.y;

            // 计算新的位置
            Vector3 screenPosWithZ = new Vector3(inputScreenPos.x, inputScreenPos.y, zDistance);
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPosWithZ);

            // 核心修改：只应用 X 和 Z，保持原有的 Y 不变
            transform.position = new Vector3(worldPos.x, fixedY, worldPos.z);
        }

        if (inputUp)
        {
            isDragging = false;
        }
    }
}
