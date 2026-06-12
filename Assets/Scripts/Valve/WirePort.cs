using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WirePort : PortBase
{
    public PortType portType = PortType.Free;
    public BaseValve parentValve;     // 所属阀门
    [Header("实时状态")]
    public bool pressure;            // 通电状态 (0-1)
    public float volt = 1.0f;     // 流量系数 (0-1)
    public WirePort connectedTo; // 连向的对端端口
    public float MaxVolt = 24;

    private void Awake()
    {
        base.Awake();
        chanel = portChanel.pneumatic;
    }
    public override void OnConnect(PortBase other)
    {
        if (other is WirePort otherPort)
        {
            this.connectedTo = otherPort;
            isOccupied=true;
        }
    }
    public override void Disconnect()
    {
      
            this.connectedTo = null;
        isOccupied = false;

    }
    public override void AlwaysON()
    {
        pressure = true;
        volt = MaxVolt;
    }

    // 将本端压力和流量同步给连线对端
    public override void Propagate()
    {
        if (connectedTo == null) return;

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
            connectedTo.volt = this.volt;
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
                bool targetP = this.pressure || connectedTo.pressure; ;
                this.pressure = targetP;
                connectedTo.pressure = targetP;
                float targetFlow = Mathf.Min(this.volt, connectedTo.volt);
                this.volt = targetFlow;
                connectedTo.volt = targetFlow;
            }
            else if (connectedTo.portType == PortType.Input)
            {
                connectedTo.pressure = this.pressure;
                connectedTo.volt = this.volt;
            }


        }
    }
    // 在 PneumaticPort.cs 中加入
    private void OnDrawGizmos()
    {
        // 有压变红，无压变白
        // Gizmos.color = pressure > 0.1f ? Color.red : Color.white;
        //Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
