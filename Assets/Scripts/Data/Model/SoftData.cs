using System;
/// <summary>
/// 学习软件数据,登录后提交
/// </summary>
public class SoftData 
{
    public int SoftID { get; set; }
    public string SoftName { get; set; } 
    public int LoginCount  { get; set; }
    public DateTime LoginTime { get; set; }
    public string Account { get; set; }
    public string UserName { get; set; }
    public string SchoolName { get; set; }
    public string ClassInfo { get; set; }
    public int IsOnline​ { get; set; }//是否在线
    public int UserType { get; set; }
}
