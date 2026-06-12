using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// 可调单向节流阀 (One-Way Flow Control Valve)
/// 端口配置约定：
/// - ports[0]: P口 (进气端) -> 必须设为 PortType.Input
/// - ports[1]: A口 (出气端) -> 必须设为 PortType.Output
/// </summary>
public class ThrottleValve : BaseValve
{
    [Range(0.01f, 1f)]
    [Header("节流开口度")]
    public float opening = 0.5f; // 旋钮开口度，越小阻力越大，气缸越慢
    protected override void Start()
    {
        base.Start(); // 执行基类的注册逻辑

        if (ports.Count >= 2)
        {
            // 🔴 节流阀在物理上是永久双向连通的。
            // 我们在初始化时，就直接让 P口 和 A口 在内部“小手拉大手”搭建好高速路。
            ports[0].internalConnectTo = ports[1]; // P -> A
            ports[1].internalConnectTo = ports[0]; // A -> P
        }
    }
    /// <summary>
    /// 阶段 1：根据当前的绝对压差，动态调整滑道内部的阻力特征
    /// </summary>
    public override void ProcessLogic()
    {
        if (ports.Count < 2) return;

        PneumaticPort portP = ports[0]; // 左端 P 口
        PneumaticPort portA = ports[1]; // 右端 A 口

        // 强制确保两端处于激活导通态
        portP.state = PortState.Conduct;
        portA.state = PortState.Conduct;
        portP.ReceiveInternalInfo(1,opening, portA.outFlowRate);
        portA.ReceiveInternalInfo(1,opening, portA.outFlowRate);
        /*
        // ==========================================
        // 核心：基于压差驱动的内部流量约束分配
        // ==========================================
        if (portP.pressure > portA.pressure + 0.001f)
        {
            // --- 【流向：左(P) -> 右(A)】 伸出排气，单向阀堵死，节流阀芯卡脖子！ ---
            // 此时由于窄门限制，我们通知 P 口和 A 口：你们之间的内部通道变狭窄了！
            // 这样，在随后的“阶段 2”和“阶段 3”里，流经这条内部通道的最终流速会被强制锁死在 0.3f
            portP.ReceiveInternalInfo(1,opening);
            portA.ReceiveInternalInfo(1, opening);
        }
        else if (portA.pressure > portP.pressure + 0.001f)
        {
            // --- 【流向：右(A) -> 左(P)】 快退进气，内部单向阀被瞬间冲开！ ---
            // 阀门内部大门敞开，流量系数 100% 解除限制，恢复完全不设防的自由通过状态
            portP.ReceiveInternalInfo();
            portA.ReceiveInternalInfo(1,1);
        }
        else
        {/*
            // --- 静态平衡状态（保压） ---
            if (portP.pressure > 0.1f)
            {
                portP.ReceiveInternalInfo();
                portA.ReceiveInternalInfo();
            }
        }*/


    }
}