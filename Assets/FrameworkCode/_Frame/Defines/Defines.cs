/******************************************************************************
 *
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.配置文件
 *  2.全局的委托
 *  3.全局的枚举
 *
 *  Author:
 *
 *  Date:
 *
 ******************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;


#region 全局委托

public delegate void StateChangeEvent(object sender, EnumObjectState newState, EnumObjectState oldState);

public delegate void MethodAction(object args);

public delegate void OnTouchEventHandle(GameObject listener, object eventData, params object[] args);

public delegate void MessageEvent(Message msg);

#endregion

/// <summary>
/// 对象当前状态
/// </summary>
public enum EnumObjectState
{
    None,
    Initial, //初始化
    Loading, //装载中
    Ready, //准备结束
    Disabled, //过去的
    Closing, //关闭
}

/// <summary>
/// 点击事件类型事件类型
/// </summary>
public enum EnumTouchEventType
{
    OnBeginDrag,
    OnCancel,
    OnDeselect,
    OnDrag,
    OnDrop,
    OnEndDrag,
    OnInitializePotentialDrag,
    OnMove,
    OnClick,
    OnDoubleClick,
    OnDown,
    OnEnter,
    OnExit,
    OnUp,
    OnScroll,
    OnSelect,
    OnSubmit,
    OnUpdateSelected,
}

/// <summary>
/// 按钮点击可变参数的键值类型
/// </summary>
public enum EnumHashtableParamsType
{
    None = 0,
    Audio,
    LockAllClick,
    LockSelfClick,
}

/// <summary>
/// UI界面类型
/// </summary>
public enum EnumUIType
{
    None = -1,
    LoginUI, //登录界面
    MessageBoxUI, //消息提示面板
    MainMenu, //教师主菜单
    TaskMenu,        //任务菜单
    CognitiveResourcesMenuPanel,   //认知机构面板
    ResourcesMenuPanel,            //认知资源面板
    CognitiveMenuPanel,//认知菜单面板
    CognitivePanel,  //认知场景UI

    StudentMainMenu, //学生主菜单
    LoadingUI, //场景转换面板

    CongitionUI, //测试认知面板
    AdministratorPanel, //管理员端面板
    TeacherTjPanel, //教师端统计面板
    StudentTjPanel, //学生端统计面板
    TraniningPanel, //实训面板
    AssessPanel, //考核面板
    SettingPanel, //设置面板
    MethumanAIManager
}
public enum MenuPanelType
{
    None = -1,
    认知,
    实训,
    考核,
    任务,
    个人主页
}
/// <summary>
/// 场景类型
/// </summary>
public enum EnumSceneType
{
    None = -1,
    GameStart, //开始场景
    Congition, //测试认知场景
    TrainScene_PC, //实训场景
    ExamScene_PC //考核场景
}
public enum trainType
{
    None = -1,
    free, //自由实训
    test, //实训练习
    exam, //实训考核

}

public enum PageUIType
{
    None = -1,

    //ClassLearnersNumber_Page,
    //KnowledgePointTaskStatistics_Page,
    //DetailedPracticalTrainingTasks_Page,
    TeacherCourseStatistics_Page,
    TSKnowledgePointTaskStatistics_Page,
    TSsxcztj_Page,
    TSDetailedPracticalTrainingTasks_Page,
    StudentCourseStatistics_Page,
    SSKnowledgePointTaskStatistics_Page,
    SSsxcztj_Page,
    SSDetailedPracticalTrainingTasks_Page,
    TexturePage,
    TextPage,
    PDFPage,
    VideoPage
}


public enum PanelUIType
{
    None = -1,
    AdminTeacherManagement_Panel, //管理员管理教师信息
    AdminStudentManagement_Panel, //管理员管理学生信息
    TeacherCenter_Panel, //教师个人中心
    TeacherClassTj_Panel, //教师班级统计
    TeacherStudentTj_Panel, //教师学生统计
    StudentCenter_Panel, //学生端个人中心
    StudentZHTj_Panel, //学生端综合统计
    SafetyProtectionPanel, //安全防护面板
    CheckEntrenchPanel,      //物品查看面板
    AIDigitalHuman_Panel,
    AIAgent_Panel,                //AI智能体面板
    TaskPosting_Panel,            //任务发布面板     
    ExamConfiguration_Panel,      //考核配置面板
}

public enum SelectType
{
    A,
    B,
    C,
    D
}

public enum Trueorfalse
{
    正确,
    错误
}

/// <summary>
/// 弹窗类型
/// </summary>
public enum EnumMessageBoxType
{
    None = -1,
    OK,
    OKCancel,
}


public enum SelectDays
{
    Week,
    Month,
    Custom,
    None
}

/// <summary>
/// 数字人开启状态
/// </summary>
public enum DigitalHumanOnState
{
    None = -1,
    未启用,
    已启用
}
//Y -470
public class Defines
{
    #region 只读变量

    public const string UIPREFAB = "UIPrefab/"; //界面预设
    public const string SPRITEPREFAB = "Sprites/"; //图片精灵预设

    public const string MODELICON = "ModelIcon/"; //认知模型icon
    public const string MODELTEXTURE = "ModelTexture/"; //认知模型Texture

    public const string AUDIOVOLUME = "DQDAudioVolume";//本地化音量大小缓存（电驱动总成）
    public const string RESOLUTION = "DQDPCResolution"; //本地化分辨率缓存（电驱动总成）
    public const string FULLSCREEN = "DQDFullScreen";  //本地化是否全屏（电驱动总成）

    public const SoftInfo softInfo = SoftInfo.气压传动虚拟仿真实训系统;

    #endregion

    public static bool IsStart = true;
    //全局变量
    public static bool isOpenDigiteHuman = false;

    /// <summary>
    /// 缩略图
    /// </summary>
    public const string THUMBNAILS = "Thumbnails/";
    /// <summary>
    /// 原始图像
    /// </summary>
    public const string ORIGINALIMAGES = "OriginalImages/";
    public static TaskType taskType = TaskType.练习模式;
}

public class ResponseData
{
    public bool success { get; set; }
    public string message { get; set; }
    public string name { get; set; }
    public string path { get; set; }
}

public class PathData
{
    public int ID { get; set; }
    public string Des { get; set; }
    public string Path { get; set; }
}

public enum SoftInfo
{
    气压传动虚拟仿真实训系统 = 2600
}

/// <summary>
/// 认知学习科目
/// </summary>
public enum CLSType
{
    None = -1,
    元器件认知 = 101,

}

/// <summary>
/// 实训操作科目
/// </summary>
public enum POTSType
{
    None = -1,
    自动往返气路训练 = 201,
    快慢速自动切换气路训练,
    串联二段速节流控制调速气路训练,
    串并联三段速节流控制调速气路训练,
    压印机自动延时气路,
    单作用缸快速缩回气路
}

/// <summary>
/// 考核科目
/// </summary>
[System.Serializable]
public enum EVSType
{
    None = -1,
    自动往返气路训练 = 301,
    快慢速自动切换气路训练,
    串联二段速节流控制调速气路训练,
    串并联三段速节流控制调速气路训练,
    压印机自动延时气路,
    单作用缸快速缩回气路
}

/// <summary>
/// 场景模式
/// </summary>
[System.Serializable]
public enum Gamemode
{
    None = -1,
    RenZhi = 1,
    ShiXun,
    KaoHe,
    ZhuChangJing
}
public enum CourseType
{
    None,
    认知,
    实训,
    考核
}
public class TrainTaskMessage
{
    /// <summary>
    /// 任务模式
    /// </summary>
    public Gamemode gamemode;

    /// <summary>
    /// 任务类型
    /// </summary>
    public int taskType;

    /// <summary>
    /// 任务ID
    /// </summary>
    public int taskID;
}
/// <summary>
/// 装备穿戴类型
/// </summary>
public enum AppareType
{
    /// <summary>
    /// 空
    /// </summary>
    None,
    /// <summary>
    /// 头部
    /// </summary>
    Head,
    /// <summary>
    /// 手部
    /// </summary>
    Hand,
    /// <summary>
    /// 眼部
    /// </summary>
    Eye,
    /// <summary>
    /// 手腕
    /// </summary>
    Wrist,
    /// <summary>
    /// 身体
    /// </summary>
    Body,
    /// <summary>
    /// 脚部
    /// </summary>
    Feet
}
public enum CarKeySignTyoe
{
    None,
    /// <summary>
    /// 解锁
    /// </summary>
    Unlock,
    /// <summary>
    /// 锁车
    /// </summary>
    Lock,
    /// <summary>
    /// 找车
    /// </summary>
    FindCar,
    /// <summary>
    /// 解锁后备箱
    /// </summary>
    TrunkRekease,
    /// <summary>
    /// 机械钥匙
    /// </summary>
    MechanicalKey
}

public enum CarGearType
{
    None,
    /// <summary>
    /// 空挡
    /// </summary>
    N,
    /// <summary>
    /// 前进档
    /// </summary>
    D,
    /// <summary>
    /// 驻车档
    /// </summary>
    P,
    /// <summary>
    /// 倒车档
    /// </summary>
    R,
    /// <summary>
    /// 手动挡
    /// </summary>
    M
}
public enum ResourcesType
{
    None,
    PDF,
    Manual,
    Video,
    Texture,
    Text
}
public enum FileType
{
    ZIP,
    RAR,
    MP4,
    JPG,
    JPEG,
    PNG,
    TXT,
    PDF,
    DOC,
    XLS,
    WAV,
    MP3,
    OGG,
    None
}
public class FileDataInfo
{
    public FileType FileType { get; set; }
    public string FileName { get; set; }
    public string FileSize { get; set; }
    public string FilePath { get; set; }
}
public class WorkData
{
    public int FileType { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
}
public class TextureAssetsData
{
    public int FileType { get; set; }
    public string Name { get; set; }
    /// <summary>
    /// 缩列图路径
    /// </summary>
    public string ThumbnailsPath { get; set; }
    /// <summary>
    /// 原始图像路径
    /// </summary>
    public string OriginalImagesPath { get; set; }
}

public class TextureResource
{
    public int ID { get; set; }
    public Texture Thumbnails { get; set; }
    public Texture OriginalImages { get; set; }
}

public class ModelsInfoData
{
    public int ID { get; set; }
    public int Type { get; set; }
    public string Name { get; set; }
    public string ModelName { get; set; }
    public string IconName { get; set; }
    public string TextureName { get; set; }
    public string Intro { get; set; }
    public string Principle { get; set; }
    public int IsAnimation { get; set; }
}


/// <summary>
/// 教师任务发布数据
/// </summary>
public class TaskPostingData
{
    /// <summary>
    /// 发布哪款软件的任务
    /// </summary>
    public int SoftID { get; set; }
    public string Schoolname { get; set; }
    /// <summary>
    /// 发布者账号
    /// </summary>
    public string FBAccount { get; set; }
    /// <summary>
    /// 发布者姓名
    /// </summary>
    public string FBUsername { get; set; }
    /// <summary>
    /// 发布任务名称
    /// </summary>
    public string TaskName { get; set; }
    /// <summary>
    /// 发布任务类型
    /// </summary>
    public int TaskType { get; set; }
    /// <summary>
    /// 任务信息列表
    /// </summary>
    public string TaskInfos { get; set; }
    /// <summary>
    /// 任务分配用户列表
    /// </summary>
    public string TaskUsers { get; set; }
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatTime { get; set; }
}
public class TaskInfoData
{
    public int CourseID { get; set; }
    public string CourseName { get; set; }

}
public class TaskByUsers
{
    /// <summary>
    /// 账号
    /// </summary>
    public string Account { get; set; }
    /// <summary>
    /// 姓名
    /// </summary>
    public string Username { get; set; }
}
public enum TaskType
{
    练习模式,
    考核模式
}
public enum AgentType
{
    None = -1,
    技术支持,
    私域运营,
    客服销售,
    获客引流,
    品牌营销,
    项目设计,
    教研工具,
    课件内容生成,
    模型3D生成,
    辅助学习工具
}
