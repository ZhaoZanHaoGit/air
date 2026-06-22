using UnityEngine;

public class DynamicSwitch : CircuitEdge
{
    [Header("========= 开关特有属性 =========")]
    public bool isClosed = false;     // 闸刀是否闭合

    // 实现父类的抽象接口：开关导通的条件就是闸刀闭合
    public override bool IsEdgeConnected => isClosed;

    /// <summary>
    /// 外部（如鼠标点击）调用此方法来拨动闸刀
    /// </summary>
    public void SetSwitchState(bool closed)
    {
        isClosed = closed;
        // 状态变了，通知大脑“拓扑网格变脏了”，帧末统一重构
        if (DynamicCircuitManager.Instance != null)
        {
            DynamicCircuitManager.Instance.MarkTopologyDirty();
        }
    }
}