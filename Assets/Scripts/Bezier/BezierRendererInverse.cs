using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class BezierRendererInverse : MonoBehaviour
{
    [Header("Anchor 点（Anchors）")]
    public Transform p0;
    public Transform p2;

    [Header("中点控制物体（On-Curve Handle）")]
    public Transform midpointHandle;

    [Header("渲染与交互配置")]
    [Range(10, 100)]
    public int resolution = 50;
    [Tooltip("点击线条的判定范围（像素）")]
    public float clickThresholdPixels = 30f;

    private LineRenderer lineRenderer;
    private Vector3 calculatedP1;
    private Camera mainCamera;
    DeleteLine destroybtn;

    void Start()
    {

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = resolution;
        mainCamera = Camera.main;

        if (midpointHandle != null && p0 != null && p2 != null && midpointHandle.position == Vector3.zero)
        {
            InitializeHandlePosition();
        }

        // 默认开始时隐藏控制点
        SetHandleActive(false);
    }

    void Update()
    {
        if (p0 == null || p2 == null || midpointHandle == null)
        {
            Debug.LogWarning("请确保 p0、p2 和 midpointHandle 都已正确设置！");
            Destroy(this.gameObject);
            return;
        }

        DrawCurveInverse();
        HandleSelectionLogic();
    }
    private void OnDestroy()
    {

        PneumaticPort portA = p0.GetComponent<PneumaticPort>();
        PneumaticPort portB = p2.GetComponent<PneumaticPort>();

        if (portA != null && portB != null)
        {
            portA.connectedTo = null;
            portB.connectedTo = null;
            portA.isOccupied = false;
            portB.isOccupied = false;
        }
    }
    // --- 1. 核心绘制逻辑 ---
    void DrawCurveInverse()
    {
        Vector3 pos0 = p0.position;
        Vector3 pos2 = p2.position;
        Vector3 posHandle = midpointHandle.position;

        calculatedP1 = 2f * posHandle - 0.5f * (pos0 + pos2);

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            Vector3 position = BezierIntepolate3(pos0, calculatedP1, pos2, t);
            lineRenderer.SetPosition(i, position);
        }
    }

    internal static Vector3 BezierIntepolate3(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        var u = 1 - t;
        var tt = t * t;
        return u * u * p0 + 2 * u * t * p1 + tt * p2;
    }

    // --- 2. 选择与隐藏逻辑 ---
    void HandleSelectionLogic()
    {
        bool inputDown = false;
        Vector3 inputScreenPos = Vector3.zero;

        // 兼容手机 Touch 和 PC 鼠标
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                inputDown = true;
                inputScreenPos = touch.position;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            inputDown = true;
            inputScreenPos = Input.mousePosition;
        }

        // 如果发生了点击
        if (inputDown)
        {
            // 第一步：先用 3D 物理射线检查是否直接点到了现有的三个控制球
            Ray ray = mainCamera.ScreenPointToRay(inputScreenPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == midpointHandle || hit.transform == p0 || hit.transform == p2)
                {
                    // 点中了控制点，显示并保持激活状态
                    SetHandleActive(true);
                    return;
                }
            }

            // 第二步：如果没有点中控制球，用数学方法检查是否点中了线条
            if (IsClickOnLine(inputScreenPos))
            {
                SetHandleActive(true);  // 点中线条，显示控制点
            }
            else
            {
                SetHandleActive(false); // 点中了空白处或其他无关物体，隐藏控制点
            }
        }
    }

    // --- 3. 数学检测：判断点击是否在线条附近 ---
    bool IsClickOnLine(Vector2 clickPos)
    {
        // 遍历组成曲线的所有线段
        for (int i = 0; i < resolution - 1; i++)
        {
            // 获取 3D 世界坐标
            Vector3 worldPos1 = lineRenderer.GetPosition(i);
            Vector3 worldPos2 = lineRenderer.GetPosition(i + 1);

            // 转换到 2D 屏幕坐标 (像素)
            Vector2 screenPos1 = mainCamera.WorldToScreenPoint(worldPos1);
            Vector2 screenPos2 = mainCamera.WorldToScreenPoint(worldPos2);

            // 计算点击点到这段线段的最短距离
            float distance = DistanceToSegment(clickPos, screenPos1, screenPos2);

            // 如果距离小于阈值（比如 30 像素），判定为点中
            if (distance <= clickThresholdPixels)
            {
                return true;
            }
        }
        return false;
    }

    // 数学工具：计算一个点到一条线段的最短距离
    float DistanceToSegment(Vector2 p, Vector2 v, Vector2 w)
    {
        float sqrLength = (v - w).sqrMagnitude;
        if (sqrLength == 0) return Vector2.Distance(p, v); // 线段两端点重合的情况

        // 计算投影点的比例 t
        float t = Mathf.Max(0, Mathf.Min(1, Vector2.Dot(p - v, w - v) / sqrLength));
        Vector2 projection = v + t * (w - v);

        return Vector2.Distance(p, projection);
    }

    // 状态控制：显示/隐藏中点
    void SetHandleActive(bool active)
    {
        /*
                if (active)
                {
                    destroybtn = SimulationLoop.Instance.deleteLineScript;
                    destroybtn.gameObject.SetActive(active);
                    destroybtn.lineObject = gameObject;
                }
                else
                {
                  //  Invoke("HideDeleteBtn", 0.1f);
                }*/
        if (midpointHandle != null && midpointHandle.gameObject.activeSelf != active)
        {
            midpointHandle.gameObject.SetActive(active);

        }
    }
    void HideDeleteBtn()
    {
        destroybtn = SimulationLoop.Instance.deleteLineScript;
        destroybtn.gameObject.SetActive(false);
        destroybtn.lineObject = null;
    }

    [ContextMenu("Initialize Handle Position")]
    void InitializeHandlePosition()
    {
        if (p0 != null && p2 != null && midpointHandle != null)
        {
            midpointHandle.position = (p0.position + p2.position) / 2f;
        }
    }
}