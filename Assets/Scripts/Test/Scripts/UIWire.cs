using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class UIWire : MaskableGraphic
{
    [Header("连接两端（RectTransform）")]
    public RectTransform StartRect;
    public RectTransform EndRect;

    [Header("外观")]
    [Min(2)] public int segments = 24;
    public float thickness = 6f;
    public float slack = 40f;
    public bool verticalSag = true;

    [Header("动态/弹性")]
    [Range(0.01f, 0.5f)] public float smoothTime = 0.08f;
    [Range(0f, 0.6f)] public float wobble = 0.18f;

    Canvas _canvas;
    Vector2 _controlOffset, _controlVel;
    Vector2 _prevA, _prevB;
    float _prevTime;

    // 在字段里加一个标记 & 可选上限
    bool _initialized;
    [SerializeField] float maxWobbleOffset = 60f; // wobble 贡献的最大偏移，按需调整
    protected override void OnEnable()
    {
        base.OnEnable();
        PrimeFirstFrame();       // 首帧预热
        SetVerticesDirty();
    }
    void PrimeFirstFrame()
    {
        if (StartRect == null || EndRect == null) return;

        Vector2 a = WorldToLocal(StartRect);
        Vector2 b = WorldToLocal(EndRect);
        Vector2 dir = (b - a);
        float dist = dir.magnitude;

        // 与你原有逻辑一致的“目标下垂方向”
        Vector2 baseDir = verticalSag
            ? Vector2.down
            : (dist > 1e-3f ? new Vector2(-dir.y, dir.x).normalized : Vector2.down);

        // 直接把控制点设到目标处（避免从 0 缓动导致首帧跳变）
        _controlOffset = baseDir * (slack + dist * 0.05f);

        _prevA = a;
        _prevB = b;
        _prevTime = Time.time;
        _initialized = true;
    }
    protected override void Awake()
    {
        base.Awake();
        _canvas = GetComponentInParent<Canvas>();
        _prevTime = Application.isPlaying ? Time.time : 0f;
    }

    void Update() => SetVerticesDirty();

    Camera UICamera
    {
        get
        {
            if (_canvas == null) _canvas = GetComponentInParent<Canvas>();
            if (_canvas == null) return null;
            if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;
            return _canvas.worldCamera;
        }
    }

    Vector2 WorldToLocal(RectTransform t)
    {
        if (t == null) return Vector2.zero;
        Vector3 world = t.transform.TransformPoint(Vector3.zero);
        Vector2 screen = RectTransformUtility.WorldToScreenPoint(UICamera, world);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screen, UICamera, out Vector2 local);
        return local;
    }

    static Vector2 BezierQuad(Vector2 a, Vector2 c, Vector2 b, float t)
    {
        var ab = Vector2.Lerp(a, c, t);
        var cb = Vector2.Lerp(c, b, t);
        return Vector2.Lerp(ab, cb, t);
    }

    static Vector2 BezierQuadTangent(Vector2 a, Vector2 c, Vector2 b, float t)
    {
        return 2f * (1f - t) * (c - a) + 2f * t * (b - c);
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (StartRect == null || EndRect == null || segments < 2 || thickness <= 0f)
            return;

        // 端点（本地坐标）
        Vector2 a = WorldToLocal(StartRect);
        Vector2 b = WorldToLocal(EndRect);

        // 友好的 dt（避免极小 dt 造成速度爆炸）
        float now = Application.isPlaying ? Time.time : 0f;
        float dt = Mathf.Max(1f / 60f, now - _prevTime);
        _prevTime = now;

        // 下垂方向 & 目标控制点偏移
        Vector2 dir = (b - a);
        float dist = dir.magnitude;
        Vector2 baseDir = verticalSag
            ? Vector2.down
            : (dist > 1e-3f ? new Vector2(-dir.y, dir.x).normalized : Vector2.down);

        Vector2 targetOffset = baseDir * (slack + dist * 0.05f);

        // 仅在已初始化后叠加“相对速度”造成的弹性抖动，并做幅度上限
        if (_initialized && wobble > 0f)
        {
            Vector2 velA = (a - _prevA) / dt;
            Vector2 velB = (b - _prevB) / dt;
            Vector2 relative = (velA - velB) * (wobble * 0.02f);
            if (maxWobbleOffset > 0f)
                relative = Vector2.ClampMagnitude(relative, maxWobbleOffset);
            targetOffset += relative;
        }

        _prevA = a; _prevB = b;

        // 平滑控制点
        _controlOffset = Vector2.SmoothDamp(_controlOffset, targetOffset, ref _controlVel, smoothTime);

        // 二次贝塞尔控制点
        Vector2 c = (a + b) * 0.5f + _controlOffset;

        float half = thickness * 0.5f;
        int pointCount = segments + 1;

        // 采样中心线、法线，并累计弧长（用于沿长 s 的 UV）
        var centers = new Vector2[pointCount];
        var normals = new Vector2[pointCount];
        var sAcc = new float[pointCount];

        float totalLen = 0f;
        Vector2 prevP = Vector2.zero;

        for (int i = 0; i < pointCount; i++)
        {
            float t = i / (float)segments;
            Vector2 p = BezierQuad(a, c, b, t);
            Vector2 tan = BezierQuadTangent(a, c, b, t);
            Vector2 n = new Vector2(-tan.y, tan.x);
            if (n.sqrMagnitude < 1e-6f) n = Vector2.up; else n.Normalize();

            centers[i] = p;
            normals[i] = n;

            if (i == 0)
            {
                sAcc[i] = 0f;
            }
            else
            {
                totalLen += Vector2.Distance(p, prevP);
                sAcc[i] = totalLen;
            }
            prevP = p;
        }
        if (totalLen < 1e-5f) totalLen = 1e-5f;

        // 写入顶点（左/右边界），UV：x=弧长归一化 s，y=0..1
        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        for (int i = 0; i < pointCount; i++)
        {
            float s = sAcc[i] / totalLen;

            Vector2 left = centers[i] - normals[i] * half;
            Vector2 right = centers[i] + normals[i] * half;

            vert.position = left;
            vert.uv0 = new Vector2(s, 0f);
            vh.AddVert(vert);

            vert.position = right;
            vert.uv0 = new Vector2(s, 1f);
            vh.AddVert(vert);
        }

        // 三角形条带
        for (int i = 0; i < segments; i++)
        {
            int i0 = i * 2;
            int i1 = i0 + 1;
            int i2 = i0 + 2;
            int i3 = i0 + 3;
            vh.AddTriangle(i0, i2, i1);
            vh.AddTriangle(i2, i3, i1);
        }
    }

}
