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
    /// 【新增方法】获取当前选定案例对应的标准答案连线列表
    /// </summary>
    /// <returns>返回当前案例的标准连线列表；若获取失败则返回 null</returns>
    public List<caseportData> GetCurrentStandardAnswers()
    {
        // 1. 确保已选择任务案例
        if (trainingPanel == null || trainingPanel.currentTrainingCase == null)
        {
            Debug.LogWarning("未选择当前训练案例，或 TrainingPanel 未正确引用！");
            return null;
        }

        int currentCaseNamber = trainingPanel.currentTrainingCase.CaseNamber;

        // 2. 将 CaseNamber 映射为 List 的索引 (假设编号从1开始：Namber 1 -> Index 0)
        int listIndex = currentCaseNamber - 1;

        // 3. 校验数据库引用
        if (allCases == null)
        {
            Debug.LogError("标准答案数据库 (answersDb) 为空！");
            return null;
        }

        // 4. 校验索引是否越界
        if (listIndex < 0 || listIndex >= allCases.Count)
        {
            Debug.LogError($"无法找到 CaseNamber: {currentCaseNamber} 对应的标准答案列表。当前库中仅有 {allCases.Count} 个案例。");
            return null;
        }

        // 5. 返回获取到的标准答案列表
        return allCases[listIndex];
    }
    /// <summary>
    /// 验证给定的单条连接数据是否包含在标准答案中
    /// </summary>
    /// <param name="targetData">玩家实际连接的一条线段数据</param>
    /// <returns>返回 true 表示连接正确，false 表示连错或多连</returns>
    public bool VerifyPortConnection(caseportData targetData)
    {
        // 1. 确保已选择任务案例
        if (trainingPanel.currentTrainingCase == null)
        {
            Debug.LogWarning("未选择当前训练案例！");
            return false;
        }

        int currentCaseNamber = trainingPanel.currentTrainingCase.CaseNamber;

        // 2. 将 CaseNamber 映射为 List 的索引
        // 假设 JSON 中的 CaseNamber 是 1, 2, 3... 对应 allCases 的索引 0, 1, 2...
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

        // 3. 获取当前案例的标准答案集合
        List<caseportData> standardAnswers = allCases[listIndex];

        // 4. 遍历比对（支持正反向）
        foreach (caseportData standard in standardAnswers)
        {
            // 正向匹配：A-A, B-B
            // 注意：如果 portandvalve 是 class 或 struct，建议使用 .Equals() 替代 ==
            // 除非你在 portandvalve 中重载了 == 运算符。
            bool isDirectMatch = (standard.portA.Equals(targetData.portA) && standard.portB.Equals(targetData.portB));

            // 反向匹配：A-B, B-A (调换端口)
            bool isReverseMatch = (standard.portA.Equals(targetData.portB) && standard.portB.Equals(targetData.portA));

            if (isDirectMatch || isReverseMatch)
            {
                return true; // 找到匹配项，直接判定为包含
            }
        }

        // 遍历结束未找到
        return false;
    }
    /// <summary>
    /// 方法一：仅判断玩家列表中的每一条连线是否存在于标准答案中，输出状态文本。
    /// </summary>
    public string CheckConnectionsBasic(List<caseportData> userList, List<caseportData> standardList)
    {
        if (userList == null || standardList == null) return "数据为空。";

        StringBuilder sb = new StringBuilder();

        foreach (var userItem in userList)
        {
            // === 【新增逻辑】判断是否为 L 口连线 ===
            if (IsLPortConnection(userItem))
            {
                sb.AppendLine($"连线 [{FormatPort(userItem.portA)} <-> {FormatPort(userItem.portB)}] : <color=#00FFFF>L口连线 (仅记录)</color>");
                continue;
            }
            bool exists = false;
            foreach (var stdItem in standardList)
            {
                if (IsConnectionMatch(userItem, stdItem))
                {
                    exists = true;
                    break;
                }
            }

            string status = exists ? "<color=#00FF00>存在于标准答案中</color>" : "<color=#FF0000>不存在 (多余连线)</color>";
            sb.AppendLine($"连线 [{FormatPort(userItem.portA)} <-> {FormatPort(userItem.portB)}] : {status}");
        }

        return sb.ToString();
    }

    public void TiShi(TextMeshProUGUI text)
    {
        text.text = CheckConnectionsBasic(SimulationLoop.Instance.currentPostsData, GetCurrentStandardAnswers());
        RectTransform Rect = (RectTransform)text.transform.parent;
        if (Rect != null)
        {
            // 强行重建当前物体以及所有子物体的布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
        }
    }
    public void xunxian(TextMeshProUGUI text)
    {

        text.text = EvaluateAllConnections(SimulationLoop.Instance.currentPostsData, GetCurrentStandardAnswers()).DetailReport;
        RectTransform Rect = (RectTransform)text.transform.parent;
        if (Rect != null)
        {
            // 强行重建当前物体以及所有子物体的布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
        }

    }
    public ConnectionReport ExamEvaluate()
    {
        ConnectionReport report = EvaluateAllConnections(SimulationLoop.Instance.currentPostsData, GetCurrentStandardAnswers());
        return report;


    }


    /// <summary>
    /// 方法二：进阶评估。不仅检查玩家的线是否正确，还找出标准答案中遗漏的线，并统计错误总数。
    /// </summary>
    public ConnectionReport EvaluateAllConnections(List<caseportData> userList, List<caseportData> standardList)
    {
        ConnectionReport result = new ConnectionReport();
        if (userList == null || standardList == null) return result;

        StringBuilder sb = new StringBuilder();
        int errorCount = 0;
        int overCount = 0;

        // 复制一份标准答案列表，用于追踪哪些标准连线已经被玩家连上了
        List<caseportData> unmatchedStandard = new List<caseportData>(standardList);

        // 1. 遍历玩家连线，检查多连或错连
        foreach (var userItem in userList)
        {
            // === 【新增逻辑】判断是否为 L 口连线 ===
            if (IsLPortConnection(userItem))
            {
                // 仅输出详情，不增加错误数量，也不去标准答案里比对
                sb.AppendLine($"连线 [{FormatPort(userItem.portA)} <-> {FormatPort(userItem.portB)}] : <color=#00FFFF>L口连线 (仅记录，不参与判定)</color>");
                continue;
            }
            // 在未匹配的标准答案中寻找匹配项
            int matchIndex = unmatchedStandard.FindIndex(std => IsConnectionMatch(userItem, std));

            if (matchIndex != -1)
            {
                // 找到了匹配，说明连线正确。从待匹配列表中移除，防止一条标准答案被多次重复匹配
                sb.AppendLine($"连线 [{FormatPort(userItem.portA)} <-> {FormatPort(userItem.portB)}] : <color=#00FF00>正确</color>");
                unmatchedStandard.RemoveAt(matchIndex);
            }
            else
            {
                // 没找到匹配，说明是多余的连线，或是重复连了同一根线
                sb.AppendLine($"连线 [{FormatPort(userItem.portA)} <-> {FormatPort(userItem.portB)}] : <color=#FF0000>错误 (多余或重复)</color>");
                overCount++;
            }
        }

        // 2. 遍历结束后，unmatchedStandard 中剩下的就是玩家遗漏的连线
        foreach (var missingItem in unmatchedStandard)
        {
            sb.AppendLine($"连线 [{FormatPort(missingItem.portA)} <-> {FormatPort(missingItem.portB)}] : <color=#FFAA00>遗漏 (标准答案中有但未连接)</color>");
            errorCount++;
        }

        result.DetailReport = sb.ToString();
        result.ErrorCount = errorCount;
        result.OverCount = overCount;

        return result;
    }

    // ================= 辅助方法 =================

    /// <summary>
    /// 【新增方法】判断这根线的两端是否含有 L 口
    /// </summary>
    private bool IsLPortConnection(caseportData item)
    {
        // 只要 portA 或者 portB 的端口名是 L，就返回 true
        return item.portA.port == PneumaticPortName.L || item.portB.port == PneumaticPortName.L;
    }

    /// <summary>
    /// 判定两条线段是否等效（支持正向与反向判定）
    /// </summary>
    private bool IsConnectionMatch(caseportData a, caseportData b)
    {
        // 正向匹配：A-A, B-B
        bool direct = (a.portA.valve == b.portA.valve && a.portA.port == b.portA.port) &&
                      (a.portB.valve == b.portB.valve && a.portB.port == b.portB.port);

        // 反向匹配：A-B, B-A
        bool reverse = (a.portA.valve == b.portB.valve && a.portA.port == b.portB.port) &&
                       (a.portB.valve == b.portA.valve && a.portB.port == b.portA.port);

        return direct || reverse;
    }

    /// <summary>
    /// 格式化端口名称用于文本输出（根据你 portandvalve 的实际结构进行修改）
    /// </summary>
    private string FormatPort(portandvalve port)
    {
        // 假设 portandvalve 有 name 字段。如果是对象，直接返回 port.ToString() 也可以
        if (port == null) return "未知端口";

        // 此处请替换为你实际获取元器件/端口名字的属性
        return port.GetPortAndValveInfo();
    }
}
[System.Serializable]
public struct caseportData
{
    public portandvalve portA; // 例如："双气控阀"
    public portandvalve portB;         // 例如：2
}
// 用于封装方法二的返回结果
public class ConnectionReport
{
    public string DetailReport; // 详情字符串
    public int ErrorCount;      // 错误数量（ 漏连）
    public int OverCount;      // 错误数量（多连）
}
