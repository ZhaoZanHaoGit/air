using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine;

public class PneumaticEvaluator : MonoBehaviour
{
    [Header("评分设置")]
    public int part1ScoreMax = 30;         // 初始满分
    public int deductPerMissing = 10;    // 少领/漏领一个扣除的分数
    public int deductPerExtra = 10;      // 多领/错领一个扣除的分数
    public int part2ScoreMax = 30;
    public int part3ScoreMax = 40;
    public casesPart2Answer part2Answer;
    public TraniningPanel trainingPanelUI;
    [HideInInspector]
    private ConnectionReport report;
    public int part1Score, part3Score;
    [HideInInspector]
    public bool part1Evaluated = false;   // 标记零部件领取是否经过评分
    int finalscore;// 零部件领取得分
    float m_Timer=0; // 记录训练时间

    // ===== 文件上传相关 =====
    private string workOrderPath = "";           // 工单服务器路径
    private string operationContentPath = "";    // 操作内容服务器路径
    private string serverFolderPath = "";        // 服务器上传目录
    // 捕获的JSON（在清理前捕获，避免数据丢失）
    private string capturedWorkOrderJson = "";
    private string capturedOperationContentJson = "";
    // 待提交的数据（上传完成后提交）
    private SoftwareLearningData pendingSubmitData = null;

    void Start()
    {
        var pathData = AppController.Instance?.pathDatas?.Find(a => a.ID == 9);
        if (pathData != null)
            serverFolderPath = pathData.Path;
    }

    private void Update()
    {
        if (trainingPanelUI.startTraining)
        {
            m_Timer += Time.deltaTime;
        }
    }

    /// <summary>
    /// 评估结果数据结构
    /// </summary>
    public class EvaluationResult
    {
        public int FinalScore;
        public string DetailReport;
        public bool IsPerfect; // 是否完全正确
    }
    public void UpdateFinalScore(SetUIData uiData)
    {
        // 第一部分：未经评分流程（没选元器件/没点确认），直接 0 分
        int part1score = part1Evaluated ? part1Score : 0;

        // 第二部分：根据接线判定结果计算
        report = part2Answer.ExamEvaluate();
        int part2score1 = Mathf.Clamp(
            part2ScoreMax - (report.OverCount * deductPerExtra) - (report.ErrorCount * deductPerMissing),
            0, part2ScoreMax);

        // 第三部分：第二部分满分才给随机分，否则 0 分
        part3Score = (part2score1 >= part2ScoreMax)
            ? UnityEngine.Random.Range(0, part3ScoreMax + 1)
            : 0;

        finalscore = part1score + part2score1 + part3Score;
        uiData.setPanelData(part1score.ToString(), part2score1.ToString(), part3Score.ToString(), finalscore.ToString(), report.ErrorCount.ToString(), report.OverCount.ToString());
        trainingPanelUI.startTraining = false;
    }

    public void UpdateScore()
    {

        if (AppController.Instance.loginUser.Usertype == (int)UserType.学生)
        {
            Debug.Log("上传考核数据");
            CaptureUploadData();  // 在 ExamUIFinish 清理之前捕获评估数据

            SoftwareLearningData softwareLearningData = new SoftwareLearningData
            {
                SoftID = (int)Defines.softInfo,
                SoftName = Defines.softInfo.ToString(),
                SchoolName = AppController.Instance.loginUser.School_name,
                ClassInfo = AppController.Instance.loginUser.Classinfo,
                Account = AppController.Instance.loginUser.Account,
                UserName = AppController.Instance.loginUser.Username,
                CourseType = (int)AppController.Instance.courseType,
                CourseID = (int)AppController.Instance.eVSType,
                CourseName = AppController.Instance.eVSType.ToString(),
                LearnTime = (int)m_Timer,
                Score = finalscore.ToString(),
                // WorkOrder 和 OperationContent 由 StartUploadAndSubmit 填充
            };
            StartUploadAndSubmit(softwareLearningData);
        }
        trainingPanelUI.ExamUIFinish();

    }

    // ===== 工单生成与文件上传 =====

    /// <summary>
    /// 在清理操作前捕获评估数据，生成工单JSON和操作步骤JSON
    /// </summary>
    public void CaptureUploadData()
    {
        // 如果 report 为空（test 模式未经过 UpdateFinalScore），即时评估
        if (report == null || report.DetailReport == null)
        {
            report = part2Answer.ExamEvaluate();
        }
        capturedWorkOrderJson = GenerateWorkOrderJson();
        capturedOperationContentJson = GenerateOperationStepsJson();
    }

    /// <summary>
    /// 基于 PneumaticEvaluator 三部分评分结果创建工单
    /// </summary>
    public WorkOrder CreatePneumaticWorkOrder()
    {
        string caseName = trainingPanelUI.currentTrainingCase?.Casname ?? "气动训练";
        WorkOrder order = new WorkOrder("1", caseName);

        // ---- Section 1: 零部件领取 ----
        CheckSection section1 = new CheckSection("零部件领取");
        var expectedList = trainingPanelUI.currentTrainingCase?.ComponentList;
        // 汇总标准答案
        var expectedDict = new Dictionary<string, int>();
        if (expectedList != null)
        {
            foreach (var dict in expectedList)
            {
                foreach (var kvp in dict)
                {
                    if (expectedDict.ContainsKey(kvp.Key))
                        expectedDict[kvp.Key] += kvp.Value;
                    else
                        expectedDict[kvp.Key] = kvp.Value;
                }
            }
        }
        // 获取实际领取
        var actualPicked = new Dictionary<string, int>();
        if (ItemManager.Instance != null && ItemManager.Instance.inventory != null)
        {
            foreach (var item in ItemManager.Instance.inventory)
            {
                actualPicked[item.Key.itemName] = item.Value;
            }
        }
        // 汇总实际领取中存在但标准答案中没有的元件
        foreach (var kvp in actualPicked)
        {
            if (!expectedDict.ContainsKey(kvp.Key))
                expectedDict[kvp.Key] = 0;
        }
        foreach (var kvp in expectedDict)
        {
            int expectedQty = kvp.Value;
            int actualQty = actualPicked.ContainsKey(kvp.Key) ? actualPicked[kvp.Key] : 0;
            string status;
            if (expectedQty == actualQty)
                status = $"应领{expectedQty}个，实领{actualQty}个，正确";
            else if (actualQty > expectedQty)
                status = expectedQty == 0
                    ? $"应领{expectedQty}个，实领{actualQty}个，错领{actualQty}个"
                    : $"应领{expectedQty}个，实领{actualQty}个，多领{actualQty - expectedQty}个";
            else
                status = $"应领{expectedQty}个，实领{actualQty}个，少领{expectedQty - actualQty}个";

            CheckItem item = new CheckItem(kvp.Key, TicketValueType.input, AnswerType.标准值, "正确");
            item.inputValue = status;
            section1.checkItems.Add(item);
        }
        order.sections.Add(section1);

        // ---- Section 2: 回路连接 ----
        CheckSection section2 = new CheckSection("回路连接");
        if (report != null)
        {
            // 计算正确连接数
            int totalConnections = 0;
            if (SimulationLoop.Instance != null && SimulationLoop.Instance.currentPostsData != null)
            {
                foreach (var conn in SimulationLoop.Instance.currentPostsData)
                {
                    // 过滤 L 口连线（仅记录，不参与判定）
                    bool hasLPort = false;
                    if (conn?.nodes != null)
                    {
                        foreach (var node in conn.nodes)
                        {
                            if (node == null) continue;
                            if (node.valve == ValveName.三通 || node.valve == ValveName.四通) continue;
                            if (node.port == PneumaticPortName.L) { hasLPort = true; break; }
                        }
                    }
                    if (!hasLPort)
                        totalConnections++;
                }
            }
            int correctCount = totalConnections - report.OverCount;
            if (correctCount < 0) correctCount = 0;

            CheckItem correctItem = new CheckItem("正确连接数", TicketValueType.input, AnswerType.标准值, correctCount.ToString());
            correctItem.inputValue = correctCount.ToString();
            section2.checkItems.Add(correctItem);

            CheckItem overItem = new CheckItem("多余连接数", TicketValueType.input, AnswerType.标准值, "0");
            overItem.inputValue = report.OverCount.ToString();
            section2.checkItems.Add(overItem);

            CheckItem errorItem = new CheckItem("遗漏连接数", TicketValueType.input, AnswerType.标准值, "0");
            errorItem.inputValue = report.ErrorCount.ToString();
            section2.checkItems.Add(errorItem);

            // 连接详情（去除 HTML 标签）
            string cleanReport = StripHtmlTags(report.DetailReport ?? "");
            CheckItem detailItem = new CheckItem("连接详情", TicketValueType.input, AnswerType.标准值, "");
            detailItem.inputValue = cleanReport;
            section2.checkItems.Add(detailItem);
        }
        order.sections.Add(section2);

        // ---- Section 3: 评分结果 ----
        CheckSection section3 = new CheckSection("评分结果");
        int p1 = part1Evaluated ? part1Score : 0;
        int p2 = 0;
        if (report != null)
        {
            p2 = Mathf.Clamp(
                part2ScoreMax - (report.OverCount * deductPerExtra) - (report.ErrorCount * deductPerMissing),
                0, part2ScoreMax);
        }
        int p3 = (p2 >= part2ScoreMax) ? part3Score : 0;
        int total = p1 + p2 + p3;

        CheckItem s1 = new CheckItem("零部件领取得分", TicketValueType.input, AnswerType.标准值, "");
        s1.inputValue = $"{p1}/{part1ScoreMax}";
        section3.checkItems.Add(s1);

        CheckItem s2 = new CheckItem("回路连接得分", TicketValueType.input, AnswerType.标准值, "");
        s2.inputValue = $"{p2}/{part2ScoreMax}";
        section3.checkItems.Add(s2);

        CheckItem s3 = new CheckItem("附加得分", TicketValueType.input, AnswerType.标准值, "");
        s3.inputValue = $"{p3}/{part3ScoreMax}";
        section3.checkItems.Add(s3);

        CheckItem sTotal = new CheckItem("总分", TicketValueType.input, AnswerType.标准值, "");
        sTotal.inputValue = total.ToString();
        section3.checkItems.Add(sTotal);

        order.sections.Add(section3);

        return order;
    }

    /// <summary>
    /// 去除 HTML 富文本标签
    /// </summary>
    private string StripHtmlTags(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        StringBuilder sb = new StringBuilder();
        bool insideTag = false;
        foreach (char c in input)
        {
            if (c == '<') { insideTag = true; continue; }
            if (c == '>') { insideTag = false; continue; }
            if (!insideTag) sb.Append(c);
        }
        return sb.ToString();
    }

    /// <summary>
    /// 生成工单 JSON 字符串
    /// </summary>
    public string GenerateWorkOrderJson()
    {
        WorkOrder order = CreatePneumaticWorkOrder();
        order.updateTime = DateTime.Now;
        return JsonConvert.SerializeObject(order);
    }

    /// <summary>
    /// 从 currentPostsData 生成操作步骤 JSON 字符串
    /// </summary>
    public string GenerateOperationStepsJson()
    {
        OperationStepsData stepsData = new OperationStepsData();
        stepsData.StepInfos = new List<StepInfo>();

        if (SimulationLoop.Instance != null && SimulationLoop.Instance.currentPostsData != null)
        {
            foreach (var conn in SimulationLoop.Instance.currentPostsData)
            {
                if (conn?.nodes == null || conn.nodes.Count < 2) continue;

                StringBuilder sb = new StringBuilder();
                sb.Append("连接: ");
                for (int i = 0; i < conn.nodes.Count; i++)
                {
                    if (conn.nodes[i] == null) continue;
                    if (i > 0) sb.Append(" → ");
                    sb.Append(conn.nodes[i].GetPortAndValveInfo());
                }

                StepInfo step = new StepInfo
                {
                    StepName = sb.ToString(),
                    CreatTime = DateTime.Now
                };
                stepsData.StepInfos.Add(step);
            }
        }

        return JsonConvert.SerializeObject(stepsData);
    }

    /// <summary>
    /// 启动文件上传流程：保存工单/步骤到文件 → 上传到服务器 → 提交 SoftwareLearningData
    /// </summary>
    public void StartUploadAndSubmit(SoftwareLearningData data)
    {
        pendingSubmitData = data;
        workOrderPath = "";
        operationContentPath = "";

        // 保存工单到本地文件
        string gdFilePath = Path.Combine(Application.persistentDataPath, "气动工单.txt");
        File.WriteAllText(gdFilePath, capturedWorkOrderJson);

        if (string.IsNullOrEmpty(serverFolderPath))
        {
            // 无服务器路径，直接用 JSON 内容填充
            workOrderPath = capturedWorkOrderJson;
            operationContentPath = capturedOperationContentJson;
            SubmitPendingData();
            return;
        }

        // 上传工单文件
        List<string> paths = new List<string> { gdFilePath };
        PhpHelper.Instance.OnUploadFiles(paths, serverFolderPath, UpLoadGDFilesBack);
    }

    private void UpLoadGDFilesBack(string filename, string filepath, FileType type)
    {
        workOrderPath = filepath;

        // 保存操作步骤到本地文件
        string spFilePath = Path.Combine(Application.persistentDataPath, "气动操作步骤.txt");
        File.WriteAllText(spFilePath, capturedOperationContentJson);

        if (string.IsNullOrEmpty(serverFolderPath))
        {
            operationContentPath = capturedOperationContentJson;
            SubmitPendingData();
            return;
        }

        List<string> paths = new List<string> { spFilePath };
        PhpHelper.Instance.OnUploadFiles(paths, serverFolderPath, UpLoadBZFilesBack);
    }

    private void UpLoadBZFilesBack(string filename, string filepath, FileType type)
    {
        operationContentPath = filepath;
        SubmitPendingData();
    }

    private void SubmitPendingData()
    {
        if (pendingSubmitData == null) return;

        pendingSubmitData.WorkOrder = workOrderPath;
        pendingSubmitData.OperationContent = operationContentPath;

        _ = NetHelper.Instance.AddSoftLearningData(pendingSubmitData, code =>
        {
            if (code == -1)
            {
                UIManager.Instance.OpenMessageBoxUI("提示", "发送数据失败！", 0, EnumMessageBoxType.OK);
            }
            else
            {
                m_Timer = 0;
            }
        });
    }

    /// <summary>
    /// 执行零部件领取比对与评分
    /// </summary>
    /// <param name="expectedList">来自 JSON 的 ComponentList (标准答案)</param>
    /// <param name="actualPicked">玩家实际领取的零部件集合</param>
    /// <returns>包含得分和详情报告的结果对象</returns>
    public EvaluationResult EvaluateComponents(List<Dictionary<string, int>> expectedList, Dictionary<string, int> actualPicked)
    {
        EvaluationResult result = new EvaluationResult();
        result.FinalScore = part1ScoreMax;
        StringBuilder reportBuilder = new StringBuilder();

        // 1. 汇总标准答案 (将 List<Dictionary> 展平并合并同类项)
        Dictionary<string, int> expectedDict = new Dictionary<string, int>();
        if (expectedList != null)
        {
            foreach (var dict in expectedList)
            {
                foreach (var kvp in dict)
                {
                    if (expectedDict.ContainsKey(kvp.Key))
                        expectedDict[kvp.Key] += kvp.Value;
                    else
                        expectedDict[kvp.Key] = kvp.Value;
                }
            }
        }

        // 2. 收集所有出现过的零部件名称（标准要求 + 实际领取的并集）
        HashSet<string> allComponentNames = new HashSet<string>(expectedDict.Keys);
        if (actualPicked != null)
        {
            allComponentNames.UnionWith(actualPicked.Keys);
        }

        int totalErrors = 0; // 记录总差异数量

        // 3. 逐个比对差异
        foreach (string compName in allComponentNames)
        {
            int expectedQty = expectedDict.ContainsKey(compName) ? expectedDict[compName] : 0;
            int actualQty = actualPicked != null && actualPicked.ContainsKey(compName) ? actualPicked[compName] : 0;

            if (expectedQty == actualQty)
            {
                // 数量一致，跳过
                continue;
            }

            // 存在差异
            int diff = actualQty - expectedQty;

            if (diff > 0)
            {
                // 多领了 (或标准答案里根本没有这个件，属于错领)
                int deduction = diff * deductPerExtra;
                result.FinalScore -= deduction;
                totalErrors += diff;

                if (expectedQty == 0)
                {
                    reportBuilder.AppendLine($"<color=#FF5555>【错领】</color> 领了 {diff} 个多余的 [{compName}]。");// (扣 {deduction} 分)
                }
                else
                {
                    reportBuilder.AppendLine($"<color=#FFAA00>【多领】</color> [{compName}] 多领了 {diff} 个 (应领 {expectedQty}，实领 {actualQty})。");// (扣 {deduction} 分)
                }
            }
            else if (diff < 0)
            {
                // 少领了 (或根本没领，漏领)
                int missingCount = Mathf.Abs(diff);
                int deduction = missingCount * deductPerMissing;
                result.FinalScore -= deduction;
                totalErrors += missingCount;

                reportBuilder.AppendLine($"<color=#FF5555>【少领】</color> [{compName}] 少领了 {missingCount} 个 (应领 {expectedQty}，实领 {actualQty})。");// (扣 {deduction} 分)
            }
        }

        // 4. 防止分数扣成负数
        result.FinalScore = Mathf.Max(0, result.FinalScore);

        // 5. 总结报告
        if (totalErrors == 0)
        {
            result.IsPerfect = true;
            reportBuilder.Insert(0, "<color=#00FF00><b>验证通过！零部件选取完全正确。</b></color>\n\n");
        }
        else
        {
            result.IsPerfect = false;
            reportBuilder.Insert(0, $"<color=#FF0000><b>发现错误！总计 {totalErrors} 处数量差异。</b></color>\n\n");
            // reportBuilder.AppendLine($"\n<b>最终得分：{result.FinalScore} / {part1ScoreMax}</b>");
        }

        result.DetailReport = reportBuilder.ToString();
        return result;
    }
}
