using System.Collections.Generic;
using System.Text;
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
        int part3score = (part2score1 >= part2ScoreMax)
            ? UnityEngine.Random.Range(0, part3ScoreMax + 1)
            : 0;

        finalscore = part1score + part2score1 + part3score;
        uiData.setPanelData(part1score.ToString(), part2score1.ToString(), part3score.ToString(), finalscore.ToString(), report.ErrorCount.ToString(), report.OverCount.ToString());
        trainingPanelUI.startTraining = false;
    }

    public void UpdateScore()
    {

        if (AppController.Instance.loginUser.Usertype == (int)UserType.学生)
        {
            Debug.Log("上传认知数据");
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
                WorkOrder = "",
                OperationContent = ""
            };
            _ = NetHelper.Instance.AddSoftLearningData(softwareLearningData, code =>
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
        trainingPanelUI.ExamUIFinish();

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
