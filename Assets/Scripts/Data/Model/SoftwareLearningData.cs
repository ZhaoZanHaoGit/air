using System;

public class SoftwareLearningData
{
    public int ID { get; set; }
    public int SoftID { get; set; }
    public string SoftName { get; set; }
    public string SchoolName { get; set; }
    public string ClassInfo { get; set; }
    public string Account { get; set; }
    public string UserName { get; set; }
    /// <summary>
    /// 科目类型（认知、实训、考核）
    /// </summary>
    public int CourseType { get; set; }
    public int CourseID { get; set; }
    /// <summary>
    /// 科目名称
    /// </summary>
    public string CourseName { get; set; }
    /// <summary>
    /// 学习次数
    /// </summary>
    public int LearnCount { get; set; }
    /// <summary>
    /// 学习时长
    /// </summary>
    public int LearnTime { get; set; }

    public string Score { get; set; }
    /// <summary>
    /// 操作工单 json格式
    /// </summary>
    public string WorkOrder { get; set; }
    /// <summary>
    /// 操作内容 json格式
    /// </summary>
    public string OperationContent { get; set; }

    public DateTime Time { get; set; }
}
