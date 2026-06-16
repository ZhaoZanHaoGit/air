using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DynamicWire : MonoBehaviour
{
    [Header("========= 拓扑逻辑配置 =========")]
    public DynamicNode nodeA;        // 逻辑起始节点
    public DynamicNode nodeB;        // 逻辑终止节点
    public bool isSwitch = false;    // 是否是开关
    public bool isClosed = true;     // 开关是否闭合
    [HideInInspector] public string wireID;

    [Header("========= 样条曲线锚点 (表现层) =========")]
    public Transform p0;            // 对应 nodeA 的 Transform
    public Transform p2;            // 对应 nodeB 的 Transform
    public Transform handle20;      // 20% 控制点
    public Transform handle50;      // 50% 控制点
    public Transform handle80;      // 80% 控制点

    [Header("========= 渲染与交互配置 =========")]
    [Range(10, 100)] public int resolution = 50;
    public float clickThresholdPixels = 30f;
    public Vector3 defaultCurveOffset = new Vector3(0, -0.5f, 0); // 默认向下下垂 0.5 米

    private LineRenderer lineRenderer;
    private Camera mainCamera;
    private bool isInitialized = false;
    private Vector3[] controlPoints = new Vector3[5];

    public bool IsConnected => !isSwitch || isClosed;

    /// <summary>
    /// 外部（如画线系统）动态生成连线时调用的核心初始化函数
    /// </summary>
    public void SetupWire(DynamicNode a, DynamicNode b)
    {
        nodeA = a;
        nodeB = b;
        p0 = a.transform;
        p2 = b.transform;

        wireID = System.Guid.NewGuid().ToString();

        // 注册到电路拓扑管理器，激活导通属性
        if (DynamicCircuitManager.Instance != null)
        {
            DynamicCircuitManager.Instance.RegisterWire(this);
        }
    }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = resolution;
        mainCamera = Camera.main;

        // 默认隐藏手柄
        SetHandlesActive(false);

        // 如果是在编辑器里预先摆放好的导线，自动进行关联
        if (nodeA != null && nodeB != null && (p0 == null || p2 == null))
        {
            p0 = nodeA.transform;
            p2 = nodeB.transform;
            DynamicCircuitManager.Instance.RegisterWire(this);
        }
    }

    void Update()
    {
        // 防呆机制：缺少必要节点则自毁
        if (p0 == null || p2 == null || handle20 == null || handle50 == null || handle80 == null)
        {
            Debug.LogWarning("存在未绑定的控制点，导线已自动销毁！");
            Destroy(this.gameObject);
            return;
        }

        // 延迟初始化：确保两端拉开物理距离后生成下垂曲线
        if (!isInitialized && Vector3.Distance(p0.position, p2.position) > 0.01f)
        {
            InitializeHandlesCurved();
            isInitialized = true;
        }

        DrawSpline();
        HandleSelectionLogic();
        UpdateWireVisualEffect();
    }

    private void OnDestroy()
    {
        // 销毁时，自动从电路管理器注销并重新计算拓扑
        if (DynamicCircuitManager.Instance != null)
        {
            DynamicCircuitManager.Instance.UnregisterWire(this);
        }
    }

    public void SetSwitchState(bool closed)
    {
        if (!isSwitch) return;
        isClosed = closed;
        DynamicCircuitManager.Instance.MarkTopologyDirty();
    }

    /// <summary>
    /// 根据当前电路的通电染色状态，动态改变导线的渲染颜色
    /// </summary>
    private void UpdateWireVisualEffect()
    {
        if (!IsConnected)
        {
            lineRenderer.startColor = lineRenderer.endColor = Color.gray; // 断开显示灰色
            return;
        }

        // 只要有一端碰到正极，整条导线渲染为红色（代表高电平）
        if (nodeA.currentPowerStatus == NodeType.Positive || nodeB.currentPowerStatus == NodeType.Positive)
        {
            lineRenderer.startColor = lineRenderer.endColor = Color.red;
        }
        // 只要有一端碰到负极，整条导线渲染为蓝色（代表低电平/地）
        else if (nodeA.currentPowerStatus == NodeType.Negative || nodeB.currentPowerStatus == NodeType.Negative)
        {
            lineRenderer.startColor = lineRenderer.endColor = Color.blue;
        }
        else
        {
            lineRenderer.startColor = lineRenderer.endColor = Color.white; // 未通电显示白色
        }
    }

    #region 样条曲线核心绘制 (Catmull-Rom)
    void DrawSpline()
    {
        controlPoints[0] = p0.position;
        controlPoints[1] = handle20.position;
        controlPoints[2] = handle50.position;
        controlPoints[3] = handle80.position;
        controlPoints[4] = p2.position;

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            lineRenderer.SetPosition(i, GetPointOnSpline(t));
        }
    }

    Vector3 GetPointOnSpline(float t)
    {
        int numSections = controlPoints.Length - 1;
        int currSection = Mathf.Min(Mathf.FloorToInt(t * numSections), numSections - 1);
        float localT = (t * numSections) - currSection;

        Vector3 pA = GetNode(currSection - 1);
        Vector3 pB = GetNode(currSection);
        Vector3 pC = GetNode(currSection + 1);
        Vector3 pD = GetNode(currSection + 2);

        return GetCatmullRomPosition(localT, pA, pB, pC, pD);
    }

    Vector3 GetNode(int index)
    {
        if (index < 0) return controlPoints[0] + (controlPoints[0] - controlPoints[1]);
        if (index >= controlPoints.Length) return controlPoints[controlPoints.Length - 1] + (controlPoints[controlPoints.Length - 1] - controlPoints[controlPoints.Length - 2]);
        return controlPoints[index];
    }

    Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float t2 = t * t; float t3 = t2 * t;
        Vector3 a = 2f * p1; Vector3 b = p2 - p0; Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3; Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;
        return 0.5f * (a + (b * t) + (c * t2) + (d * t3));
    }
    #endregion

    #region 射线交互选择与手柄控制
    void HandleSelectionLogic()
    {
        bool inputDown = false; Vector3 inputScreenPos = Vector3.zero;
        if (Input.touchCount > 0) { Touch touch = Input.GetTouch(0); if (touch.phase == TouchPhase.Began) { inputDown = true; inputScreenPos = touch.position; } }
        else if (Input.GetMouseButtonDown(0)) { inputDown = true; inputScreenPos = Input.mousePosition; }

        if (inputDown)
        {
            Ray ray = mainCamera.ScreenPointToRay(inputScreenPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == handle20 || hit.transform == handle50 || hit.transform == handle80 || hit.transform == p0 || hit.transform == p2)
                {
                    SetHandlesActive(true); return;
                }
            }

            if (IsClickOnLine(inputScreenPos)) SetHandlesActive(true);
            else SetHandlesActive(false);
        }
    }

    bool IsClickOnLine(Vector2 clickPos)
    {
        for (int i = 0; i < resolution - 1; i++)
        {
            Vector3 worldPos1 = lineRenderer.GetPosition(i); Vector3 worldPos2 = lineRenderer.GetPosition(i + 1);
            Vector2 screenPos1 = mainCamera.WorldToScreenPoint(worldPos1); Vector2 screenPos2 = mainCamera.WorldToScreenPoint(worldPos2);
            if (DistanceToSegment(clickPos, screenPos1, screenPos2) <= clickThresholdPixels) return true;
        }
        return false;
    }

    float DistanceToSegment(Vector2 p, Vector2 v, Vector2 w) { float sqrLength = (v - w).sqrMagnitude; if (sqrLength == 0) return Vector2.Distance(p, v); float t = Mathf.Max(0, Mathf.Min(1, Vector2.Dot(p - v, w - v) / sqrLength)); return Vector2.Distance(p, v + t * (w - v)); }

    void SetHandlesActive(bool active)
    {
        if (handle20 != null) handle20.gameObject.SetActive(active);
        if (handle50 != null) handle50.gameObject.SetActive(active);
        if (handle80 != null) handle80.gameObject.SetActive(active);
    }

    public void InitializeHandlesCurved()
    {
        if (p0 != null && p2 != null && handle20 != null && handle50 != null && handle80 != null)
        {
            Vector3 startPos = p0.position; Vector3 endPos = p2.position;
            handle20.position = Vector3.Lerp(startPos, endPos, 0.2f) + defaultCurveOffset * 0.64f;
            handle50.position = Vector3.Lerp(startPos, endPos, 0.5f) + defaultCurveOffset * 1.00f;
            handle80.position = Vector3.Lerp(startPos, endPos, 0.8f) + defaultCurveOffset * 0.64f;
        }
    }
    #endregion
}