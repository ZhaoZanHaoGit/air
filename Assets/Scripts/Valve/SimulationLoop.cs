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

    void Awake() => Instance = this;

    void Update()
    {
        // 1. 全局重置 (必须保留，用于模拟气压消失)
        ResetAllPorts();


        // --- 核心迭代：严格执行你提出的三阶段逻辑 ---
        for (int i = 0; i < iterationCount; i++)
        {
            
            // 阶段 1：接收元器件内部计算信息 (对应你的 v.ProcessLogic)
            // 每个阀门（如节流阀、三通、气缸）全心全意根据当前状态算出自己两端“本该有的数值”
            foreach (var v in allValves)
            {
                v.ProcessLogic();
            }

            // 阶段 2：接收外部连接另一端的信息 (取代原来的暴力 Propagate)
            // 全场所有的端口通过手拉手的管线连接，看一眼对端这一帧传过来了什么数据
            foreach (var v in allValves)
            {
                foreach (var p in v.ports)
                {
                    p.ReceiveExternalInfo();
                }
            }

            // 阶段 3：整合输入信息，决定当前节点的确定流向，并进行最终物理输出
            // 端口根据“内压”和“外压”的压差（ΔP）拍板：确定谁是上游推压，谁是下游憋流
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
                // 关键：不重置 Output 端口，由阀门逻辑控制它们的生死
                // 同样，不重置 Input 端口可能导致信号残留，
                // 所以我们只重置 Free 类型的管路中间件
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
        // ItemManager.Instance.AddItem(@object.GetComponent<Item3D>().itemSelf, 1);
    }
    public void InitTrainType(PneumaticTrainingCase @case, trainType type
       )
    {
        CurrentCase = @case;
        currenttrainType = type;


    }
}