using UnityEngine;

public enum NodeType
{
    Normal,     // 普通接线点
    Positive,   // 电源正极
    Negative    // 电源负极
}

public class DynamicNode : MonoBehaviour
{
    [Header("节点配置")]
    public string nodeName;          // 节点唯一名称
    public NodeType type = NodeType.Normal;

    [Header("运行时状态 (由管理器计算)")]
    public int currentLevel = -1;    // 拓扑级数（-1为未通电，0为电源）
    public NodeType currentPowerStatus = NodeType.Normal; // 被染色的极性

    private void Start()
    {
        // 如果没有手动命名，默认使用物体名字
        if (string.IsNullOrEmpty(nodeName))
        {
            nodeName = gameObject.name;
        }

        // 动态注册到电路拓扑管理器
        if (DynamicCircuitManager.Instance != null)
        {
            DynamicCircuitManager.Instance.RegisterNode(this);
        }
    }

    private void OnDestroy()
    {
        // 被玩家删除时，自动注销并触发重构
        if (DynamicCircuitManager.Instance != null)
        {
            DynamicCircuitManager.Instance.UnregisterNode(this);
        }
    }

    /// <summary>
    /// 电路拓扑重构后被自动调用，可用于改变接线柱的高亮状态
    /// </summary>
    public void UpdateVisualEffect()
    {
        // 现阶段可以先留空，或者做个简单的Debug打印
    }
}