using System.Collections.Generic;

public class AppController : DDOLSingleton<AppController>
{
    public UserType userType = UserType.学生;

    public List<PathData> pathDatas = new List<PathData>();

    //当前登录学生的所有学习数据
    public List<SoftwareLearningData> currentUserLearnDatas = new List<SoftwareLearningData>();
    public List<SoftwareLearningData> classesLearnDatas = new List<SoftwareLearningData>();
    public List<User> currentStudents = new List<User>();
    public List<SoftResourcesData> softResourcesDatas = new List<SoftResourcesData>();

    public TrainTaskMessage CurrentTaskMessage = new TrainTaskMessage();

    //当前登录教师的所有数字人数据
    public List<DigitalHumanData> currentDigitalHumanDatas = new List<DigitalHumanData>();
    public DigitalHumanData curStudentDigitalHumanDatas = null;
    public List<ModelsInfoData> ModelsDatas = new List<ModelsInfoData>();

    //当前教师发布的所有任务
    public List<TaskPostingData> currentTeacherTasksDatas = new List<TaskPostingData>();
    //当前学生的所有任务
    public List<TaskPostingData> currentStudentTasksDatas = new List<TaskPostingData>();

    //学校所有智能体
    public List<AgentData> allAgentDatas = new List<AgentData>();

    /// <summary>
    /// 当前登录用户
    /// </summary>
    public User loginUser;

    /// <summary>
    /// 所有当前学校的用户
    /// </summary>
    public List<User> users = new List<User>();

    /// <summary>
    /// 当前登录软件信息
    /// </summary>
    public SoftData softData = null;

    public ExamSettingDatas examSettingDatas = null;

    public CLSType cLSType = CLSType.None;
    public POTSType pOTSType = POTSType.None;
    public EVSType eVSType = EVSType.None;
    public CourseType courseType = CourseType.None;
    public MenuPanelType menuPanelType = MenuPanelType.None;
    public override void Init()
    {
    }

    private new void OnApplicationQuit()
    {
        _ = NetHelper.Instance.OnSignOutClick();
    }

    public void OnReset()
    {
        loginUser = null;
        softData = null;
        userType = UserType.学生;
        if (users != null)
        {
            users.Clear();
        }

        if (currentUserLearnDatas != null)
        {
            currentUserLearnDatas.Clear();
        }

        if (classesLearnDatas != null)
        {
            classesLearnDatas.Clear();
        }

        if (currentStudents != null)
        {
            currentStudents.Clear();
        }

        if (curStudentDigitalHumanDatas != null)
        {
            curStudentDigitalHumanDatas = null;
        }

        if (softResourcesDatas != null)
        {
            softResourcesDatas.Clear();
        }
        if (currentTeacherTasksDatas != null)
        {
            currentTeacherTasksDatas.Clear();
        }
        if (currentStudentTasksDatas != null)
        {
            currentStudentTasksDatas.Clear();
        }
        if (allAgentDatas != null)
        {
            allAgentDatas.Clear();
        }
        Defines.isOpenDigiteHuman = false;
    }
}