using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class casesPart2Answer : MonoBehaviour
{

    // 拖拽赋值你的主 UI 面板以获取当前案例
    public TraniningPanel trainingPanel;
    public List<caseportData> case1 = new List<caseportData>();
    public List<caseportData> case2 = new List<caseportData>();
    public List<caseportData> case3 = new List<caseportData>();
    public List<caseportData> case4 = new List<caseportData>();
    public List<caseportData> case5 = new List<caseportData>();
    public List<caseportData> case6 = new List<caseportData>();
    public List<caseportData> case7 = new List<caseportData>();
    public List<caseportData> case8 = new List<caseportData>();

    public List<List<caseportData>> allCases = new List<List<caseportData>>();

    // Start is called before the first frame update
    void Start()
    {
        allCases.Add(case1);
        allCases.Add(case2);
        allCases.Add(case3);
        allCases.Add(case4);
        allCases.Add(case5);
        allCases.Add(case6);
        allCases.Add(case7);
        allCases.Add(case8);
        if (SimulationLoop.Instance != null && SimulationLoop.Instance.currentPostsData != null)
        {
            SimulationLoop.Instance.currentPostsData.OnItemAdded += onportchange;
            SimulationLoop.Instance.currentPostsData.OnItemRemoved += onportchange;
        }

    }
    private void OnDestroy()
    {
        if (SimulationLoop.Instance != null && SimulationLoop.Instance.currentPostsData != null)
        {
            SimulationLoop.Instance.currentPostsData.OnItemAdded -= onportchange;
            SimulationLoop.Instance.currentPostsData.OnItemRemoved -= onportchange;
        }
    }
    void onportchange(caseportData data)
    {
        if (SimulationLoop.Instance.currenttrainType != trainType.free)
        { trainingPanel.DebugData.text = CheckConnectionsBasic(SimulationLoop.Instance.currentPostsData, GetCurrentStandardAnswers()); }
        RectTransform Rect = (RectTransform)trainingPanel.DebugData.transform.parent;
        if (Rect != null)
        {
            // 强行重建当前物体以及所有子物体的布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
        }
    }
    /// <summary>
    /// 获取当前选定案例对应的标准答案连线列表
    /// </summary>
    /// <returns>返回当前案例的标准连线列表；若获取失败则返回 null</returns>
    public List<caseportData> GetCurrentStandardAnswers()
    {
        if (trainingPanel == null || trainingPanel.currentTrainingCase == null)
        {
            Debug.LogWarning("未选择当前训练案例，或 TrainingPanel 未正确引用！");
            return null;
        }

        int currentCaseNamber = trainingPanel.currentTrainingCase.CaseNamber;
        int listIndex = currentCaseNamber - 1;

        if (allCases == null)
        {
            Debug.LogError("标准答案数据库 (answersDb) 为空！");
            return null;
        }

        if (listIndex < 0 || listIndex >= allCases.Count)
        {
            Debug.LogError($"无法找到 CaseNamber: {currentCaseNamber} 对应的标准答案列表。当前库中仅有 {allCases.Count} 个案例。");
            return null;
        }

        return allCases[listIndex];
    }

    /// <summary>
    /// 验证给定的单条回路数据是否包含在标准答案中
    /// </summary>
    /// <param name="targetData">玩家实际连接的一条回路数据</param>
    /// <returns>返回 true 表示连接正确，false 表示连错或多连</returns>
    public bool VerifyPortConnection(caseportData targetData)
    {
        if (trainingPanel.currentTrainingCase == null)
        {
            Debug.LogWarning("未选择当前训练案例！");
            return false;
        }

        int currentCaseNamber = trainingPanel.currentTrainingCase.CaseNamber;
        int listIndex = currentCaseNamber - 1;

        if (allCases == null)
        {
            Debug.LogError("标准答案数据库为空！");
            return false;
        }

        if (listIndex < 0 || listIndex >= allCases.Count)
        {
            Debug.LogError($"无法找到 CaseNamber: {currentCaseNamber} 对应的标准答案列表。");
            return false;
        }

        List<caseportData> standardAnswers = allCases[listIndex];

        foreach (caseportData standard in standardAnswers)
        {
            if (GroupsMatch(targetData, standard))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 方法一：仅判断玩家列表中的每一条回路是否存在于标准答案中，输出状态文本。
    /// </summary>
    public string CheckConnectionsBasic(List<caseportData> userList, List<caseportData> standardList)
    {
        if (userList == null || standardList == null) return "数据为空。";

        StringBuilder sb = new StringBuilder();

        foreach (var userItem in userList)
        {
            // 判断是否为 L 口连线
            if (IsLPortConnection(userItem))
            {
                sb.AppendLine($"回路 [{FormatGroup(userItem)}] : <color=#00FFFF>L口连线 (仅记录)</color>");
                continue;
            }
            bool exists = false;
            foreach (var stdItem in standardList)
            {
                if (GroupsMatch(userItem, stdItem))
                {
                    exists = true;
                    break;
                }
            }

            string status = exists ? "<color=#00FF00>存在于标准答案中</color>" : "<color=#FF0000>不存在 (多余连线)</color>";
            sb.AppendLine($"回路 [{FormatGroup(userItem)}] : {status}");
        }

        return sb.ToString();
    }

    public void TiShi(TextMeshProUGUI text)
    {
        text.text = CheckConnectionsBasic(SimulationLoop.Instance.currentPostsData, GetCurrentStandardAnswers());
        RectTransform Rect = (RectTransform)text.transform.parent;
        if (Rect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
        }
    }
    public void xunxian(TextMeshProUGUI text)
    {
        text.text = EvaluateAllConnections(SimulationLoop.Instance.currentPostsData, GetCurrentStandardAnswers()).DetailReport;
        RectTransform Rect = (RectTransform)text.transform.parent;
        if (Rect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
        }
    }
    public ConnectionReport ExamEvaluate()
    {
        ConnectionReport report = EvaluateAllConnections(SimulationLoop.Instance.currentPostsData, GetCurrentStandardAnswers());
        return report;
    }


    /// <summary>
    /// 方法二：进阶评估。不仅检查玩家的回路是否正确，还找出标准答案中遗漏的回路，并统计错误总数。
    /// </summary>
    public ConnectionReport EvaluateAllConnections(List<caseportData> userList, List<caseportData> standardList)
    {
        ConnectionReport result = new ConnectionReport();
        if (userList == null || standardList == null) return result;

        StringBuilder sb = new StringBuilder();
        int errorCount = 0;
        int overCount = 0;

        // 复制一份标准答案列表，用于追踪哪些标准回路已经被玩家连上了
        List<caseportData> unmatchedStandard = new List<caseportData>(standardList);

        // 1. 遍历玩家回路，检查多连或错连
        foreach (var userItem in userList)
        {
            // 判断是否为 L 口连线
            if (IsLPortConnection(userItem))
            {
                sb.AppendLine($"回路 [{FormatGroup(userItem)}] : <color=#00FFFF>L口连线 (仅记录，不参与判定)</color>");
                continue;
            }
            // 在未匹配的标准答案中寻找匹配项
            int matchIndex = -1;
            for (int i = 0; i < unmatchedStandard.Count; i++)
            {
                if (GroupsMatch(userItem, unmatchedStandard[i]))
                {
                    matchIndex = i;
                    break;
                }
            }

            if (matchIndex != -1)
            {
                sb.AppendLine($"回路 [{FormatGroup(userItem)}] : <color=#00FF00>正确</color>");
                unmatchedStandard.RemoveAt(matchIndex);
            }
            else
            {
                sb.AppendLine($"回路 [{FormatGroup(userItem)}] : <color=#FF0000>错误 (多余或重复)</color>");
                overCount++;
            }
        }

        // 2. 遍历结束后，unmatchedStandard 中剩下的就是玩家遗漏的回路
        foreach (var missingItem in unmatchedStandard)
        {
            sb.AppendLine($"回路 [{FormatGroup(missingItem)}] : <color=#FFAA00>遗漏 (标准答案中有但未连接)</color>");
            errorCount++;
        }

        result.DetailReport = sb.ToString();
        result.ErrorCount = errorCount;
        result.OverCount = overCount;

        return result;
    }

    // ================= 辅助方法 =================

    /// <summary>
    /// 判断这根回路中是否含有 L 口
    /// </summary>
    private bool IsLPortConnection(caseportData item)
    {
        if (item?.nodes == null) return false;
        foreach (var node in item.nodes)
        {
            if (node != null && node.port == PneumaticPortName.L)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 判定两条回路是否等效：过滤三通/四通端口后，做精确集合匹配。
    /// 标准答案只需列出元器件端口，三通中间端口自动忽略。
    /// </summary>
    private bool GroupsMatch(caseportData userGroup, caseportData standardGroup)
    {
        if (userGroup?.nodes == null || standardGroup?.nodes == null) return false;

        // 过滤三通/四通端口，只保留元器件端口
        var userComponentPorts = FilterComponentPorts(userGroup.nodes);
        var stdComponentPorts = FilterComponentPorts(standardGroup.nodes);

        // 数量不同，肯定不匹配
        if (userComponentPorts.Count != stdComponentPorts.Count) return false;

        // 检查标准答案中的每个端口是否都存在于玩家回路中
        foreach (var stdNode in stdComponentPorts)
        {
            bool found = false;
            foreach (var userNode in userComponentPorts)
            {
                if (userNode.valve == stdNode.valve && userNode.port == stdNode.port)
                {
                    found = true;
                    break;
                }
            }
            if (!found) return false;
        }
        return true;
    }

    /// <summary>
    /// 过滤掉三通/四通端口，只保留真正的元器件端口
    /// </summary>
    private List<portandvalve> FilterComponentPorts(List<portandvalve> nodes)
    {
        var result = new List<portandvalve>();
        foreach (var node in nodes)
        {
            if (node == null) continue;
            if (node.valve != ValveName.三通 && node.valve != ValveName.四通)
            {
                result.Add(node);
            }
        }
        return result;
    }

    /// <summary>
    /// 格式化单个端口名称用于文本输出
    /// </summary>
    private string FormatPort(portandvalve port)
    {
        if (port == null) return "未知端口";
        return port.GetPortAndValveInfo();
    }

    /// <summary>
    /// 格式化整条回路的所有端口，用 | 分隔
    /// </summary>
    private string FormatGroup(caseportData group)
    {
        if (group?.nodes == null || group.nodes.Count == 0) return "空回路";

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < group.nodes.Count; i++)
        {
            if (i > 0) sb.Append(" | ");
            sb.Append(FormatPort(group.nodes[i]));
        }
        return sb.ToString();
    }
}

/// <summary>
/// 接线标准/运行时数据结构：一条回路的多个连通节点。
/// 支持三通级联场景：P → 三通 → A、B 属于同一条回路。
/// </summary>
[System.Serializable]
public class caseportData
{
    public List<portandvalve> nodes = new List<portandvalve>();

    public caseportData() { }

    public caseportData(portandvalve a, portandvalve b)
    {
        nodes = new List<portandvalve> { a, b };
    }
}

/// <summary>
/// 用于封装方法二的返回结果
/// </summary>
public class ConnectionReport
{
    public string DetailReport; // 详情字符串
    public int ErrorCount;      // 错误数量（漏连）
    public int OverCount;       // 错误数量（多连）
}
