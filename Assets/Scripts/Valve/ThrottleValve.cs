using UnityEngine;

/// <summary>
/// 可调单向节流阀 (One-Way Flow Control Valve)
/// 
/// 物理行为：
///   正向 (P → A)：气流经过节流芯，速度受 opening 限制（慢速）
///   反向 (A → P)：气流经过内置单向阀旁路，不受节流限制（全速）
/// 
/// 端口配置约定：
///   ports[0]: P口 (进气/出气端，正向进气侧)
///   ports[1]: A口 (出气/进气端，连向气缸腔室侧)
/// 
/// 典型用法（出口节流控制气缸伸出速度）：
///   主气源 → 换向阀 → 节流阀P口 → 节流阀A口 → 气缸A腔
///   气缸A腔排气时(A→P方向)：单向阀开，全速排气
///   气缸A腔进气时(P→A方向)：节流芯节流，慢速进气
/// </summary>
public class ThrottleValve : BaseValve
{
    [Range(0.01f, 1f)]
    [Header("节流开口度 (1=全开/全速, 0.01=几乎关死/极慢)")]
    public float opening = 0.5f;

    protected override void Start()
    {
        base.Start();

        if (ports.Count >= 2)
        {
            // 节流阀内部通道永久双向连通（无论正反向气都能过）
            // 速度差异由 ProcessLogic 中的 flowRate 参数控制
            ports[0].internalConnectTo = ports[1]; // P ↔ A
            ports[1].internalConnectTo = ports[0]; // A ↔ P
        }
    }

    /// <summary>
    /// 阶段1：根据两端压差判断气流方向，动态分配节流速率
    /// 
    /// ReceiveInternalInfo(pressurePercent, inFlowPercent, outFlowPercent) 参数说明：
    ///   - pressurePercent: 从 internalConnectTo 端口获取压力时的缩放比（减压阀用）
    ///   - inFlowPercent:   从内部通道"流入本端口方向"的速率系数
    ///   - outFlowPercent:  从本端口"流向外部管路方向"的速率系数
    /// 
    /// 单向节流阀规则：
    ///   正向 P→A（pP > pA）：A口是接收高压气的一侧
    ///     → A口的 inFlow 被 opening 限速（从P侧吸进来的气被卡慢）
    ///     → P口的 outFlow 被 opening 限速（往A侧推出的气被卡慢）
    ///   反向 A→P（pA > pP）：P口是接收高压气的一侧（排气通过单向阀旁路）
    ///     → 不限速，inFlow/outFlow 均为 1
    /// </summary>
    public override void ProcessLogic()
    {
        if (ports.Count < 2) return;

        PneumaticPort portP = ports[0]; // P口
        PneumaticPort portA = ports[1]; // A口

        // 节流阀两端始终导通（截止由上游换向阀控制）
        portP.state = PortState.Conduct;
        portA.state = PortState.Conduct;

        float pP = portP.exPressure; // 从外部管线读到的P侧压力
        float pA = portA.exPressure; // 从外部管线读到的A侧压力

        if (pP > pA + 0.001f)
        {
            // ====================================================
            // 【正向：P → A】 气通过节流芯，速度受 opening 限制
            // ====================================================
            // P口：正在往A口推气，其"流出到外部管路"的速率被限 → outFlow = opening
            //      从A口回来的内压直接读取（压力不衰减）
            portP.ReceiveInternalInfo(1f, 1f, opening);

            // A口：正在从P口接收气，其"从内部通道流入"的速率被限 → inFlow = opening
            //      向外部管路输出时速率同样被限（背压端也卡慢，避免跑压）
            portA.ReceiveInternalInfo(1f, opening, opening);
        }
        else if (pA > pP + 0.001f)
        {
            // ====================================================
            // 【反向：A → P】 气经过内置单向阀旁路，不受节流限制
            // ====================================================
            // 单向阀全开，inFlow/outFlow 均为 1，不施加任何速率限制
            portP.ReceiveInternalInfo(1f, 1f, 1f);
            portA.ReceiveInternalInfo(1f, 1f, 1f);
        }
        else
        {
            // ====================================================
            // 【静态平衡 / 保压】 两端压力相等，无净流动
            // ====================================================
            // 保持当前压力，不施加流量限制
            portP.ReceiveInternalInfo(1f, 1f, 1f);
            portA.ReceiveInternalInfo(1f, 1f, 1f);
        }
    }
}
