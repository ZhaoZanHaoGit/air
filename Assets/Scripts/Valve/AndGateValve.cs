using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndGateValve : BaseValve
{
    // Index 0: Input 1
    // Index 1: Input 2
    // Index 2: Output A (设置为 PortType.Output)

    public override void ProcessLogic()
    {
        // 只有当两个输入口压力都超过阈值时
        if (ports[0].pressure > 0.1f && ports[1].pressure > 0.1f)
        {
            // 输出较小的压力值（模拟双压阀物理特性）
            ports[2].pressure = Mathf.Min(ports[0].pressure, ports[1].pressure);
           // ports[2].flowRate = Mathf.Min(ports[0].flowRate, ports[1].flowRate);
        }
        else
        {
            ports[2].pressure = 0f;
        }
    }
}
