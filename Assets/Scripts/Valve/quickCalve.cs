using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quickCalve : BaseValve
{
    // 端口索引约定：
    // ports[0]: P (进气口) -> 建议 PortType.Input
    // ports[1]: A (出气口/接执行元件) -> 必须 PortType.Output
    // ports[2]: R (排气口) -> 仿真中通常为常开，不需要物理连接

    void Start()
    {
        base.Start();
        if (ports.Count < 2) return;
        ports[0].state = PortState.Conduct; // P口常开
        ports[1].state = PortState.Conduct; // A口常开
        ports[1].internalConnectTo = ports[0]; // A口连接到P口

    }
    public override void ProcessLogic()
    {
        if (ports.Count < 2) return;
        ports[0].ReceiveInternalInfo();
        ports[1].ReceiveInternalInfo();
    }
}
