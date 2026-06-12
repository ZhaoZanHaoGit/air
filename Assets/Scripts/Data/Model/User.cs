
public class User
{
    /// <summary>
    /// 上传时读取编号排序
    /// </summary>
    public int Number { get; set; }
    /// <summary>
    /// 账号
    /// </summary>
    public string Account { get; set; }
    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// 姓名
    /// </summary>
    public string Username { get; set; }
    /// <summary>
    /// 性别
    /// </summary>
    public int Sex { get; set; }

    /// <summary>
    /// 学校名称
    /// </summary>
    public string School_name { get; set; }
    /// <summary>
    /// 班级（学生班级名称教师是管理班级）
    /// </summary>
    public string Classinfo { get; set; }
    /// <summary>
    /// 学生代表专业教师代表职务
    /// </summary>
    public string Position { get; set; }
    /// <summary>
    /// 手机号
    /// </summary>
    public string Phonenumber { get; set; }
    public string Email { get; set; }
    /// <summary>
    /// 0超级管理员1管理员2教师3学生
    /// </summary>
    public int Usertype { get; set; }
    /// <summary>
    /// 登录次数
    /// </summary>
    public int Logincount { get; set; }
    /// <summary>
    /// 登录时间
    /// </summary>
    public string Logintime { get; set; }
    /// <summary>
    /// 注册时间
    /// </summary>
    public string Registertime { get; set; }
    public string ServerIP { get; set; }

}
