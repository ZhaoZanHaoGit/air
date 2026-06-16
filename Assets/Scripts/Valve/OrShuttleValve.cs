using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 工业级气动或门梭阀（纯拓扑指针改绑 + 压力自收敛版）
/// 端口索引约定：
/// - ports[0]: A口 (输入端1) -> Input
/// - ports[1]: B口 (输入端2) -> Input
/// - ports[2]: Y口 (工作输出端) -> Output
/// </summary>
public class OrShuttleValve : BaseValve
{
    [Header("抗扰动微调")]
    [Tooltip("触发内部滑块切变所需的最低有效气压阈值，低于此值视为管路噪声微弱残压")]
    public float pTriggerThreshold = 0.15f;

    [Header("实时状态监测看板（只读）")]
    [Tooltip("当前哪一路输入胜出并主导了输出通路")]
    public string activeChannel = "None";

    /// <summary>
    /// 阶段 1：【纯拓扑算命】动态改绑外部连线指针，让压力顺着图论网格自发渗透
    /// </summary>
    public override void ProcessLogic()
    {
        if (ports.Count < 3) return;

        PneumaticPort portA = ports[0]; // 输入 A
        PneumaticPort portB = ports[1]; // 输入 B
        PneumaticPort portY = ports[2]; // 输出 Y

        // 强行确保梭阀的所有工作物理接触面处于正常导通极性
   
        portY.state = PortState.Conduct;

        // ==========================================
        // 1. 或门逻辑判定：加入相对压差抗扰动机制
        // ==========================================
        float pA = portA.pressure;
        float pB = portB.pressure;

        if (pA > pB + pTriggerThreshold)
        {
            // --- A 口高压胜出 ---
            activeChannel = "Channel_A";

            // 🔴 纯拓扑指针改绑：将输出口 Y 的外部连线终点，死死绑在源头输入口 A 上！
            // 这样下游网络（如气缸）在阶段 2 和阶段 3 运行时，会自发顺着指针咬到 A 口的纯净高压。
            portY.connectedTo = portA;
            portA.state = PortState.Conduct;
            portB.state = PortState.CutOff;
        }
        else if (pB > pA + pTriggerThreshold)
        {
            // --- B 口高压胜出 ---
            activeChannel = "Channel_B";

            // 🔴 纯拓扑指针改绑：将输出口 Y 的外部连线终点，死死绑在源头输入口 B 上！
            portY.connectedTo = portB; 
            portA.state = PortState.CutOff; 
            portB.state = PortState.Conduct;
        }
        else
        {
            // --- 两端都没有气，或者两端压差极其微弱（处于死区噪声范围内） ---
            // 此时梭阀内部阀芯维持原位，或退守到双边平衡状态。
            // 为了防止悬空引发空指针，如果两端确实有基础对等压力，我们让它默认继承较高的一侧；如果全为0，则断开连接。
            if (pA > 0.1f || pB > 0.1f)
            {
                portY.connectedTo = (pA >= pB) ? portA : portB;
                activeChannel = (pA >= pB) ? "Channel_A (Balanced)" : "Channel_B (Balanced)";
            }
            else
            {
                portY.connectedTo = null; // 彻底断开，防止无压力时的残留穿透
                activeChannel = "None (DeadZone)";
            }
        }

        // ==========================================
        // 2. 拓扑因果自激活：驱动统一无参数重载接口演化
        // ==========================================
        // 外部公开字段被调用后，数据在全局 SimulationLoop 的多轮迭代中无损传递
        portA.ReceiveInternalInfo();
        portB.ReceiveInternalInfo();
        portY.ReceiveInternalInfo();
    }
}