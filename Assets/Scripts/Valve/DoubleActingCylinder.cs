using DG.Tweening;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// 工业级双作用气缸（物理解算与渲染动画完全解耦版）
/// </summary>
public class DoubleActingCylinder : BaseValve
{
    [Header("机械结构参数")]
    public Transform pistonRod;
    public float maxStroke = 0.5f;
    public float minDuration = 0.5f;  // 满流速1.0时的最快时间
    public float maxDuration = 10.0f; // 流速极低时的最慢时间（防抖用）
    public float currentPos = 0f;

    [Header("音效与信号")]
    public AudioSource audioSource;
    public AudioClip extendSound;
    public AudioClip retractSound;
    public CylinderSignalSource cySignal1, cySignal2;

    // ==========================================
    // 🔴 核心解耦看板：物理与渲染的“中间桥梁”
    // ==========================================
    public float targetDirection = 0f; // 本帧因果迭代最终沉淀出的运动意图 (1:伸出, -1:缩回, 0:不动)
    public float targetFlowRate = 0f;  // 本帧因果迭代最终沉淀出的卡脖子有效流速

    private float lastPlayingDirection = 0f;
    private float lastPlayingFlowRate = -1f;
    private Tweener moveTweener;

    protected override void Start()
    {
        base.Start();
        ports[0].InFlow = 1;
        ports[1].InFlow = 1;
        ports[0].outFlow = 1;
        ports[1].outFlow = 1;
        // DisconnectInternalTopology();
    }

    /// <summary>
    /// 阶段 1：【只谈物理，不谈动画】
    /// 负责在 10 次迭代中计算出本帧确定性的运动方向、捕获流速，并向外推高压
    /// </summary>
    public override void ProcessLogic()
    {
        if (ports.Count < 2) return;

        PneumaticPort portA = ports[0]; // 后腔 (左)
        PneumaticPort portB = ports[1]; // 前腔 (右 - 连分流三通)

        portA.state = PortState.Conduct;
        portB.state = PortState.Conduct;

        float pA = portA.exPressure;
        float pB = portB.exPressure;

        // 1. 纯物理意图与流速判定
        if (pA > pB + 0.1f && currentPos < maxStroke)
        {
            targetDirection = 1f; // 伸出
                                  // 🔴 【核心修改】：双边流量因果制约！

        }
        else if (pB > pA + 0.1f && currentPos > 0f)
        {
            targetDirection = -1f; // 缩回

        }
        else
        {
            targetDirection = 0f;
            targetFlowRate = 1f;
        }


        // 2. 依据物理意图装配内部拓扑连接，向网络宣泄能量
        if (targetDirection != 0)
        {
            if (targetDirection == 1)
            {
                // 【伸出】：A进气，B排气。
                // 仅让接收端 B 的内指针连向送气端 A。B 会自发从 A 身上剥离高压
                // 接收端 B 绝对不会污染或影响送气端 A 的数据
                portA.internalConnectTo = null;
                portB.internalConnectTo = portA;
                targetFlowRate = Mathf.Min(portA.inFlowRate, portB.inFlowRate);
                portB.ReceiveInternalInfo(0.5f);
            }
            else
            {
                // 【缩回】：B进气，A排气。
                // 仅让排气端 A 的内指针单向连向进气端 B
                portB.internalConnectTo = null;
                portA.internalConnectTo = portB;
                targetFlowRate = Mathf.Min(portA.outFlowRate, portB.outFlowRate);
                portA.ReceiveInternalInfo(0.5f);
            }

            // 🔴 驱动无参数调用！
            // 此时由于单向指针已绑好，排气口会自动顺着 internalConnectTo 把进气口的高压咬过来！
            // 彻底消灭了原先硬编码的 portB.pressure = pA; 手动强写！                       
            // 🔴 同理，缩回时的流速同样采取双边取极小值
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
    /// </summary>
    private void Update()
    {
        // 驱动动画状态机控制器
        RenderCylinderAnimation(targetDirection, targetFlowRate);
    }

    /// <summary>
    /// 核心渲染行动机：完美隔离在主计算循环之外，绝对不会引发数值震荡！
    /// </summary>
    private void RenderCylinderAnimation(float direction, float flow)
    {
        // 栅栏防御：如果最终沉淀下来的物理数据没有发生本质跃迁，维持现有的 Tween 动画平稳播放，拒绝闪烁重启
        if (direction == lastPlayingDirection && Mathf.Abs(flow - lastPlayingFlowRate) < 0.01f) return;

        lastPlayingDirection = direction;
        lastPlayingFlowRate = flow;

        moveTweener?.Kill(); // 只有当快慢速真的切换、或者停死时，才安全切换动画速度

        if (direction == 0 || flow <= 0.001f)
        {
            PlayActionSound(0);
            return;
        }

        float targetStrokePos = (direction == 1) ? maxStroke : 0f;
        float remainingDistance = Mathf.Abs(targetStrokePos - currentPos);
        float progressPercent = remainingDistance / maxStroke;

        // 套用非线性物理调速公式计算所需时间
        float calcDuration = Mathf.Lerp(maxDuration, minDuration, flow) * progressPercent;

        PlayActionSound(direction);

        moveTweener = DOTween.To(() => currentPos, x =>
        {
            currentPos = x;
            if (pistonRod != null) pistonRod.localPosition = new Vector3(currentPos, 0, 0);
            //CheckSignals(); // 沿途触发磁性行程开关
        }, targetStrokePos, calcDuration)
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
        if (direction == 1 && extendSound != null) { audioSource.clip = extendSound; audioSource.Play(); }
        else if (direction == -1 && retractSound != null) { audioSource.clip = retractSound; audioSource.Play(); }
    }
}