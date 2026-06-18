using UnityEngine;

public abstract class CircuitEdge : MonoBehaviour
{
    [Header("========= 基础拓扑配置 =========")]
    public DynamicNode nodeA;        // 起始接线柱/节点
    public DynamicNode nodeB;        // 终止接线柱/节点

    [HideInInspector] public string edgeID;

    protected virtual void Start()
    {
        edgeID = System.Guid.NewGuid().ToString();

        // 只要边被初始化且端点齐全，就自动注册到网格管理器中
        if (nodeA != null && nodeB != null)
        {
            RegisterToManager();
        }
    }

    // 核心多态接口：由子类（导线或开关）自己决定当前在物理上是否导通
    public abstract bool IsEdgeConnected { get; }

    protected void RegisterToManager()
    {
        if (DynamicCircuitManager.Instance != null)
        {
            DynamicCircuitManager.Instance.RegisterEdge(this);
        }
    }

    protected virtual void OnDestroy()
    {
        // 无论什么边被销毁（被剪断或开关被拆除），都从管理器注销，触发重构
        if (DynamicCircuitManager.Instance != null)
        {
            DynamicCircuitManager.Instance.UnregisterEdge(this);
        }
    }
}