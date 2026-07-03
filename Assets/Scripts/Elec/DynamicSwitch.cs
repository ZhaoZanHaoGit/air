using UnityEngine;

/// <summary>
/// 开关类型
/// </summary>
public enum SwitchType
{
    /// <summary>常开：默认断开，触发后导通</summary>
    NormallyOpen,
    /// <summary>常闭：默认导通，触发后断开</summary>
    NormallyClosed
}

public class DynamicSwitch : CircuitEdge
{
    [Header("========= 开关特有属性 =========")]
    [Tooltip("常开：默认断开，按下后导通；常闭：默认导通，按下后断开")]
    public SwitchType switchType = SwitchType.NormallyOpen;

    [Tooltip("当前闸刀是否闭合（运行时状态，无需手动设置）")]
    [SerializeField]
    private bool isClosed = false;     // 闸刀是否闭合

    /// <summary>
    /// 当前闸刀是否闭合（只读属性，外部通过 SetSwitchState 修改）
    /// </summary>
    public bool IsClosed => isClosed;

    // 实现父类的抽象接口：开关导通的条件就是闸刀闭合
    public override bool IsEdgeConnected => isClosed;

    protected override void Start()
    {
        base.Start();
        // 根据 switchType 初始化闸刀状态
        isClosed = (switchType == SwitchType.NormallyClosed);
        // 初始状态也可能影响拓扑，标记脏位
        if (DynamicCircuitManager.Instance != null)
        {
            DynamicCircuitManager.Instance.MarkTopologyDirty();
        }
    }

    /// <summary>
    /// 外部（如鼠标点击）调用此方法来拨动闸刀
    /// </summary>
    public void SetSwitchState(bool closed)
    {
        isClosed = closed;
        // 状态变了，通知大脑"拓扑网格变脏了"，帧末统一重构
        if (DynamicCircuitManager.Instance != null)
        {
            DynamicCircuitManager.Instance.MarkTopologyDirty();
        }
    }
}
