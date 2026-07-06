using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class linerendeler : MonoBehaviour
{
    [Header("Anchor 点（两端接口）")]
    public Transform p0;
    public Transform p2;

    [Header("控制节点（On-Curve Handles）")]
    public Transform handle20; // 20% 位置
    public Transform handle50; // 50% 位置
    public Transform handle80; // 80% 位置

    [Header("渲染与交互配置")]
    [Range(10, 100)]
    public int resolution = 50;
    [Tooltip("点击线条的判定范围（像素）")]
    public float clickThresholdPixels = 30f;
    [Tooltip("初始创建时的弯曲偏移量（控制默认下垂方向和力度）")]
    public Vector3 defaultCurveOffset = new Vector3(0.5f, 0, 0.5f);

    private LineRenderer lineRenderer;
    private Camera mainCamera;

    // --- 线缆注册：存储两端 portandvalve 数据 + PortBase 引用，用于注销时定位 ---
    private portandvalve _wirePortA;
    private portandvalve _wirePortB;
    private PortBase _wireBaseA;
    private PortBase _wireBaseB;
    private bool isConnectionRegistered = false;

    // 存储样条曲线的所有核心节点
    private Vector3[] controlPoints = new Vector3[5];
    // 1. 在变量声明区域新增一个标记
    private bool isInitialized = false;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = resolution;
        mainCamera = Camera.main;
        /*
                // 如果三个控制点都在原点，且首尾已经连接，则进行弯曲初始化
                if (handle20 != null && handle50 != null && handle80 != null && p0 != null && p2 != null)
                {
                    if (handle50.position == Vector3.zero)
                    {
                        InitializeHandlesCurved();
                    }
                }
        */
        // 默认开始时隐藏控制点
        SetHandlesActive(false);
    }

    void Update()
    {
        // 防呆机制：缺少任何一个节点则自毁
        if (p0 == null || p2 == null || handle20 == null || handle50 == null || handle80 == null)
        {
            Debug.LogWarning("存在未绑定的控制点，管线已销毁！");
            Destroy(this.gameObject);
            return;
        }
        // 2.核心修复：延迟到 Update 中进行初始化
        // 确保 p0 和 p2 已经有了真实的物理距离（避免在同一点初始化）且只执行一次
        if (!isInitialized && Vector3.Distance(p0.position, p2.position) > 0.01f)
        {
            InitializeHandlesCurved();

            // ---> [新增] 在初始化曲线的同时，注册连接数据 <---
            RegisterConnection();


            isInitialized = true;
        }

        DrawSpline();
        HandleSelectionLogic();
    }

    private void OnDestroy()
    {
        // ---> [新增] 销毁管线时，从 SimulationLoop 中移除连接数据 <---
        UnregisterConnection();
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

    // --- 1. 核心绘制逻辑 (Catmull-Rom 样条) ---
    void DrawSpline()
    {
        // 1. 更新当前的所有控制节点位置
        controlPoints[0] = p0.position;
        controlPoints[1] = handle20.position;
        controlPoints[2] = handle50.position;
        controlPoints[3] = handle80.position;
        controlPoints[4] = p2.position;

        // 2. 将整条线等分为 resolution 个点进行渲染
        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            Vector3 position = GetPointOnSpline(t);
            lineRenderer.SetPosition(i, position);
        }
    }

    // 根据总进度 t (0~1) 获取样条曲线上的点
    Vector3 GetPointOnSpline(float t)
    {
        int numSections = controlPoints.Length - 1; // 一共 4 段
        // 确定当前 t 属于哪一个线段 (0-1, 1-2, 2-3, 3-4)
        int currSection = Mathf.Min(Mathf.FloorToInt(t * numSections), numSections - 1);
        // 计算在该线段内的局部进度 localT
        float localT = (t * numSections) - currSection;

        // 获取 Catmull-Rom 计算所需的 4 个参考点
        Vector3 pA = GetNode(currSection - 1);
        Vector3 pB = GetNode(currSection);     // 线段起点
        Vector3 pC = GetNode(currSection + 1); // 线段终点
        Vector3 pD = GetNode(currSection + 2);

        return GetCatmullRomPosition(localT, pA, pB, pC, pD);
    }

    // 安全获取节点（处理首尾越界，生成虚拟延长点以保持切线平滑）
    Vector3 GetNode(int index)
    {
        if (index < 0)
            return controlPoints[0] + (controlPoints[0] - controlPoints[1]);
        if (index >= controlPoints.Length)
            return controlPoints[controlPoints.Length - 1] + (controlPoints[controlPoints.Length - 1] - controlPoints[controlPoints.Length - 2]);

        return controlPoints[index];
    }

    // 数学工具：标准 Catmull-Rom 插值公式
    Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        return 0.5f * (a + (b * t) + (c * t2) + (d * t3));
    }

    // --- 2. 选择与隐藏逻辑 ---
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
        else if (Input.GetMouseButtonDown(0))
        {
            inputDown = true;
            inputScreenPos = Input.mousePosition;
        }

        if (inputDown)
        {
            // 物理射线检查所有控制球
            Ray ray = mainCamera.ScreenPointToRay(inputScreenPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == handle20 || hit.transform == handle50 || hit.transform == handle80 ||
                    hit.transform == p0 || hit.transform == p2)
                {
                    SetHandlesActive(true);
                    return;
                }
            }

            // 屏幕空间数学检查
            if (IsClickOnLine(inputScreenPos))
            {
                SimulationLoop.Instance.currentLine = this.gameObject;
                SetHandlesActive(true);
            }
            else
            {
                SetHandlesActive(false);
            }
        }
    }

    bool IsClickOnLine(Vector2 clickPos)
    {
        for (int i = 0; i < resolution - 1; i++)
        {
            Vector3 worldPos1 = lineRenderer.GetPosition(i);
            Vector3 worldPos2 = lineRenderer.GetPosition(i + 1);

            Vector2 screenPos1 = mainCamera.WorldToScreenPoint(worldPos1);
            Vector2 screenPos2 = mainCamera.WorldToScreenPoint(worldPos2);

            float distance = DistanceToSegment(clickPos, screenPos1, screenPos2);

            if (distance <= clickThresholdPixels)
            {
                return true;
            }
        }
        return false;
    }

    float DistanceToSegment(Vector2 p, Vector2 v, Vector2 w)
    {
        float sqrLength = (v - w).sqrMagnitude;
        if (sqrLength == 0) return Vector2.Distance(p, v);

        float t = Mathf.Max(0, Mathf.Min(1, Vector2.Dot(p - v, w - v) / sqrLength));
        Vector2 projection = v + t * (w - v);

        return Vector2.Distance(p, projection);
    }

    // --- 3. 状态控制与初始化 ---
    void SetHandlesActive(bool active)
    {


        if (handle20 != null) handle20.gameObject.SetActive(active);
        if (handle50 != null) handle50.gameObject.SetActive(active);
        if (handle80 != null) handle80.gameObject.SetActive(active);
    }

    [ContextMenu("Initialize Handles Curved")]
    public void InitializeHandlesCurved()
    {
        if (p0 != null && p2 != null && handle20 != null && handle50 != null && handle80 != null)
        {
            Vector3 startPos = p0.position;
            Vector3 endPos = p2.position;

            // 基于二次抛物线规律 4*t*(1-t) 计算下垂权重，让管线呈现自然的U型
            handle20.position = Vector3.Lerp(startPos, endPos, 0.2f) + defaultCurveOffset * 0.64f;
            handle50.position = Vector3.Lerp(startPos, endPos, 0.5f) + defaultCurveOffset * 1.00f;
            handle80.position = Vector3.Lerp(startPos, endPos, 0.8f) + defaultCurveOffset * 0.64f;
        }
    }

    // --- 注册连接数据到 SimulationLoop ---
    private void RegisterConnection()
    {
        if (SimulationLoop.Instance == null || p0 == null || p2 == null) return;

        PortBase portA = p0.GetComponent<PortBase>();
        PortBase portB = p2.GetComponent<PortBase>();

        if (portA != null && portB != null)
        {
            // 缓存数据，注销时即使 p0/p2 已销毁也能通过 FindObjectsByType 查到
            _wirePortA = portA.portandvalve;
            _wirePortB = portB.portandvalve;
            _wireBaseA = portA;
            _wireBaseB = portB;

            SimulationLoop.Instance.RegisterWire(portA, portB);
            isConnectionRegistered = true;
        }
        else
        {
            Debug.LogWarning("管线两端的接口缺少 PortBase 组件，无法注册线缆！");
        }
    }

    // --- 从 SimulationLoop 注销连接数据 ---
    private void UnregisterConnection()
    {
        if (isConnectionRegistered && SimulationLoop.Instance != null)
        {
            // 用缓存的 PortBase 引用注销（包含唯一实例信息）
            SimulationLoop.Instance.UnregisterWire(_wireBaseA, _wireBaseB);
            isConnectionRegistered = false;
            _wireBaseA = null;
            _wireBaseB = null;
        }
    }
}