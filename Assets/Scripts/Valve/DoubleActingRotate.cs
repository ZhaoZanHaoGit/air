using DG.Tweening;
using UnityEngine;

/// <summary>
/// 工业级双作用旋转气缸（物理解算与渲染动画完全解耦版）
/// 端口排列（Inspector中按顺序配置 2 个 PneumaticPort）：
///   [0] A  气口A（正转进气口）
///   [1] B  气口B（反转进气口）
///
/// 工作逻辑：
///   A口进气压力 > B口压力：气缸正转（顺时针/逆时针，取决于安装方向）
///   B口进气压力 > A口压力：气缸反转（回到初始位置）
///   A/B口压力相等：保持在当前位置
///
/// 消噪滤波：
///   与 DoubleActingCylinder 逻辑一致，防止 SimulationLoop 重置时
///   瞬间断开导致误判运动方向。
/// </summary>
public class DoubleActingRotate : BaseValve
{
    [Header("机械结构参数")]
    public Transform rotateBody;          // 旋转的气缸 body（实际旋转的部分）
    public float maxAngle = 90.0f;      // 最大旋转角度（度）
    public float minDuration = 0.5f;    // 满流速1.0时的最快旋转时间（秒）
    public float maxDuration = 10.0f;    // 流速极低时的最慢旋转时间（秒）（防抖用）
    public float currentAngle = 0f;      // 当前旋转角度（度，0~maxAngle）

    [Header("旋转轴设置")]
    public Vector3 rotateAxis = Vector3.forward;  // 旋转轴（本地坐标系，forward=Z轴，用于2D俯视视角）

    [Header("音效与信号")]
    public AudioSource audioSource;
    public AudioClip rotateForwardSound;   // 正转音效
    public AudioClip rotateBackwardSound;  // 反转音效
    public CylinderSignalSource cySignal1, cySignal2;  // 磁性行程开关信号

    [Header("消噪滤波设置")]
    [Tooltip("气压信号断开时的消噪延时(秒)，用于过滤 SimulationLoop 重置等瞬间断开带来的气压震荡")]
    public float debounceDuration = 0.05f;

    // ==========================================
    // 🔴 核心解耦看板：物理与渲染的"中间桥梁"
    // ==========================================
    public float targetDirection = 0f;    // 本帧因果迭代最终沉淀出的运动意图 (1:正转, -1:反转, 0:不动)
    public float targetFlowRate = 0f;     // 本帧因果迭代最终沉淀出的卡脖子有效流速

    // 消噪滤波器内部状态
    private float filterTimerForward = 0f;  // 正转消噪定时器
    private float filterTimerBackward = 0f; // 反转消噪定时器
    private bool rawTargetForward = false;    // 原始正转信号
    private bool rawTargetBackward = false;   // 原始反转信号
    private int filteredDirection = 0;       // 消噪后的稳定方向

    private float lastPlayingDirection = 0f;
    private float lastPlayingFlowRate = -1f;
    private Tweener rotateTweener;

    protected override void Start()
    {
        base.Start();
        if (ports.Count >= 2)
        {
            ports[0].InFlow = 1;
            ports[1].InFlow = 1;
            ports[0].outFlow = 1;
            ports[1].outFlow = 1;
        }

        // 初始化当前角度（从实际 Transform 读取，支持场景初始状态）
        if (rotateBody != null)
        {
            // 尝试从当前旋转反推 currentAngle
            // 假设初始旋转为 0，目标旋转为 maxAngle
            float angle = Vector3.Angle(rotateBody.localRotation * Vector3.forward, rotateAxis);
            currentAngle = Mathf.Clamp(angle, 0f, maxAngle);
        }
    }

    /// <summary>
    /// 阶段 1：【只谈物理，不谈动画】
    /// 负责在迭代中计算出本帧确定性的运动方向、捕获流速，并向外推高压
    /// </summary>
    public override void ProcessLogic()
    {
        if (ports.Count < 2) return;

        PneumaticPort portA = ports[0]; // A口（正转进气）
        PneumaticPort portB = ports[1]; // B口（反转进气）

        portA.state = PortState.Conduct;
        portB.state = PortState.Conduct;

        float pA = portA.exPressure;
        float pB = portB.exPressure;

        // 1. 纯物理意图判定（使用 exPressure = 外部管线压力，已经包含节流阀约束的上一帧结果）
        if (pA > pB + 0.01f && currentAngle < maxAngle)
        {
            targetDirection = 1f;  // 正转：A腔进气，B腔排气
        }
        else if (pB > pA + 0.01f && currentAngle > 0f)
        {
            targetDirection = -1f; // 反转：B腔进气，A腔排气
        }
        else
        {
            targetDirection = 0f;
            targetFlowRate = 1f;
        }

        // 2. 依据物理意图装配内部拓扑连接，向网络传递压力
        if (targetDirection != 0)
        {
            if (targetDirection == 1)
            {
                // 【正转】：A口进气（高压推旋转），B口排气（低压释放）
                portA.internalConnectTo = null;
                portB.internalConnectTo = portA;

                // 流量应读 exFlowRate（已被节流阀写入的外部流量系数）
                targetFlowRate = Mathf.Min(portA.inExFlow, portB.outExFlow);
                portB.ReceiveInternalInfo(0.5f);
            }
            else
            {
                // 【反转】：B口进气（高压推旋转反转），A口排气
                portB.internalConnectTo = null;
                portA.internalConnectTo = portB;

                targetFlowRate = Mathf.Min(portB.inExFlow, portA.outExFlow);
                portA.ReceiveInternalInfo(0.8f);
            }
        }
        else
        {
            DisconnectInternalTopology();
        }
    }

    /// <summary>
    /// 🔴 原生 Update()：【只谈动画，不谈物理】
    /// 此时 SimulationLoop 的多轮级联迭代已经全部完结，数值已经绝对稳定。
    /// 气缸走过来收割物理快照，平滑更新视觉层。
    /// 
    /// 消噪滤波：防止 SimulationLoop 重置时瞬间断开导致误判运动方向
    /// </summary>
    private void Update()
    {
        // ============================================================
        // 消噪滤波器：与 ManualValve32 / PilotValve52 逻辑完全一致
        // 原理：
        //   信号出现（targetDirection != 0）→ 立即生效，定时器满格重置
        //   信号消失（targetDirection == 0）→ 不立即失效，定时器倒计时
        //   定时器到期后 → 才确认"真的消失了"
        // ============================================================

        // --- 正转信号消噪滤波器 ---
        if (targetDirection == 1)
        {
            // 正转信号出现：立即生效，重置定时器
            rawTargetForward = true;
            filterTimerForward = debounceDuration;
        }
        else
        {
            // 正转信号消失：启动消噪倒计时
            if (filterTimerForward > 0f)
            {
                filterTimerForward -= Time.deltaTime;
            }
            else
            {
                // 倒计时结束，确认正转信号真正消失
                rawTargetForward = false;
            }
        }

        // --- 反转信号消噪滤波器 ---
        if (targetDirection == -1)
        {
            // 反转信号出现：立即生效，重置定时器
            rawTargetBackward = true;
            filterTimerBackward = debounceDuration;
        }
        else
        {
            // 反转信号消失：启动消噪倒计时
            if (filterTimerBackward > 0f)
            {
                filterTimerBackward -= Time.deltaTime;
            }
            else
            {
                // 倒计时结束，确认反转信号真正消失
                rawTargetBackward = false;
            }
        }

        // --- 根据消噪后的稳定信号决定最终方向 ---
        if (rawTargetForward && !rawTargetBackward)
            filteredDirection = 1;    // 正转
        else if (rawTargetBackward && !rawTargetForward)
            filteredDirection = -1;   // 反转
        else
            filteredDirection = 0;    // 停止（中位）

        // 驱动动画状态机控制器（使用消噪后的稳定方向）
        RenderRotateAnimation(filteredDirection, targetFlowRate);
    }

    /// <summary>
    /// 核心渲染行动机：完美隔离在主计算循环之外，绝对不会引发数值震荡！
    /// </summary>
    private void RenderRotateAnimation(float direction, float flow)
    {
        // 栅栏防御：如果最终沉淀下来的物理数据没有发生本质跃迁，维持现有的 Tween 动画平稳播放，拒绝闪烁重启
        if (direction == lastPlayingDirection && Mathf.Abs(flow - lastPlayingFlowRate) < 0.01f) return;

        lastPlayingDirection = direction;
        lastPlayingFlowRate = flow;

        rotateTweener?.Kill(); // 只有当快慢速真的切换、或者停死时，才安全切换动画速度

        if (direction == 0 || flow <= 0.001f)
        {
            PlayActionSound(0);
            return;
        }

        float targetAngle = (direction == 1) ? maxAngle : 0f;
        float remainingAngle = Mathf.Abs(targetAngle - currentAngle);
        float progressPercent = remainingAngle / maxAngle;

        // 套用非线性物理调速公式计算所需时间
        float calcDuration = Mathf.Lerp(maxDuration, minDuration, flow) * progressPercent;

        PlayActionSound(direction);

        rotateTweener = DOTween.To(() => currentAngle, x =>
        {
            currentAngle = x;
            if (rotateBody != null)
            {
                // 应用旋转（本地坐标系）
                rotateBody.localRotation = Quaternion.AngleAxis(currentAngle, rotateAxis);
            }
            // CheckSignals(); // 沿途触发磁性行程开关
        }, targetAngle, calcDuration)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            // 行程结束，在随后的物理帧里自发断开
            targetDirection = 0f;
            targetFlowRate = 0f;
        });
    }

    private void DisconnectInternalTopology()
    {
        if (ports.Count < 2) return;
        ports[0].internalConnectTo = null;
        ports[1].internalConnectTo = null;
    }

    private void PlayActionSound(float direction)
    {
        if (audioSource == null) return;
        audioSource.Stop();
        if (direction == 1 && rotateForwardSound != null)
        {
            audioSource.clip = rotateForwardSound;
            audioSource.Play();
        }
        else if (direction == -1 && rotateBackwardSound != null)
        {
            audioSource.clip = rotateBackwardSound;
            audioSource.Play();
        }
    }

 
}
