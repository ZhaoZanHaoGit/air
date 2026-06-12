//智能体数据
using System;

public class AgentData
{
    public int ID { get; set; }
    public string SchoolName { get; set; }
    public string Account { get; set; }
    public string AgentName { get; set; }
    public int Type { get; set; }

    /// <summary>
    /// 缩略图地址
    /// </summary>
    public string ThumBnailPath { get; set; }
    /// <summary>
    /// 缩列图所在文件夹
    /// </summary>
    public string ThumbnailFolder { get; set; }

    public string Des { get; set; }
    /// <summary>
    /// 共享的班级信息，多个班级用|分隔
    /// </summary>
    public string Classinfo { get; set; }
    /// <summary>
    /// 教师是否共享 0：不共享 1：共享
    /// </summary>
    public int IsCommon { get; set; }

    public string AgentURL { get; set; }
    public DateTime CreateTime { get; set; }
}
