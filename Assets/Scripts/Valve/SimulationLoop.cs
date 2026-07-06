using Hypertonic.GridPlacement;
using System.Collections.Generic;
using UnityEngine;

public class SimulationLoop : MonoBehaviour
{
    public static SimulationLoop Instance;
    public List<BaseValve> allValves = new List<BaseValve>();
    public List<PortBase> mainSources = new List<PortBase>();
    public List<GameObject> gridInScene = new List<GameObject>();
    public GameObject currentLine;

    public DeleteLine deleteLineScript;
    public PneumaticTrainingCase CurrentCase;
    public CustomList<caseportData> currentPostsData = new CustomList<caseportData>();
    public trainType currenttrainType;
    public int Score = 100;
    [Header("仿真设置")]
    [Tooltip("迭代次数建议设为 8 以上，确保压力穿透三通等并联节点")]
    public int iterationCount = 20;

    // ==========================================
    // 线缆注册表：记录每根管的两个端点 portandvalve
    // RebuildGroups() 时用并查集把它们按连通性分组
    // ==========================================
    private class WireEntry
    {
        public portandvalve portAData;
        public portandvalve portBData;
        public string keyA;       // 基于 PortBase 实例ID 的唯一 key
        public string keyB;       // 基于 PortBase 实例ID 的唯一 key
    }
    private readonly List<WireEntry> _wireRegistry = new List<WireEntry>();

    void Awake() => Instance = this;

    void Update()
    {
        // 1. 全局重置 (必须保留，用于模拟气压消失)
        ResetAllPorts();


        // --- 核心迭代：严格执行你提出的三阶段逻辑 ---
        for (int i = 0; i < iterationCount; i++)
        {

            // 阶段 1：接收元器件内部计算信息 (对应你的 v.ProcessLogic)
            foreach (var v in allValves)
            {
                v.ProcessLogic();
            }

            // 阶段 2：接收外部连接另一端的信息 (取代原来的暴力 Propagate)
            foreach (var v in allValves)
            {
                foreach (var p in v.ports)
                {
                    p.ReceiveExternalInfo();
                }
            }

            // 阶段 3：整合输入信息，决定当前节点的确定流向，并进行最终物理输出
            foreach (var v in allValves)
            {
                foreach (var p in v.ports)
                {
                    p.IntegrateAndOutput();
                }
            }


        }
    }
    void ResetAllPorts()
    {
        foreach (var v in allValves)
        {
            foreach (var p in v.ports)
            {
                if (p.portType != PortType.main)
                {
                    p.ClearPressureState();
                }
            }
        }
    }
    public void RegisterValve(BaseValve v) { if (!allValves.Contains(v)) allValves.Add(v); }

    public void DeleteAllValves()
    {
        Debug.Log("删除全部");
        foreach (var v in gridInScene)
        {
            ReturnItemUI(v);
            GridManagerAccessor.GridManager.DeleteObject(v);
        }
        gridInScene.Clear();
    }
    public void DeleteAllLines()
    {
        // 先清空线缆注册表并重建分组（currentPostsData 立即刷新）
        _wireRegistry.Clear();
        RebuildGroups();

        linerendeler[] ports = GameObject.FindObjectsByType<linerendeler>(FindObjectsSortMode.None);
        foreach (var item in ports)
        {
            Destroy(item.gameObject);
        }
    }
    public void ReturnItemUI(GameObject @object)
    {
        if (@object.GetComponent<Item3D>().itemUI == null)
        { return; }
        @object.GetComponent<Item3D>().itemUI.itemnum++;
        @object.GetComponent<Item3D>().itemUI.chazhi--;
        @object.GetComponent<Item3D>().itemUI.UpdateUIState();
    }
    public void InitTrainType(PneumaticTrainingCase @case, trainType type
       )
    {
        CurrentCase = @case;
        currenttrainType = type;


    }

    // ==========================================
    // 线缆注册 / 注销 — 供 linerendeler 调用
    // ==========================================

    /// <summary>
    /// 注册一根气管：两端的 PortBase 传入，自动提取 portandvalve 并加入注册表，
    /// 然后触发 RebuildGroups() 更新 currentPostsData。
    /// </summary>
    public void RegisterWire(PortBase portA, PortBase portB)
    {
        if (portA == null || portB == null) return;
        var pvA = portA.portandvalve;
        var pvB = portB.portandvalve;
        if (pvA == null || pvB == null)
        {
            Debug.LogWarning("[SimulationLoop] 端口缺少 portandvalve 数据，无法注册线缆！");
            return;
        }

        Debug.Log($"<color=cyan>[接线注册]</color> {pvA.valve}.{pvA.port} ←→ {pvB.valve}.{pvB.port}");

        string keyA = GetPortKey(portA);
        string keyB = GetPortKey(portB);
        _wireRegistry.Add(new WireEntry { portAData = pvA, portBData = pvB, keyA = keyA, keyB = keyB });
        RebuildGroups();
        DebugDumpWiring("注册后");
    }

    /// <summary>
    /// 注销一根气管：根据两端的 portandvalve 数据移除注册表中的对应条目，
    /// 然后触发 RebuildGroups() 更新 currentPostsData。
    /// </summary>
    public void UnregisterWire(PortBase portA, PortBase portB)
    {
        if (portA == null || portB == null) return;

        string keyA = GetPortKey(portA);
        string keyB = GetPortKey(portB);
        var pvA = portA.portandvalve;
        var pvB = portB.portandvalve;

        Debug.Log($"<color=#FF6600>[接线注销]</color> {pvA.valve}.{pvA.port} ←→ {pvB.valve}.{pvB.port}");
        _wireRegistry.RemoveAll(w =>
            (w.keyA == keyA && w.keyB == keyB) ||
            (w.keyA == keyB && w.keyB == keyA));

        RebuildGroups();
        DebugDumpWiring("注销后");
    }

    // ==========================================
    // 并查集分组重建 — 核心逻辑
    // ==========================================

    /// <summary>
    /// 生成端口唯一 key：包含实例 ID，确保同名端口在不同元件实例之间不碰撞。
    /// </summary>
    private static string GetPortKey(PortBase port)
    {
        if (port == null) return "";
        int id = port.gameObject.GetInstanceID();
        return $"{port.portandvalve.valve}_{port.portandvalve.port}_{id}";
    }

    /// <summary>
    /// Debug：输出当前所有接线信息和分组结果
    /// </summary>
    private void DebugDumpWiring(string tag)
    {
        if (_wireRegistry.Count == 0)
        {
            Debug.Log($"<color=#888>[接线信息]</color> {tag}：当前无任何线缆");
            return;
        }

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"<color=yellow>[接线信息]</color> {tag}：共 {_wireRegistry.Count} 根线缆，{currentPostsData.Count} 个回路组");

        for (int i = 0; i < _wireRegistry.Count; i++)
        {
            var w = _wireRegistry[i];
            sb.AppendLine($"  线缆[{i}] {w.portAData.valve}.{w.portAData.port} ←→ {w.portBData.valve}.{w.portBData.port}  [key: {w.keyA} / {w.keyB}]");
        }

        sb.AppendLine("  ---- 分组结果 ----");
        for (int i = 0; i < currentPostsData.Count; i++)
        {
            var group = currentPostsData[i];
            sb.Append($"  组[{i}]：");
            for (int j = 0; j < group.nodes.Count; j++)
            {
                if (j > 0) sb.Append(", ");
                sb.Append($"{group.nodes[j].valve}.{group.nodes[j].port}");
            }
            sb.AppendLine();
        }

        Debug.Log(sb.ToString());
    }

    /// <summary>
    /// 根据当前线缆注册表 + 三通内部连通性，用并查集重建 currentPostsData。
    /// 三通级联（三通→三通→元器件）也能正确归入同一组。
    /// </summary>
    private void RebuildGroups()
    {
        // 并查集数据结构
        var parent = new Dictionary<string, string>();
        var portDataMap = new Dictionary<string, portandvalve>();

        string Find(string x)
        {
            if (!parent.ContainsKey(x)) parent[x] = x;
            while (parent[x] != x)
            {
                parent[x] = parent[parent[x]]; // 路径压缩
                x = parent[x];
            }
            return x;
        }

        void Union(string a, string b)
        {
            string ra = Find(a), rb = Find(b);
            if (ra != rb) parent[ra] = rb;
        }

        // 步骤 1：注册所有线缆端点（使用预计算的唯一 key）
        foreach (var wire in _wireRegistry)
        {
            if (wire.portAData != null && !portDataMap.ContainsKey(wire.keyA))
                portDataMap[wire.keyA] = wire.portAData;
            if (wire.portBData != null && !portDataMap.ContainsKey(wire.keyB))
                portDataMap[wire.keyB] = wire.portBData;
        }

        // 步骤 2：合并线缆直连的两个端口（使用预计算的唯一 key）
        foreach (var wire in _wireRegistry)
        {
            if (wire.portAData != null && wire.portBData != null)
            {
                Union(wire.keyA, wire.keyB);
            }
        }

        // 步骤 3：合并三通上的端口（只有已被线缆连接的三通端口才参与合并）
        // 三通内部 3 个端口全部互通，所以 2 根以上线接入同一三通时，它们的对端元器件自动归入一组
        foreach (var valve in allValves)
        {
            if (valve is T_JointValve tJoint)
            {
                var connectedKeys = new List<string>();
                foreach (var port in tJoint.ports)
                {
                    if (port.portandvalve != null)
                    {
                        string key = GetPortKey(port);
                        if (portDataMap.ContainsKey(key))
                        {
                            connectedKeys.Add(key);
                        }
                    }
                }
                for (int i = 1; i < connectedKeys.Count; i++)
                {
                    Union(connectedKeys[0], connectedKeys[i]);
                }
            }
        }

        // 步骤 4：按根节点分组
        var groups = new Dictionary<string, List<portandvalve>>();
        foreach (var kvp in portDataMap)
        {
            string root = Find(kvp.Key);
            if (!groups.ContainsKey(root))
                groups[root] = new List<portandvalve>();
            groups[root].Add(kvp.Value);
        }

        // 步骤 5：构建 caseportData 列表（只保留 2 节点以上的组）
        var newGroups = new List<caseportData>();
        foreach (var group in groups.Values)
        {
            if (group.Count >= 2)
            {
                newGroups.Add(new caseportData { nodes = group });
            }
        }

        // 步骤 6：批量更新 currentPostsData（只触发一次回调）
        currentPostsData.Rebuild(newGroups);
    }
}
