using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SequenceCheckValve : BaseValve
{
    [Range(0.01f, 1f)]
    [Header("阀门开启限制")]
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

        
     

        float pP = portP.exPressure; // 从外部管线读到的P侧压力
        float pA = portA.exPressure; // 从外部管线读到的A侧压力

        if (pP > pA + 0.001f)
        {
            if (pP > opening)
            {
                portP.state = PortState.Conduct;
                portA.state = PortState.Conduct;
                portP.ReceiveInternalInfo(1f, 1f, 1f);
                portA.ReceiveInternalInfo(1f, 1f, 1f);
            }
            else
            {

                portP.state = PortState.CutOff;
                portA.state = PortState.CutOff;
            }
           
        }
        else if (pA > pP + 0.001f)
        {
            portP.state = PortState.Conduct;
            portA.state = PortState.Conduct;
            // ====================================================
            // 【反向：A → P】 气经过内置单向阀旁路，不受节流限制
            // ====================================================
            // 单向阀全开，inFlow/outFlow 均为 1，不施加任何速率限制
            portP.ReceiveInternalInfo(1f, 1f, 1f);
            portA.ReceiveInternalInfo(1f, 1f, 1f);
        }
        else
        {
            portP.state = PortState.Conduct;
            portA.state = PortState.Conduct;
            // ====================================================
            // 【静态平衡 / 保压】 两端压力相等，无净流动
            // ====================================================
            // 保持当前压力，不施加流量限制
            portP.ReceiveInternalInfo(1f, 1f, 1f);
            portA.ReceiveInternalInfo(1f, 1f, 1f);
        }
    }
}
