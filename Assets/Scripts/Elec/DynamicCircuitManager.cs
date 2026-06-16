using System.Collections.Generic;
using UnityEngine;

public class DynamicCircuitManager : MonoBehaviour
{
    // 单例模式，方便运行时动态生成的节点和导线随时访问
    public static DynamicCircuitManager Instance { get; private set; }

    // 运行时维护的核心动态物理网格列表
    private List<DynamicNode> activeNodes = new List<DynamicNode>();
    private List<DynamicWire> activeWires = new List<DynamicWire>();

    // 脏标记：如果一帧内有多处改动（比如一瞬间删除了多个电器），只在帧末尾统一计算一次拓扑，避免卡顿
    private bool isTopologyDirty = false;

    // 并查集查找字典：节点 -> 它的父节点
    private Dictionary<DynamicNode, DynamicNode> parent = new Dictionary<DynamicNode, DynamicNode>();
    // 图的邻接表：节点 -> 与其直接导通的邻居节点列表（供BFS遍历使用）
    private Dictionary<DynamicNode, List<DynamicNode>> adjacencyList = new Dictionary<DynamicNode, List<DynamicNode>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void LateUpdate()
    {
        // 检查脏标记，在渲染前的最后一刻统一重构电路网络
        if (isTopologyDirty)
        {
            ReconstructCircuit();
            isTopologyDirty = false;
        }
    }

    /// <summary>
    /// 主动标记拓扑结构已改变，申请在帧末重构
    /// </summary>
    public void MarkTopologyDirty() => isTopologyDirty = true;

    #region 动态实体注册与注销接口
    public void RegisterNode(DynamicNode node) { activeNodes.Add(node); MarkTopologyDirty(); }
    public void UnregisterNode(DynamicNode node) { activeNodes.Remove(node); MarkTopologyDirty(); }
    public void RegisterWire(DynamicWire wire) { activeWires.Add(wire); MarkTopologyDirty(); }
    public void UnregisterWire(DynamicWire wire) { activeWires.Remove(wire); MarkTopologyDirty(); }
    #endregion

    /// <summary>
    /// 核心算法：重新构建并分析整个场景的电路网格
    /// </summary>
    private void ReconstructCircuit()
    {
        // 1. 初始化所有运行数据和状态
        parent.Clear();
        adjacencyList.Clear();
        foreach (var node in activeNodes)
        {
            parent[node] = node;
            adjacencyList[node] = new List<DynamicNode>();

            node.currentLevel = -1;       // 默认恢复未通电状态
            node.currentPowerStatus = NodeType.Normal;
        }

        // 2. 筛选有效导线，构建并查集连通性与邻接表
        foreach (var wire in activeWires)
        {
            // 只有当导线处于导通状态，且两端的节点都真实存在于场景中时才进行计算
            if (wire.IsConnected && activeNodes.Contains(wire.nodeA) && activeNodes.Contains(wire.nodeB))
            {
                Union(wire.nodeA, wire.nodeB);

                // 建立无向图的邻接双向边
                adjacencyList[wire.nodeA].Add(wire.nodeB);
                adjacencyList[wire.nodeB].Add(wire.nodeA);
            }
        }

        // 3. 回路（连通分量）聚合
        // 将属于同一个根节点的全部普通节点归类到同一个回路列表中
        Dictionary<DynamicNode, List<DynamicNode>> loops = new Dictionary<DynamicNode, List<DynamicNode>>();
        foreach (var node in activeNodes)
        {
            DynamicNode root = Find(node);
            if (!loops.ContainsKey(root))
            {
                loops[root] = new List<DynamicNode>();
            }
            loops[root].Add(node);
        }

        // 4. 分析每个独立回路的电源供电状态
        // 【核心修复点】：修正了大括号包裹与错位残留代码
        foreach (var kvp in loops)
        {
            List<DynamicNode> loopNodes = kvp.Value;

            // 筛选出该回路内部包含的所有正极和负极电源
            List<DynamicNode> posSources = loopNodes.FindAll(n => n.type == NodeType.Positive);
            List<DynamicNode> negSources = loopNodes.FindAll(n => n.type == NodeType.Negative);

            // 【情况A】：同时包含正极和负极 -> 触发短路冲突检测
            if (posSources.Count > 0 && negSources.Count > 0)
            {
                HandleShortCircuit(posSources, negSources);
            }
            // 【情况B】：只包含正极供电
            else if (posSources.Count > 0)
            {
                RunBFS(posSources, NodeType.Positive);
            }
            // 【情况C】：只包含负极供电
            else if (negSources.Count > 0)
            {
                RunBFS(negSources, NodeType.Negative);
            }
            // 【情况D】：无源回路，不做任何处理，保持初始断电状态
        }

        // 5. 统一通知所有活动的节点根据最新计算出的状态更新画面
        foreach (var node in activeNodes)
        {
            node.UpdateVisualEffect();
        }
    }

    #region 并查集数据结构核心实现
    private DynamicNode Find(DynamicNode node)
    {
        if (parent[node] == node) return node;
        // 带有路径压缩的递归查找
        return parent[node] = Find(parent[node]);
    }

    private void Union(DynamicNode nodeA, DynamicNode nodeB)
    {
        DynamicNode rootA = Find(nodeA);
        DynamicNode rootB = Find(nodeB);
        if (rootA != rootB)
        {
            parent[rootA] = rootB; // 合并两个回路
        }
    }
    #endregion

    #region 广度优先搜索 (BFS) 级数染色
    private void RunBFS(List<DynamicNode> sources, NodeType powerType)
    {
        Queue<DynamicNode> queue = new Queue<DynamicNode>();

        // 初始化所有电源点为第 0 级
        foreach (var src in sources)
        {
            src.currentLevel = 0;
            src.currentPowerStatus = powerType;
            queue.Enqueue(src);
        }

        // 开始向外层层扩散计算距离级数
        while (queue.Count > 0)
        {
            DynamicNode curr = queue.Dequeue();

            foreach (var neighbor in adjacencyList[curr])
            {
                // 如果邻居尚未被染色，且是普通节点
                if (neighbor.currentLevel == -1 && neighbor.type == NodeType.Normal)
                {
                    neighbor.currentLevel = curr.currentLevel + 1;
                    neighbor.currentPowerStatus = powerType;
                    queue.Enqueue(neighbor);
                }
            }
        }
    }
    #endregion

    #region 双端同步 BFS 短路交汇点精确定位
    private void HandleShortCircuit(List<DynamicNode> posSources, List<DynamicNode> negSources)
    {
        Queue<DynamicNode> posQueue = new Queue<DynamicNode>();
        Queue<DynamicNode> negQueue = new Queue<DynamicNode>();

        // 记录正极和负极分别传导到某节点时的步数
        Dictionary<DynamicNode, int> posLevels = new Dictionary<DynamicNode, int>();
        Dictionary<DynamicNode, int> negLevels = new Dictionary<DynamicNode, int>();

        // 初始化双端起点
        foreach (var p in posSources) { posQueue.Enqueue(p); posLevels[p] = 0; p.currentPowerStatus = NodeType.Positive; p.currentLevel = 0; }
        foreach (var n in negSources) { negQueue.Enqueue(n); negLevels[n] = 0; n.currentPowerStatus = NodeType.Negative; n.currentLevel = 0; }

        // 双端交替步进式搜索，完美模拟两路信号在导线中相向而行的相遇过程
        while (posQueue.Count > 0 || negQueue.Count > 0)
        {
            // 1. 正极信号向前扩散一波
            int posCount = posQueue.Count;
            for (int i = 0; i < posCount; i++)
            {
                DynamicNode curr = posQueue.Dequeue();
                foreach (var neighbor in adjacencyList[curr])
                {
                    if (!posLevels.ContainsKey(neighbor))
                    {
                        posLevels[neighbor] = posLevels[curr] + 1;

                        // 碰撞检测：如果负极已经踩过这个节点了，说明这里就是交汇死锁点！
                        if (negLevels.ContainsKey(neighbor))
                        {
                            TriggerShortCircuitDebug(neighbor);
                            return;
                        }
                        posQueue.Enqueue(neighbor);
                    }
                }
            }

            // 2. 负极信号向前扩散一波
            int negCount = negQueue.Count;
            for (int i = 0; i < negCount; i++)
            {
                DynamicNode curr = negQueue.Dequeue();
                foreach (var neighbor in adjacencyList[curr])
                {
                    if (!negLevels.ContainsKey(neighbor))
                    {
                        negLevels[neighbor] = negLevels[curr] + 1;

                        // 碰撞检测：如果正极已经踩过这个节点了，说明这里就是交汇死锁点！
                        if (posLevels.ContainsKey(neighbor))
                        {
                            TriggerShortCircuitDebug(neighbor);
                            return;
                        }
                        negQueue.Enqueue(neighbor);
                    }
                }
            }
        }
    }

    // 找到具体的短路交汇点后的处理逻辑
    private void TriggerShortCircuitDebug(DynamicNode intersectionNode)
    {
        Debug.LogError($"<color=red>[电路故障DEBUG]</color> 检测到正负极冲突！交汇碰撞点位于节点: <b>{intersectionNode.nodeName}</b>");
    }
    #endregion
}