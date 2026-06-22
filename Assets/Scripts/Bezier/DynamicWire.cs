using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DynamicWire : CircuitEdge
{
    [Header("========= 样条曲线锚点 (导线特有表现层) =========")]
    public Transform p0;            // 对应 nodeA 的位置
    public Transform p2;            // 对应 nodeB 的位置
    public Transform handle20;      // 20% 控制点
    public Transform handle50;      // 50% 控制点
    public Transform handle80;      // 80% 控制点

    [Header("========= 渲染与交互配置 =========")]
    [Range(10, 100)] public int resolution = 50;
    public float clickThresholdPixels = 30f;
    public Vector3 defaultCurveOffset = new Vector3(0, -0.5f, 0);

    private LineRenderer lineRenderer;
    private Camera mainCamera;
    private bool isInitialized = false;
    private Vector3[] controlPoints = new Vector3[5];

    // 实现父类的抽象接口：有形导线拉出来默认就是永远导通的
    public override bool IsEdgeConnected => true;

    /// <summary>
    /// 外部画线系统在运行时，动态连好两个接线柱后调用的初始化方法
    /// </summary>
    public void SetupWire(DynamicNode a, DynamicNode b)
    {
        nodeA = a;
        nodeB = b;
        p0 = a.transform;
        p2 = b.transform;

        // 调用父类的注册方法
        RegisterToManager();
    }

    protected override void Start()
    {
        base.Start(); // 执行父类的ID生成逻辑

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = resolution;
        mainCamera = Camera.main;
        SetHandlesActive(false);

        // 如果是编辑器里预摆放的线
        if (nodeA != null && nodeB != null && (p0 == null || p2 == null))
        {
            p0 = nodeA.transform;
            p2 = nodeB.transform;
        }
    }

    void Update()
    {
        if (p0 == null || p2 == null || handle20 == null || handle50 == null || handle80 == null)
        {
            Destroy(this.gameObject);
            return;
        }

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
        base.OnDestroy();
        // ---> [新增] 销毁管线时，从 SimulationLoop 中移除连接数据 <---
        PortBase portA = null;
        PortBase portB = null;
        if (p0)
        { portA = p0.GetComponent<PortBase>(); }

        if (p2)
        { portB = p2.GetComponent<PortBase>(); }


        if (portA != null && portB != null)
        {
            portA.Disconnect();
            portB.Disconnect();

        }
    }

    private void UpdateWireVisualEffect()
    {
        // 如果运行时你想看这根线两端到底在干嘛，可以解开这行注释
        Debug.Log($"[导线单帧检查] {gameObject.name} -> NodeA极性:{nodeA.currentPowerStatus}(级数:{nodeA.currentLevel}) | NodeB极性:{nodeB.currentPowerStatus}(级数:{nodeB.currentLevel})");
        if (nodeA.currentPowerStatus == NodeType.Positive || nodeB.currentPowerStatus == NodeType.Positive)
            lineRenderer.startColor = lineRenderer.endColor = Color.red;
        else if (nodeA.currentPowerStatus == NodeType.Negative || nodeB.currentPowerStatus == NodeType.Negative)
            lineRenderer.startColor = lineRenderer.endColor = Color.blue;
        else
            lineRenderer.startColor = lineRenderer.endColor = Color.white;
    }

    #region 贝塞尔/样条曲线渲染与点击逻辑 (保持原样)
    void DrawSpline() { controlPoints[0] = p0.position; controlPoints[1] = handle20.position; controlPoints[2] = handle50.position; controlPoints[3] = handle80.position; controlPoints[4] = p2.position; for (int i = 0; i < resolution; i++) lineRenderer.SetPosition(i, GetPointOnSpline(i / (float)(resolution - 1))); }
    Vector3 GetPointOnSpline(float t) { int numSections = 4; int currSection = Mathf.Min(Mathf.FloorToInt(t * numSections), numSections - 1); float localT = (t * numSections) - currSection; return GetCatmullRomPosition(localT, GetNode(currSection - 1), GetNode(currSection), GetNode(currSection + 1), GetNode(currSection + 2)); }
    Vector3 GetNode(int index) { if (index < 0) return controlPoints[0] + (controlPoints[0] - controlPoints[1]); if (index >= 5) return controlPoints[4] + (controlPoints[4] - controlPoints[3]); return controlPoints[index]; }
    Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) { float t2 = t * t; float t3 = t2 * t; return 0.5f * ((2f * p1) + ((p2 - p0) * t) + ((2f * p0 - 5f * p1 + 4f * p2 - p3) * t2) + ((-p0 + 3f * p1 - 3f * p2 + p3) * t3)); }
    void HandleSelectionLogic()
    {
        bool inputDown = false;
        Vector3 inputScreenPos = Vector3.zero;
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                inputDown = true;
                inputScreenPos = touch.position;
            }
        }
        else if (Input.GetMouseButtonDown(0)) { inputDown = true; inputScreenPos = Input.mousePosition; }
        if (inputDown)
        {
            Ray ray = mainCamera.ScreenPointToRay(inputScreenPos);
            if (Physics.Raycast(ray, out RaycastHit hit)) { if (hit.transform == handle20 || hit.transform == handle50 || hit.transform == handle80 || hit.transform == p0 || hit.transform == p2) { SetHandlesActive(true); return; } }
            if (IsClickOnLine(inputScreenPos))
            {
                SetHandlesActive(true);
                SimulationLoop.Instance.currentLine = this.gameObject;
            }
            else SetHandlesActive(false);
        }
    }
    bool IsClickOnLine(Vector2 clickPos) { for (int i = 0; i < resolution - 1; i++) { if (Vector2.Distance(clickPos, DistanceToSegment(clickPos, mainCamera.WorldToScreenPoint(lineRenderer.GetPosition(i)), mainCamera.WorldToScreenPoint(lineRenderer.GetPosition(i + 1)))) <= clickThresholdPixels) return true; } return false; }
    Vector2 DistanceToSegment(Vector2 p, Vector2 v, Vector2 w) { float sqrLength = (v - w).sqrMagnitude; if (sqrLength == 0) return v; float t = Mathf.Max(0, Mathf.Min(1, Vector2.Dot(p - v, w - v) / sqrLength)); return v + t * (w - v); }
    void SetHandlesActive(bool active) { if (handle20) handle20.gameObject.SetActive(active); if (handle50) handle50.gameObject.SetActive(active); if (handle80) handle80.gameObject.SetActive(active); }
    public void InitializeHandlesCurved() { if (p0 && p2 && handle20 && handle50 && handle80) { Vector3 s = p0.position; Vector3 e = p2.position; handle20.position = Vector3.Lerp(s, e, 0.2f) + defaultCurveOffset * 0.64f; handle50.position = Vector3.Lerp(s, e, 0.5f) + defaultCurveOffset * 1.00f; handle80.position = Vector3.Lerp(s, e, 0.8f) + defaultCurveOffset * 0.64f; } }
    #endregion
}