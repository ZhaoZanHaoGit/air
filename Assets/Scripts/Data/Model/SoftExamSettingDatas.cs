using System;
using System.Collections.Generic;


[System.Serializable]
public class SoftExamSettingDatas
{
    public int SoftID { get; set; }
    public string Schoolname { get; set; }
    public string DataJson { get; set; }
}

[System.Serializable]
public class ExamSettingDatas
{
    public List<ExamSettingData> datas = new List<ExamSettingData>();
}

[System.Serializable]
public class ExamSettingData
{
    public int CourseType;
    public int AllTimer;
}