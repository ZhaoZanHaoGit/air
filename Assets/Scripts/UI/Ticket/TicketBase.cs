using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class CheckItem
{
    public string itemName; // 项目名称
    public TicketValueType itemType; // 类型: "checkbox", "input"
    public List<string> options; // 选项列表（"是/否" 或 "正常/异常"）
    public AnswerType answerType;
    public string normalAnswer;        //标准值答案
    public string minRange;            //最小标准值
    public string maxRange;            //最大标准值
    public string selectedOption; // 选择的选项（checkbox）
    public string inputValue; // 输入的值（input）

    public CheckItem(string name, TicketValueType type, AnswerType _answerType, string _answer, List<string> optionList = null, string _min = null, string _max = null) {
        itemName = name;
        itemType = type;
        options = optionList ?? new List<string>();
        answerType = _answerType;
        normalAnswer = _answer;
        minRange = _min;
        maxRange = _max;
    }
    // 检查该项是否已完成
    public bool IsCompleted() {
        if (itemType == TicketValueType.checkbox) {
            return !string.IsNullOrEmpty(selectedOption);
        }
        else if (itemType == TicketValueType.input) {
            return !string.IsNullOrEmpty(inputValue);
        }
        return false;
    }

    //public bool Result()
    //{
    //    switch (itemType)
    //    {
    //        case TicketValueType.checkbox:
    //            string selectAnswer = string.IsNullOrEmpty(selectedOption) ? "未选择" : selectedOption;
    //            if (selectAnswer == "未选择")
    //            {
    //                错误
    //                return false;
    //            }
    //            else
    //            {
    //                if (selectAnswer != normalAnswer)
    //                {
    //                    错误
    //                    return false;
    //                }
    //            }
    //            break;
    //        case TicketValueType.input:
    //            string answer = string.IsNullOrEmpty(inputValue) ? "未输入" : inputValue;
    //            switch (answerType)
    //            {
    //                case AnswerType.标准值:

    //                    if (normalAnswer == "判定" && answer == "未输入")
    //                    {
    //                        错误
    //                        return false;
    //                    }
    //                    else if (normalAnswer == "判定" && answer != "未输入")
    //                    {

    //                    }
    //                    else
    //                    {
    //                        if (answer != normalAnswer)
    //                        {
    //                            错误
    //                            return false;
    //                        }
    //                    }
    //                    break;
    //                case AnswerType.范围值:
    //                    float min = 0;
    //                    float max = 0;
    //                    float select = 0;
    //                    bool isSuccessMin = float.TryParse(minRange, out min);
    //                    bool isSuccessMax = float.TryParse(maxRange, out max);
    //                    bool isSuccessSelect = float.TryParse(answer, out select);
    //                    if (isSuccessMin && isSuccessMax && isSuccessSelect)
    //                    {
    //                        if (select >= min && select <= max)
    //                        {

    //                        }
    //                        else
    //                        {
    //                            错误
    //                            return false;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        错误
    //                        return false;
    //                    }
    //                    break;
    //            }
    //            break;
    //    }
    //}

    public bool Result() {
        switch (itemType) {
            case TicketValueType.checkbox:
                return CheckCheckboxResult();

            case TicketValueType.input:
                return CheckInputResult();

            default:
                return false;  // Return false if the item type is not recognized
        }
    }

    private bool CheckCheckboxResult() {
        string selectAnswer = string.IsNullOrEmpty(selectedOption) ? "未选择" : selectedOption;
        // Return false if no selection or if the selected option doesn't match the normal answer
        return selectAnswer != "未选择" && selectAnswer == normalAnswer;
    }

    private bool CheckInputResult() {
        string answer = string.IsNullOrEmpty(inputValue) ? "未输入" : inputValue;

        switch (answerType) {
            case AnswerType.标准值:
                return CheckStandardValueResult(answer);

            case AnswerType.范围值:
                return CheckRangeValueResult(answer);

            default:
                return false; // If the answerType is unrecognized, return false
        }
    }

    private bool CheckStandardValueResult(string answer) {
        if (normalAnswer == "判定") {
            // If normalAnswer is "判定", it's only correct if no input or the answer matches
            return answer != "未输入" || answer == normalAnswer;
        }
        // If normalAnswer isn't "判定", the answer must match the standard answer
        return answer == normalAnswer;
    }

    private bool CheckRangeValueResult(string answer) {
        // Try parsing the range and the answer
        bool isSuccessMin = float.TryParse(minRange, out float min);
        bool isSuccessMax = float.TryParse(maxRange, out float max);
        bool isSuccessSelect = float.TryParse(answer, out float select);

        // Return false if parsing fails or if the selected value is out of range
        return isSuccessMin && isSuccessMax && isSuccessSelect && select >= min && select <= max;
    }

}

[Serializable]
public class CheckSection
{
    public string sectionName; // 部分名称
    public List<CheckItem> checkItems; // 检查项列表

    public CheckSection(string name) {
        sectionName = name;
        checkItems = new List<CheckItem>();
    }
    // 检查该部分是否全部完成
    public bool IsCompleted() {
        foreach (var item in checkItems) {
            if (!item.IsCompleted()) {
                return false;
            }
        }
        return true;
    }
    // 获取未完成的项目数量
    public int GetIncompleteCount() {
        int count = 0;
        foreach (var item in checkItems) {
            if (!item.IsCompleted()) {
                count++;
            }
        }
        return count;
    }
}

[Serializable]
public class WorkOrder
{
    public string orderType; // 工单类型
    public string title; // 标题
    public List<CheckSection> sections = new List<CheckSection>();
    public DateTime createTime;
    public DateTime updateTime;

    public WorkOrder(string type, string orderTitle) {
        orderType = type;
        title = orderTitle;
        createTime = DateTime.Now;
        updateTime = DateTime.Now;
    }
    // 检查整个工单是否全部完成
    public bool IsCompleted() {
        foreach (var section in sections) {
            if (!section.IsCompleted()) {
                return false;
            }
        }
        return true;
    }
    // 获取未完成的项目总数
    public int GetTotalIncompleteCount() {
        int total = 0;
        foreach (var section in sections) {
            total += section.GetIncompleteCount();
        }
        return total;
    }
    // 获取完成进度百分比
    public float GetCompletionPercentage() {
        int totalItems = 0;
        int completedItems = 0;

        foreach (var section in sections) {
            foreach (var item in section.checkItems) {
                totalItems++;
                if (item.IsCompleted()) {
                    completedItems++;
                }
            }
        }

        return totalItems > 0 ? (float)completedItems / totalItems * 100 : 0;
    }
}
public enum TicketValueType
{
    None,
    checkbox,
    input
}
public enum AnswerType
{
    None,
    标准值,
    范围值,
    复合值
}
public class TicketBase : MonoBehaviour
{

}

/// <summary>
/// 操作步骤详情
/// </summary>
[Serializable]
public class OperationStepsData
{
    public List<StepInfo> StepInfos { get; set; }
}
[Serializable]
public class StepInfo
{
    public string StepName { get; set; }
    public DateTime CreatTime { get; set; }
}
