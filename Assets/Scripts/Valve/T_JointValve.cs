using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 工业通用分流/汇流三通（纯拓扑指针 + 快慢速自适应演化版）
/// </summary>
public class T_JointValve : BaseValve
{
    // 三通内部唯一的、不对外的虚拟核心交汇总枢纽
    private PneumaticPort coreHub;

    protected override void Start()
    {
        base.Start();

        // 1. 动态在三通内部生成这个不参与界面连线的虚拟核心
        GameObject hubObj = new GameObject("T_CoreHub_Virtual");
        hubObj.transform.SetParent(this.transform);
        coreHub = hubObj.AddComponent<PneumaticPort>();
        coreHub.state = PortState.Conduct;
        

        // 2. 【纯拓扑绑定】：让三通的 3 个物理面，在阀体内全部指向这个核心
        foreach (var port in ports)
        {
            port.state = PortState.Conduct;
            port.internalConnectTo = coreHub; // 它们的内滑道终点都是核心枢纽
        }
    }

    /// <summary>
    /// 阶段 1：核心纽带进行自适应网格松弛，随后外部端口自发索要数值
    /// </summary>
    public override void ProcessLogic()
    {
        if (ports.Count < 3 || coreHub == null) return;

        // ==========================================
        // 步骤 1：核心总枢纽睁开眼，汇总所有活跃端口的压力（前向推力）
        // ==========================================
        float maxP = 0f;
        foreach (var port in ports)
        {
            // 只有处于活跃导通（Conduct）的管路，它的压力才能渗透进三通核心
            if (port.state == PortState.Conduct && port.exPressure > maxP)
            {
                maxP = port.exPressure;
            }
        };
        coreHub.pressure = maxP;
        // 🔴 修复：三通传递流量约束应取 Min（保留最窄瓶颈），而非 Max（Max 会绕过节流阀限速）
        // 例：换向阀出口(flow=1) → 三通 → 分支A(节流阀,flow=0.3) + 分支B(直通,flow=1)
        // ports[0] 汇总两个出口的瓶颈：Min(0.3, 1) = 0.3，正确反映主路被节流阀约束
        ports[0].ReceiveInternalInfo(1, Mathf.Max(ports[1].inExFlow, ports[2].inExFlow), Mathf.Max(ports[1].outExFlow, ports[2].outExFlow));
        ports[1].ReceiveInternalInfo(1, Mathf.Max(ports[0].inExFlow, ports[2].inExFlow), Mathf.Max(ports[0].outExFlow, ports[2].outExFlow));
        ports[2].ReceiveInternalInfo(1, Mathf.Max(ports[1].inExFlow, ports[0].inExFlow), Mathf.Max(ports[1].outExFlow, ports[0].outExFlow));
        /*
        // ==========================================
        // 步骤 2：核心总枢纽汇总所有活跃下游分支的流量限制（后向憋流）
        // ==========================================
        float maxinFlow = 0f;
        foreach (var port in ports)
        {
            // 只有处于活跃导通（Conduct）的管路，它的压力才能渗透进三通核心
            if (port.state == PortState.Conduct && port.inExFlow > maxinFlow)
            {
                maxP = port.inExFlow;
            }
        }
        float maxoutFlow = 0f;
        foreach (var port in ports)
        {
            // 只有处于活跃导通（Conduct）的管路，它的压力才能渗透进三通核心
            if (port.state == PortState.Conduct && port.outExFlow > maxinFlow)
            {
                maxP = port.outExFlow;
            }
        }


        // ==========================================
        // 步骤 3：核心总枢纽自我更新，随后驱动端口通过指针索要数据
        // ==========================================
        coreHub.pressure = maxP;
        coreHub.inFlowRate = maxinFlow;
        coreHub.outFlowRate = maxoutFlow;

        // 3个物理表面执行无参数调用，自发顺着 internalConnectTo (即coreHub) 咬下数据
        foreach (var port in ports)
        {
            if (port.state == PortState.Conduct)
            {
                port.ReceiveInternalInfo();
            }
        }*/
    }
}