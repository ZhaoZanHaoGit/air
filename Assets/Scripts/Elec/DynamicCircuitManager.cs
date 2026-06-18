using System.Collections.Generic;
using UnityEngine;

public class DynamicCircuitManager : MonoBehaviour
{
    public static DynamicCircuitManager Instance { get; private set; }

    private List<DynamicNode> activeNodes = new List<DynamicNode>();

    // 【核心精简】：用一个统一的抽象父类列表，直接替代原本的 activeWires 和 activeSwitches
    private List<CircuitEdge> activeEdges = new List<CircuitEdge>();

    private bool isTopologyDirty = false;

    private Dictionary<DynamicNode, DynamicNode> parent = new Dictionary<DynamicNode, DynamicNode>();
    private Dictionary<DynamicNode, List<DynamicNode>> adjacencyList = new Dictionary<DynamicNode, List<DynamicNode>>();

    private void Awake() { if (Instance == null) Instance = this; else Destroy(gameObject); }
    private void LateUpdate() { if (isTopologyDirty) { ReconstructCircuit(); isTopologyDirty = false; } }
    public void MarkTopologyDirty() => isTopologyDirty = true;

    #region 多态多态注册接口
    public void RegisterNode(DynamicNode node)
    {
        activeNodes.Add(node);
        Debug.Log($"<color=cyan>[拓扑注册]</color> 节点已注册: {node.nodeName}, 类型: {node.type}");
        MarkTopologyDirty();
    }
    public void UnregisterNode(DynamicNode node) { activeNodes.Remove(node); MarkTopologyDirty(); }

    // 统一的边注册入口
    public void RegisterEdge(CircuitEdge edge)
    {
        activeEdges.Add(edge);
        // 强制转换为子类以获取更详细的信息
        if (edge is DynamicWire wire)
        {
            Debug.Log($"<color=cyan>[拓扑注册]</color> 导线已注册! 连通两端: {wire.nodeA?.nodeName} <-> {wire.nodeB?.nodeName}");
        }
        MarkTopologyDirty();
    }
    public void UnregisterEdge(CircuitEdge edge) { activeEdges.Remove(edge); MarkTopologyDirty(); }
    #endregion

    private void ReconstructCircuit()
    {
        // 1. 初始化节点状态
        parent.Clear();
        adjacencyList.Clear();
        foreach (var node in activeNodes)
        {
            parent[node] = node;
            adjacencyList[node] = new List<DynamicNode>();
            node.currentLevel = -1;
            node.currentPowerStatus = NodeType.Normal;
        }

        // 2. 多态建立连通性
        // 大脑现在只需要看这条边是否导通 (IsEdgeConnected)
        foreach (var edge in activeEdges)
        {
            if (!activeNodes.Contains(edge.nodeA))
            {
                Debug.LogError($"[拓扑阻断] 边 {edge.edgeID} 无法导通！因为 activeNodes 中不包含起始节点A: {edge.nodeA?.nodeName}");
                continue;
            }
            if (!activeNodes.Contains(edge.nodeB))
            {
                Debug.LogError($"[拓扑阻断] 边 {edge.edgeID} 无法导通！因为 activeNodes 中不包含终止节点B: {edge.nodeB?.nodeName}");
                continue;
            }
            if (edge.IsEdgeConnected && activeNodes.Contains(edge.nodeA) && activeNodes.Contains(edge.nodeB))
            {
                Union(edge.nodeA, edge.nodeB);
                adjacencyList[edge.nodeA].Add(edge.nodeB);
                adjacencyList[edge.nodeB].Add(edge.nodeA);
            }
            else
            {
                Debug.Log($"[拓扑跳过] 边 {edge.edgeID} 当前处于断开状态(如未闭合的开关)");
            }
        }

        // 3. 回路聚合
        Dictionary<DynamicNode, List<DynamicNode>> loops = new Dictionary<DynamicNode, List<DynamicNode>>();
        foreach (var node in activeNodes)
        {
            DynamicNode root = Find(node);
            if (!loops.ContainsKey(root)) loops[root] = new List<DynamicNode>();
            loops[root].Add(node);
        }
        // 4. 分析每个独立回路的电源供电状态
        Debug.Log($"<color=yellow>[回路盘点]</color> 当前全网共切分出 {loops.Count} 个独立网络分量。");
        // 4. 电源染色与短路分析
        foreach (var kvp in loops)
        {
            List<DynamicNode> loopNodes = kvp.Value;
            List<DynamicNode> posSources = loopNodes.FindAll(n => n.type == NodeType.Positive);
            List<DynamicNode> negSources = loopNodes.FindAll(n => n.type == NodeType.Negative);
            // 打印当前回路的详细身世
            string nodeNames = "";
            loopNodes.ForEach(n => nodeNames += n.nodeName + ", ");
            Debug.Log($"[回路详情] 根节点:{kvp.Key.nodeName} | 包含节点:[{nodeNames}] | 正极源数:{posSources.Count} | 负极源数:{negSources.Count}");
            if (posSources.Count > 0 && negSources.Count > 0) HandleShortCircuit(posSources, negSources);
            else if (posSources.Count > 0) RunBFS(posSources, NodeType.Positive);
            else if (negSources.Count > 0)
            { RunBFS(negSources, NodeType.Negative); }
            else
            {
                // 重点看这里会不会触发！
                Debug.LogWarning($"<color=orange>[无源回路]</color> 根节点为 {kvp.Key.nodeName} 的回路中没有任何电源，无法被染色！");
            }
        }

        // 5. 刷新表现
        foreach (var node in activeNodes) node.UpdateVisualEffect();
    }

    #region 并查集与搜索细节 (保持不变)
    private DynamicNode Find(DynamicNode node) { if (parent[node] == node) return node; return parent[node] = Find(parent[node]); }
    private void Union(DynamicNode nodeA, DynamicNode nodeB) { DynamicNode rA = Find(nodeA); DynamicNode rB = Find(nodeB); if (rA != rB) parent[rA] = rB; }
    private void RunBFS(List<DynamicNode> sources, NodeType powerType) { Queue<DynamicNode> q = new Queue<DynamicNode>(); foreach (var s in sources) { s.currentLevel = 0; s.currentPowerStatus = powerType; q.Enqueue(s); } while (q.Count > 0) { DynamicNode c = q.Dequeue(); foreach (var n in adjacencyList[c]) { if (n.currentLevel == -1 && n.type == NodeType.Normal) { n.currentLevel = c.currentLevel + 1; n.currentPowerStatus = powerType; q.Enqueue(n); } } } }
    private void HandleShortCircuit(List<DynamicNode> pos, List<DynamicNode> neg) { Queue<DynamicNode> pQ = new Queue<DynamicNode>(), nQ = new Queue<DynamicNode>(); Dictionary<DynamicNode, int> pL = new Dictionary<DynamicNode, int>(), nL = new Dictionary<DynamicNode, int>(); foreach (var p in pos) { pQ.Enqueue(p); pL[p] = 0; p.currentPowerStatus = NodeType.Positive; p.currentLevel = 0; } foreach (var n in neg) { nQ.Enqueue(n); nL[n] = 0; n.currentPowerStatus = NodeType.Negative; n.currentLevel = 0; } while (pQ.Count > 0 || nQ.Count > 0) { int pC = pQ.Count; for (int i = 0; i < pC; i++) { DynamicNode c = pQ.Dequeue(); foreach (var n in adjacencyList[c]) { if (!pL.ContainsKey(n)) { pL[n] = pL[c] + 1; if (nL.ContainsKey(n)) { TriggerShortCircuitDebug(n); return; } pQ.Enqueue(n); } } } int nC = nQ.Count; for (int i = 0; i < nC; i++) { DynamicNode c = nQ.Dequeue(); foreach (var n in adjacencyList[c]) { if (!nL.ContainsKey(n)) { nL[n] = nL[c] + 1; if (pL.ContainsKey(n)) { TriggerShortCircuitDebug(n); return; } nQ.Enqueue(n); } } } } }
    private void TriggerShortCircuitDebug(DynamicNode intersectionNode) { Debug.LogError($"<color=red>[电路故障DEBUG]</color> 检测到正负极冲突！交汇碰撞点位于节点: <b>{intersectionNode.nodeName}</b>"); }
    #endregion
}