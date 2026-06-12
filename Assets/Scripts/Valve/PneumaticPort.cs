using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum PortType { Output, Input, Free, main, close }
public enum PortState
{
    Conduct, // 导通：正常传递压力和流量
    CutOff   // 截止：无法传递压力和流量（相当于原行程阀关闭时的断开状态）
}

public class PneumaticPort : PortBase
{
    [Header("端口状态控制")]

    public PortState state = PortState.Conduct;
    public PortType portType = PortType.Free;
    public BaseValve parentValve;     // 所属阀门

    [Header("实时流体数据")]
    public float pressure = 0f;
    public float inFlowRate = 1.0f;
    public float outFlowRate = 1.0f;
    [Header("拓扑物理连接")]
    public PneumaticPort connectedTo; // 依然保持手拉手的连线指针

    [Header("阀门内部动态通道")]
    public PneumaticPort internalConnectTo; // 【你的灵感】：连向同一个阀门内部的另一个端口！

    // 缓存隔离区
    public float inPressure, InFlow, outFlow;
    public float exPressure, inExFlow, outExFlow;
    public void ClearPressureState()
    {
        inPressure = 0f;
        exPressure = 0f; 
        pressure = 0f;

        inExFlow=1f; 
        outExFlow=1f;
        InFlow = 1f;
        outFlow = 1f;
        inFlowRate = 1f;
        outFlowRate = 1f;
    }

    /// <summary>
    /// 步骤 1：接收【阀门内部连接对端】通过内滑道传过来的流体信息
    /// </summary>
    public void ReceiveInternalInfo(float Pressurepercent = 1, float inFlowpercent = 1, float outFlowpercent = 1)
    {
        if (internalConnectTo != null)
        {
            inPressure = internalConnectTo.pressure*Pressurepercent ;
            InFlow     =inFlowpercent;
            outFlow    =outFlowpercent;
        }
        else
        {
            inPressure = 0;
            InFlow = 1;
            outFlow = 1;

        }

    }
   

    /// <summary>
    /// 步骤 2：接收【管线外部连接对端】传过来的流体信息（保持原样）
    /// </summary>
    public void ReceiveExternalInfo()
    {
        if (debugOn)
        {
            Debug.Log($"Port {name}");
        }
        if (connectedTo != null)
        {
            exPressure = connectedTo.pressure;
            inExFlow = connectedTo.inFlowRate;
            outExFlow = connectedTo.outFlowRate;
        }
        else
        {
            exPressure = 0f;
            inExFlow = 1;
            outExFlow = 1;
        }
        if (debugOn)
        { Debug.Log($"Port {name}: External info received. Pressure: {exPressure}"); }
    }

    /// <summary>
    /// 步骤 3：融合内外两端，由压差 ΔP 拍板决定流向并输出
    /// </summary>
    public void IntegrateAndOutput()
    {
        if (isMain)
        {
            state = PortState.Conduct;
            pressure = 1f;
            outFlowRate = 1f;
            return;
        }
        if (state == PortState.CutOff) { pressure = 0f; inFlowRate = 0f; outFlowRate = 0f; return; }
        inFlowRate = Mathf.Min(inExFlow, InFlow);
        outFlowRate = Mathf.Min(outExFlow, outFlow);
        // 依据内压和外压的绝对压差判定确定的瞬时流向
        if (inPressure > exPressure + 0.001f)
        {
            // 【流向：内部通道 -> 外部管路】（气从阀内往外喷）
            pressure = inPressure;

        }
        else if (exPressure > inPressure + 0.001f)
        {
            // 【流向：外部管路 -> 内部通道】（气从外管路往阀内灌）
            pressure = exPressure;

        }
        else
        {
            pressure = (inPressure + exPressure) / 2;
        }

    }

    // 判断该端口是否属于未接线暴露在空气中的漏气点
    public bool IsLeaking()
    {
        return state == PortState.Conduct && connectedTo == null;
    }
    public override void OnConnect(PortBase other)
    {
        if (other is PneumaticPort otherPort)
        {
            isOccupied = true;
            this.connectedTo = otherPort;

        }
    }
    public override void AlwaysON()
    {
        pressure = 1.0f;            // 压力 (0-1)
        outFlowRate = 1.0f;
    }
    public override void Disconnect()
    {
        isOccupied = false;
        this.connectedTo = null;

    }
    // 将本端压力和流量同步给连线对端
    public override void Propagate()
    {

        /*if (connectedTo == null) return;

        // 1. 处理主气源 (main) 的特殊性
        if (connectedTo.portType == PortType.main)
        {
            this.pressure = connectedTo.pressure;
            return;
        }
        if (this.portType == PortType.main)
        {
            connectedTo.pressure = this.pressure;
            return;
        }

        // 2. 核心：如果我是 Output，我强行把我的状态同步给对方
        // 无论我是 0.8 还是 0，对方（Input 或 Free）都必须听我的
        if (this.portType == PortType.Output)
        {
            connectedTo.pressure = this.pressure;
            connectedTo.flowRate = this.flowRate;
        }
        // 3. 如果我是 Input，我只接收，不主动传播（除非对方是 Free）
        else if (this.portType == PortType.Input)
        {
            // 保持静默
        }
        // 4. 如果我是 Free（导线/三通），执行平衡逻辑
        else if (this.portType == PortType.Free)
        {
            if (connectedTo.portType == PortType.Free)
            {
                float targetP = Mathf.Max(this.pressure, connectedTo.pressure);
                this.pressure = targetP;
                connectedTo.pressure = targetP;
                float targetFlow = Mathf.Min(this.flowRate, connectedTo.flowRate);
                this.flowRate = targetFlow;
                connectedTo.flowRate = targetFlow;
            }
            else if (connectedTo.portType == PortType.Input)
            {
                connectedTo.pressure = this.pressure;
                connectedTo.flowRate = this.flowRate;
            }


        }*/
    }
    // 在 PneumaticPort.cs 中加入
    private void OnDrawGizmos()
    {
        // 有压变红，无压变白
        // Gizmos.color = pressure > 0.1f ? Color.red : Color.white;
        //Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
