using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Hypertonic.GridPlacement;
using Hypertonic.GridPlacement.Enums;
using Hypertonic.GridPlacement.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

/// <summary>
/// 线缆类型：气管 / 细线(tinckElec) / 粗线(simplleElec)
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum WireType { pneumatic, tinckElec, simplleElec }

// ───────────────────────── 存档数据结构 ─────────────────────────

[Serializable]
public class ValveSaveData
{
    public int valveId;
    public string prefabName;
    public string valveName;
    public SerVec3 position;
    public float rotationY;
    public int gridCellX;
    public int gridCellY;
    [JsonConverter(typeof(StringEnumConverter))]
    public ObjectAlignment alignment;
}

[Serializable]
public class WireSaveData
{
    [JsonConverter(typeof(StringEnumConverter))]
    public WireType wireType;
    public int valveAId;
    [JsonConverter(typeof(StringEnumConverter))]
    public PneumaticPortName portA;
    public int valveBId;
    [JsonConverter(typeof(StringEnumConverter))]
    public PneumaticPortName portB;
}

[Serializable]
public class ItemSaveData
{
    public string itemName;
    public int itemnum;   // 背包剩余数量
    public int chazhi;     // 已放置数量
}

[Serializable]
public class SaveData
{
    public string saveTime;
    public string caseName;
    public int caseNumber;
    public string trainType;
    public List<ValveSaveData> valves = new List<ValveSaveData>();
    public List<WireSaveData> wires = new List<WireSaveData>();
    public List<ItemSaveData> items = new List<ItemSaveData>();
}

[Serializable]
public class SerVec3
{
    public float x, y, z;
    public SerVec3() { }
    public SerVec3(Vector3 v) { x = v.x; y = v.y; z = v.z; }
    public Vector3 ToVector3() => new Vector3(x, y, z);
}

[Serializable]
public class SaveFileInfo
{
    public string fileName;
    public string saveTime;
    public string caseName;
    public int valveCount;
    public int wireCount;
}

// ───────────────────────── 存档系统主体 ─────────────────────────

public class TrainingSaveSystem : MonoBehaviour
{
    public static TrainingSaveSystem Instance;

    /// <summary>加载完成回调</summary>
    public event Action<bool, string> OnLoadComplete;

    private string _saveDir;

    void Awake()
    {
        Instance = this;
        _saveDir = Path.Combine(Application.persistentDataPath, "Saves");
    }

    // ======================== 公开接口 ========================

    /// <summary>
    /// 保存当前场景状态。返回存档文件名（不含路径）。
    /// </summary>
    public string SaveGame()
    {
        var data = BuildSaveData();
        if (!Directory.Exists(_saveDir))
            Directory.CreateDirectory(_saveDir);

        string fileName = data.saveTime.Replace(":", "-").Replace("/", "-").Replace(" ", "_") + ".json";
        string filePath = Path.Combine(_saveDir, fileName);
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, json);

        Debug.Log($"[存档] 已保存到: {filePath}");
        return fileName;
    }

    /// <summary>
    /// 加载存档。异步执行，完成后触发 OnLoadComplete。
    /// </summary>
    public void LoadGame(string fileName)
    {
        StartCoroutine(LoadGameCoroutine(fileName));
    }

    /// <summary>
    /// 获取所有存档文件信息列表（按时间倒序）。
    /// </summary>
    public List<SaveFileInfo> GetSaveFiles()
    {
        var result = new List<SaveFileInfo>();
        if (!Directory.Exists(_saveDir))
            return result;

        foreach (var filePath in Directory.GetFiles(_saveDir, "*.json"))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                var data = JsonConvert.DeserializeObject<SaveData>(json);
                if (data != null)
                {
                    result.Add(new SaveFileInfo
                    {
                        fileName = Path.GetFileName(filePath),
                        saveTime = data.saveTime,
                        caseName = data.caseName,
                        valveCount = data.valves?.Count ?? 0,
                        wireCount = data.wires?.Count ?? 0
                    });
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[存档] 无法读取存档 {filePath}: {e.Message}");
            }
        }

        result.Sort((a, b) => string.Compare(b.saveTime, a.saveTime, StringComparison.Ordinal));
        return result;
    }

    /// <summary>
    /// 删除指定存档。
    /// </summary>
    public void DeleteSave(string fileName)
    {
        string filePath = Path.Combine(_saveDir, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"[存档] 已删除: {filePath}");
        }
    }

    // ======================== 保存逻辑 ========================

    private SaveData BuildSaveData()
    {
        var sim = SimulationLoop.Instance;
        var data = new SaveData
        {
            saveTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            caseName = sim.CurrentCase?.Casname ?? "",
            caseNumber = sim.CurrentCase?.CaseNamber ?? 0,
            trainType = sim.currenttrainType.ToString()
        };

        // —— 1. 保存元器件 ——
        // 构建 PortBase → valveId 映射（稍后用于线缆）
        var portToValveId = new Dictionary<PortBase, int>();

        for (int i = 0; i < sim.gridInScene.Count; i++)
        {
            var go = sim.gridInScene[i];
            if (go == null) continue;

            var bv = go.GetComponent<BaseValve>();
            if (bv == null) continue;

            // 获取网格信息
            var gridInfo = go.GetComponent<GridObjectInfo>();
            int cellX = 0, cellY = 0;
            ObjectAlignment alignment = ObjectAlignment.CENTER;
            if (gridInfo != null)
            {
                cellX = gridInfo.GridCellIndex.x;
                cellY = gridInfo.GridCellIndex.y;
                alignment = gridInfo.ObjectAlignment;
            }

            // 获取预制体名
            string prefabName = go.name;
            var item3d = go.GetComponent<Item3D>();
            if (item3d != null && item3d.itemSelf != null)
                prefabName = item3d.itemSelf.itemName;

            var vsd = new ValveSaveData
            {
                valveId = i,
                prefabName = prefabName,
                valveName = bv.valveName ?? bv.GetType().Name,
                position = new SerVec3(go.transform.position),
                rotationY = go.transform.rotation.eulerAngles.y,
                gridCellX = cellX,
                gridCellY = cellY,
                alignment = alignment
            };
            data.valves.Add(vsd);

            // 记录端口映射
            foreach (var port in bv.ports)
            {
                if (port != null)
                    portToValveId[port] = i;
            }
        }

        // —— 2. 保存线缆 ——
        // 气管 (linerendeler)
        var pneumaticLines = FindObjectsByType<linerendeler>(FindObjectsSortMode.None);
        foreach (var line in pneumaticLines)
        {
            if (line == null) continue;
            var wsd = BuildWireSaveData(line.p0, line.p2, portToValveId, WireType.pneumatic);
            if (wsd != null) data.wires.Add(wsd);
        }

        // 导线 (DynamicWire)
        var dynamicWires = FindObjectsByType<DynamicWire>(FindObjectsSortMode.None);
        foreach (var wire in dynamicWires)
        {
            if (wire == null) continue;

            var portA = GetPortFromTransform(wire.p0);
            var portB = GetPortFromTransform(wire.p2);
            if (portA == null || portB == null) continue;

            WireType wt = WireType.tinckElec;
            if (portA.chanel == portChanel.simplleElec)
                wt = WireType.simplleElec;

            var wsd = BuildWireSaveData(wire.p0, wire.p2, portToValveId, wt, portA, portB);
            if (wsd != null) data.wires.Add(wsd);
        }

        // —— 3. 保存背包/物品状态 ——
        var itemBtns = FindObjectsByType<UI_TestPanelItemBTN>(FindObjectsSortMode.None);
        foreach (var btn in itemBtns)
        {
            if (btn == null || btn.itemins == null) continue;
            data.items.Add(new ItemSaveData
            {
                itemName = btn.itemins.itemName,
                itemnum = btn.itemnum,
                chazhi = btn.chazhi
            });
        }

        return data;
    }

    private WireSaveData BuildWireSaveData(Transform t0, Transform t2,
        Dictionary<PortBase, int> portToValveId, WireType wireType,
        PortBase portA = null, PortBase portB = null)
    {
        if (portA == null) portA = GetPortFromTransform(t0);
        if (portB == null) portB = GetPortFromTransform(t2);
        if (portA == null || portB == null)
        {
            Debug.LogWarning("[存档] 线缆端点缺少 PortBase，跳过");
            return null;
        }

        if (!portToValveId.TryGetValue(portA, out int idA) ||
            !portToValveId.TryGetValue(portB, out int idB))
        {
            Debug.LogWarning($"[存档] 线缆端点未找到对应元器件: {portA.portandvalve?.valve}.{portA.portandvalve?.port} ←→ {portB.portandvalve?.valve}.{portB.portandvalve?.port}");
            return null;
        }

        return new WireSaveData
        {
            wireType = wireType,
            valveAId = idA,
            portA = portA.portandvalve.port,
            valveBId = idB,
            portB = portB.portandvalve.port
        };
    }

    private static PortBase GetPortFromTransform(Transform t)
    {
        if (t == null) return null;
        return t.GetComponent<PortBase>() ?? t.GetComponentInParent<PortBase>();
    }

    // ======================== 加载逻辑 ========================

    private IEnumerator LoadGameCoroutine(string fileName)
    {
        string filePath = Path.Combine(_saveDir, fileName);
        if (!File.Exists(filePath))
        {
            Debug.LogError($"[读档] 存档文件不存在: {filePath}");
            OnLoadComplete?.Invoke(false, "存档文件不存在");
            yield break;
        }

        SaveData data;
        try
        {
            string json = File.ReadAllText(filePath);
            data = JsonConvert.DeserializeObject<SaveData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[读档] JSON 解析失败: {e.Message}");
            OnLoadComplete?.Invoke(false, "JSON 解析失败");
            yield break;
        }

        if (data == null)
        {
            OnLoadComplete?.Invoke(false, "存档数据为空");
            yield break;
        }

        Debug.Log($"[读档] 开始加载: {data.saveTime}，元器件 {data.valves.Count} 个，线缆 {data.wires.Count} 根");

        // —— 1. 清空场景 ——
        SimulationLoop.Instance.DeleteAllValves();
        // 等待 Destroy 生效
        yield return new WaitForEndOfFrame();
        yield return null;

        // —— 2. 放置元器件 ——
        var valveIdMap = new Dictionary<int, GameObject>();
        var gridManager = GridManagerAccessor.GridManager;

        foreach (var vsd in data.valves)
        {
            GameObject prefab = FindPrefab(vsd.prefabName);
            if (prefab == null)
            {
                Debug.LogWarning($"[读档] 未找到预制体: {vsd.prefabName}，跳过");
                continue;
            }

            var go = Instantiate(prefab);
            go.name = prefab.name;
            go.transform.rotation = Quaternion.Euler(0, vsd.rotationY, 0);

            // 添加 Item3D（如果没有）
            if (!go.TryGetComponent<Item3D>(out _))
                go.AddComponent<Item3D>();

            // 查找对应的 UI_TestPanelItemBTN 并链接
            LinkItemUI(go, vsd.prefabName);

            bool success = false;
            Vector2Int cell = new Vector2Int(vsd.gridCellX, vsd.gridCellY);
            yield return gridManager.AddObjectToGridByCell(go, cell, vsd.alignment, (result) => success = result);

            if (success)
            {
                SimulationLoop.Instance.gridInScene.Add(go);
                valveIdMap[vsd.valveId] = go;
                Debug.Log($"[读档] 放置元器件 {vsd.prefabName} (ID:{vsd.valveId}) 于网格 ({cell.x},{cell.y})");
            }
            else
            {
                // 网格放置失败，回退到直接设置世界坐标
                go.transform.position = vsd.position.ToVector3();
                go.transform.rotation = Quaternion.Euler(0, vsd.rotationY, 0);
                SimulationLoop.Instance.gridInScene.Add(go);
                valveIdMap[vsd.valveId] = go;
                Debug.LogWarning($"[读档] 网格放置失败，使用世界坐标: {vsd.prefabName}");
            }
        }

        // 等待 BaseValve.Start() 注册到 allValves
        yield return null;
        yield return new WaitForEndOfFrame();

        // —— 3. 重建线缆 ——
        foreach (var wsd in data.wires)
        {
            if (!valveIdMap.TryGetValue(wsd.valveAId, out var goA) ||
                !valveIdMap.TryGetValue(wsd.valveBId, out var goB))
            {
                Debug.LogWarning($"[读档] 线缆端点元器件缺失: A={wsd.valveAId}, B={wsd.valveBId}");
                continue;
            }

            var bvA = goA.GetComponent<BaseValve>();
            var bvB = goB.GetComponent<BaseValve>();
            if (bvA == null || bvB == null) continue;

            PortBase portA = FindPort(bvA, wsd.portA);
            PortBase portB = FindPort(bvB, wsd.portB);
            if (portA == null || portB == null)
            {
                Debug.LogWarning($"[读档] 未找到端口: {wsd.portA} 或 {wsd.portB}");
                continue;
            }

            CreateWire(wsd.wireType, portA, portB);
        }

        // —— 4. 还原背包/物品状态 ——
        RestoreInventory(data.items);

        Debug.Log("[读档] 加载完成");
        OnLoadComplete?.Invoke(true, data.saveTime);
    }

    // ======================== 辅助方法 ========================

    /// <summary>
    /// 还原背包物品状态：模拟正常领取流程，设置 itemListGet_ui.num 后调用 Cinfirm_Rreceipt
    /// </summary>
    private void RestoreInventory(List<ItemSaveData> items)
    {
        var im = ItemManager.Instance;
        if (im == null)
        {
            Debug.LogWarning("[读档] ItemManager 未找到，跳过背包还原");
            return;
        }

        // 1. 清空当前背包数据
        im.ClearAllItemData();

        // 2. 重置所有 UI_TestPanelItemBTN 的 chazhi（防止残留旧值）
        var itemBtns = FindObjectsByType<UI_TestPanelItemBTN>(FindObjectsSortMode.None);
        foreach (var btn in itemBtns)
        {
            if (btn != null) btn.chazhi = 0;
        }

        // 3. 将存档中的物品数量写入对应的 itemListGet_ui.num
        //    总数量 = itemnum + chazhi（已放置 + 剩余）
        foreach (var itemSave in items)
        {
            foreach (var itemUI in im.itemListGet_scripts)
            {
                if (itemUI.item != null && itemUI.item.itemName == itemSave.itemName)
                {
                    itemUI.num = itemSave.itemnum + itemSave.chazhi;
                    itemUI.numText.text = itemUI.num.ToString();
                    break;
                }
            }
        }

        // 4. 调用 Cinfirm_Rreceipt：清空背包UI → AddItem → updateBagIcon（与正常流程一致）
        im.Cinfirm_Rreceipt();

        // 5. 调用 UpdateTestPanelItem 初始化测试面板按钮
        if (im.uIManager_qidong != null)
        {
            im.uIManager_qidong.UpdateTestPanelItem();
        }
        else
        {
            Debug.LogWarning("[读档] uIManager_qidong 未找到，跳过 UpdateTestPanelItem");
        }

        // 6. 覆盖每个按钮的 itemnum 和 chazhi 为存档值
        foreach (var itemSave in items)
        {
            var btn = FindItemBtnByName(itemSave.itemName);
            if (btn != null)
            {
                btn.itemnum = itemSave.itemnum;
                btn.chazhi = itemSave.chazhi;
                btn.UpdateUIState();
            }
        }

        Debug.Log($"[读档] 背包还原完成，共 {items.Count} 种物品");
    }

    /// <summary>
    /// 按 itemName 从 itemListGet_scripts 中查找 Item
    /// </summary>
    private Item FindItemByName(string itemName)
    {
        var im = ItemManager.Instance;
        if (im == null) return null;

        foreach (var itemUI in im.itemListGet_scripts)
        {
            if (itemUI.item != null && itemUI.item.itemName == itemName)
                return itemUI.item;
        }

        // 备用：从场景中的 UI_TestPanelItemBTN 查找
        var btns = FindObjectsByType<UI_TestPanelItemBTN>(FindObjectsSortMode.None);
        foreach (var btn in btns)
        {
            if (btn.itemins != null && btn.itemins.itemName == itemName)
                return btn.itemins;
        }

        return null;
    }

    /// <summary>
    /// 按 itemName 查找场景中的 UI_TestPanelItemBTN
    /// </summary>
    private UI_TestPanelItemBTN FindItemBtnByName(string itemName)
    {
        var btns = FindObjectsByType<UI_TestPanelItemBTN>(FindObjectsSortMode.None);
        foreach (var btn in btns)
        {
            if (btn.itemins != null && btn.itemins.itemName == itemName)
                return btn;
        }
        return null;
    }

    private GameObject FindPrefab(string prefabName)
    {
        // 优先从库存查找
        if (ItemManager.Instance != null)
        {
            foreach (var kvp in ItemManager.Instance.inventory)
            {
                if (kvp.Key.prefab != null && kvp.Key.prefab.name == prefabName)
                    return kvp.Key.prefab;
            }
            foreach (var itemUI in ItemManager.Instance.itemListGet_scripts)
            {
                if (itemUI.item != null && itemUI.item.prefab != null && itemUI.item.prefab.name == prefabName)
                    return itemUI.item.prefab;
            }
        }

        // 从场景中的 UI_TestPanelItemBTN 查找
        var btns = FindObjectsByType<UI_TestPanelItemBTN>(FindObjectsSortMode.None);
        foreach (var btn in btns)
        {
            if (btn.itemins != null && btn.itemins.prefab != null && btn.itemins.prefab.name == prefabName)
                return btn.itemins.prefab;
        }

        return null;
    }

    private void LinkItemUI(GameObject go, string prefabName)
    {
        var item3d = go.GetComponent<Item3D>();
        if (item3d == null) return;

        var btns = FindObjectsByType<UI_TestPanelItemBTN>(FindObjectsSortMode.None);
        foreach (var btn in btns)
        {
            if (btn.itemins != null && btn.itemins.prefab != null && btn.itemins.prefab.name == prefabName)
            {
                item3d.itemUI = btn;
                item3d.itemSelf = btn.itemins;
                break;
            }
        }
    }

    private static PortBase FindPort(BaseValve valve, PneumaticPortName portName)
    {
        foreach (var port in valve.ports)
        {
            if (port != null && port.portandvalve != null && port.portandvalve.port == portName)
                return port;
        }
        return null;
    }

    private void CreateWire(WireType wireType, PortBase portA, PortBase portB)
    {
        GameObject prefab = null;
        switch (wireType)
        {
            case WireType.pneumatic:
                prefab = TubeBuilderManager.Instance.pneumaticPrefab;
                break;
            case WireType.tinckElec:
                prefab = TubeBuilderManager.Instance.xixianPrefab;
                break;
            case WireType.simplleElec:
                prefab = TubeBuilderManager.Instance.cuxianPrefab;
                break;
        }
        if (prefab == null)
        {
            Debug.LogError($"[读档] 线缆预制体缺失: {wireType}");
            return;
        }

        var wire = Instantiate(prefab);
        wire.transform.position = Vector3.zero;

        if (wireType == WireType.pneumatic)
        {
            // 气管：设置 linerendeler 的 p0/p2，linerendeler.Update() 会自动注册
            var line = wire.GetComponent<linerendeler>();
            if (line != null)
            {
                line.p0 = portA.transform;
                line.p2 = portB.transform;
            }
        }
        else
        {
            // 导线：调用 DynamicWire.SetupWire
            var dynWire = wire.GetComponent<DynamicWire>();
            if (dynWire != null && portA.circuitNode != null && portB.circuitNode != null)
            {
                dynWire.SetupWire(portA.circuitNode, portB.circuitNode);
            }
            else
            {
                Debug.LogWarning("[读档] 导线 SetupWire 失败，circuitNode 可能为空");
            }
        }

        // 绑定端口状态
        portA.OnConnect(portB);
        portB.OnConnect(portA);
        portA.isOccupied = true;
        portB.isOccupied = true;
    }
}
