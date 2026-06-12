using Cysharp.Threading.Tasks;
using Net.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


public class NetHelper : DDOLSingleton<NetHelper>
{
    public override void Init()
    {

    }
    #region 请求
    #region 平台
    public async UniTaskVoid OnSignOutClick()
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.SiginOut);
    }
    public async UniTaskVoid Login(string account, string password, Action CallbackSuccesslogin, Action<string> callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.Login, account, password, (int)AppController.Instance.userType);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        switch (code)
        {
            case 0:
                AppController.Instance.loginUser = task.model.As<User>();
                AppController.Instance.userType = (UserType)AppController.Instance.loginUser.Usertype;
                CallbackSuccesslogin?.Invoke();
                break;
            case -1:
                callback?.Invoke("账号或密码错误!");
                break;
        }
    }
    public async UniTaskVoid AddUsers(List<User> users, Action Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.AddUsers, users);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        switch (code)
        {
            case 0:
                UIManager.Instance.OpenMessageBoxUI("提示", "上传用户成功！", 3, EnumMessageBoxType.OK);
                Callback?.Invoke();
                break;
            case -1:
                UIManager.Instance.OpenMessageBoxUI("错误信息", "上传用户时发生错误！", 0, EnumMessageBoxType.OK);
                break;
            case -2:
                UIManager.Instance.OpenMessageBoxUI("错误信息", "账号已存在！", 0, EnumMessageBoxType.OK);
                break;
        }
    }
    public async UniTaskVoid GetUsers(string schoolname, Action CallBack)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.GetUsers, schoolname);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        switch (code)
        {
            case 0:
                List<User> users = task.model.As<List<User>>();
                AppController.Instance.users = users;
                CallBack?.Invoke();
                break;
            case -2:
                UIManager.Instance.OpenMessageBoxUI("错误信息", "获取数据失败!", 0, EnumMessageBoxType.OK);
                break;
            case -1:
                //UIManager.Instance.OpenMessageBoxUI("错误信息", "暂无用户数据!", 0, EnumMessageBoxType.OK);
                CallBack?.Invoke();
                break;
        }
    }
    public async UniTaskVoid DeleteUsers(List<User> users, Action Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.DeleteUsers, users);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        switch (code)
        {
            case 0:
                UIManager.Instance.OpenMessageBoxUI("提示", "用户删除成功！", 3, EnumMessageBoxType.OK);
                Callback?.Invoke();
                break;
            case -1:
                UIManager.Instance.OpenMessageBoxUI("提示", "用户删除失败！", 3, EnumMessageBoxType.OK);
                break;
        }
    }
    public async UniTaskVoid UpdateUser(User user, Action Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.UpdateUser, user);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        switch (code)
        {
            case 0:
                UIManager.Instance.OpenMessageBoxUI("提示", "用户更新成功！", 3, EnumMessageBoxType.OK);
                Callback?.Invoke();
                break;
            case -1:
                UIManager.Instance.OpenMessageBoxUI("提示", "用户更新失败！", 3, EnumMessageBoxType.OK);
                break;
        }
    }
    public async UniTaskVoid ForgotPassword(string account, string email, string newPassword, Action<int> Callback)
    {
        //print(account);
        var task = await ClientBase.Instance.Call((ushort)ProtoType.ForgotPassword, account, email, newPassword);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        Callback?.Invoke(code);
    }
    public async UniTaskVoid DeletePresetCourseDatas(List<PresetCourseData> datas, Action Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.DeletePresetCourseDatas, datas);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        switch (code)
        {
            case 0:
                UIManager.Instance.OpenMessageBoxUI("提示信息", "删除课程成功!", 0, EnumMessageBoxType.OK);
                Callback?.Invoke();
                break;
            case -1:
                UIManager.Instance.OpenMessageBoxUI("提示信息", "删除课程时发生错误！", 3, EnumMessageBoxType.OK);
                Callback?.Invoke();
                break;
        }
    }
    public async UniTaskVoid DeleteCurrentAllExamPapers(List<User> users, Action Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.DeleteCurrentAllExamPapers, users);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        switch (code)
        {
            case 0:
                Callback?.Invoke();
                break;
            case -1:
                UIManager.Instance.OpenMessageBoxUI("错误信息", "删除试卷失败!", 0, EnumMessageBoxType.OK);
                break;
        }
    }
    public async UniTaskVoid DeleteCurrentAllQuestions(List<User> users, Action Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.DeleteCurrentAllQuestions, users);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        switch (code)
        {
            case 0:
                Callback?.Invoke();
                break;
            case -1:
                UIManager.Instance.OpenMessageBoxUI("错误信息", "删除试题失败!", 0, EnumMessageBoxType.OK);
                break;
        }
    }
    public async UniTaskVoid DeleteExamResults(List<ExamResult> datas, Action Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.DeleteExamResults, datas);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        switch (code)
        {
            case 0:
                Callback?.Invoke();
                break;
            case -1:
                UIManager.Instance.OpenMessageBoxUI("错误信息", "删除学生成绩时发生错误!", 0, EnumMessageBoxType.OK);
                break;
        }
    }
    public async UniTask DeleteStudentsLearnDatas(List<User> datas, Action<int> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.DeleteStudentsLearnDatas, datas);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        Callback?.Invoke(code);
    }
    public async UniTask DeleteTaskDataByUsers(List<User> datas, Action<int> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.DeleteTaskDataByUsers, datas);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        Callback?.Invoke(code);
    }
    #endregion
    #region 新能源汽修
    public async UniTaskVoid AddSoftData(SoftData data, Action<SoftData> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.AddSoftData, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        SoftData _data = task.model.As<SoftData>();
        Callback?.Invoke(_data);
    }
    public async UniTaskVoid QuitSoft(SoftData data, Action Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.QuitSoft, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        Callback?.Invoke();
    }
    public async UniTaskVoid GetSoftStateOnline(SoftData data, Action<int> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.GetSoftStateOnline, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var count = task.model.AsInt;
        Callback?.Invoke(count);
    }
    public async UniTaskVoid AddSoftLearningData(SoftwareLearningData data, Action<int> Callback)
    {

        var task = await ClientBase.Instance.Call((ushort)ProtoType.AddSoftLearningData, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        Callback?.Invoke(code);
    }
    public async UniTaskVoid AddSoftLearningData(string data, Action<int> Callback)
    {

        var task = await ClientBase.Instance.Call((ushort)ProtoType.AddSoftLearningData, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        Callback?.Invoke(code);
    }
    /// <summary>
    /// 获取学生软件学习数据(数据量偏大)
    /// </summary>
    /// <param name="account"></param>
    /// <param name="softid"></param>
    /// <param name="Callback"></param>
    /// <returns></returns>
    public async UniTaskVoid GetSoftLearnDatasByAccount(string account, int softid, Action<List<SoftwareLearningData>> Callback)
    {

        var task = await ClientBase.Instance.Call((ushort)ProtoType.GetSoftLearnDatasByAccount, 120000, account, softid);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        if (task.model.count == 0 || task.model.buffer == null)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "数据为空!", 5, EnumMessageBoxType.OK);
            Callback?.Invoke(null);
            return;
        }
        byte[] rawData = task.model.Buffer;
        string fullStr = Encoding.UTF8.GetString(rawData);
        int jsonStart = fullStr.IndexOf("[{");
        if (jsonStart >= 0)
            fullStr = fullStr.Substring(jsonStart);

        List<SoftwareLearningData> datas = JsonConvert.DeserializeObject<List<SoftwareLearningData>>(fullStr);
        Callback?.Invoke(datas);
    }
    /// <summary>
    /// 获取班级学生软件学习数据(数据量偏大)
    /// </summary>
    /// <param name="data"></param>
    /// <param name="Callback"></param>
    /// <returns></returns>
    public async UniTaskVoid GetSoftLearnDatasByClasses(SoftData data, Action<List<SoftwareLearningData>> Callback)
    {

        var task = await ClientBase.Instance.Call((ushort)ProtoType.GetSoftLearnDatasByClasses, 120000, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        if (task.model.count == 0 || task.model.buffer == null)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "数据为空!", 5, EnumMessageBoxType.OK);
            Callback?.Invoke(null);
            return;
        }
        byte[] rawData = task.model.Buffer;
        string fullStr = Encoding.UTF8.GetString(rawData);
        int jsonStart = fullStr.IndexOf("[{");
        if (jsonStart >= 0)
            fullStr = fullStr.Substring(jsonStart);
        List<SoftwareLearningData> datas = JsonConvert.DeserializeObject<List<SoftwareLearningData>>(fullStr);
        Callback?.Invoke(datas);
    }
    public async UniTaskVoid GetUserByClasses(SoftData data, Action<List<User>> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.GetUserByClasses, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var datas = task.model.As<List<User>>();
        Callback?.Invoke(datas);
    }
    #endregion
    #region 数字人
    public async UniTask AddDigitalHumanData(DigitalHumanData data, Action<int> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.AddDigitalHumanData, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        Callback?.Invoke(code);
    }
    public async UniTask UpdateDigitalHumanData(DigitalHumanData data, Action<int> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.UpdateDigitalHumanData, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        Callback?.Invoke(code);
    }
    public async UniTask GetDigitalHumanData_Teacher(string data, Action<List<DigitalHumanData>> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.GetDigitalHumanData_Teacher, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var datas = task.model.As<List<DigitalHumanData>>();
        Callback?.Invoke(datas);
    }
    public async UniTask GetDigitalHumanData_Student(string data1, string data2, Action<DigitalHumanData> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.GetDigitalHumanData_Student, data1, data2);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var data = task.model.As<DigitalHumanData>();
        Callback?.Invoke(data);
    }
    public async UniTask DeleteDigitalHumanData(string data, Action<int> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.DeleteDigitalHumanData, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        Callback?.Invoke(code);
    }
    public async UniTask GetDigitalHumanonStateData(string data1, string data2, Action<int> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.GetDigitalHumanonStateData, data1, data2);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        Callback?.Invoke(code);
    }
    #endregion
    #region 公共协议
    public async UniTaskVoid AddSoftResourcesDatas(List<SoftResourcesData> datas, Action<int> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.AddSoftResourcesDatas, datas);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        Callback?.Invoke(code);
    }
    public async UniTaskVoid GetSoftResourcesDatas(SoftResourcesData data, Action<List<SoftResourcesData>> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.GetSoftResourcesDatas, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var datas = task.model.As<List<SoftResourcesData>>();
        Callback?.Invoke(datas);
    }
    public async UniTaskVoid DeleteResourcesDatas(List<int> ids, Action<int> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.DeleteResourcesDatas, ids);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        Callback?.Invoke(code);
    }
    public async UniTaskVoid AddSoftExamSettingData(SoftExamSettingDatas data, Action<int> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.AddSoftExamSettingData, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }

        var code = task.model.AsInt;
        Callback?.Invoke(code);
    }

    public async UniTaskVoid GetSoftExamSettingData(SoftExamSettingDatas data, Action<SoftExamSettingDatas> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.GetSoftExamSettingData, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }

        var datas = task.model.As<SoftExamSettingDatas>();
        Callback?.Invoke(datas);
    }
    public async UniTaskVoid AddTaskPostingData(TaskPostingData data, Action<int> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.AddTaskPostingData, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        Callback?.Invoke(code);
    }
    public async UniTaskVoid GetTeacherTasks(TaskPostingData data, Action<List<TaskPostingData>> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.GetTeacherTasks, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.As<List<TaskPostingData>>();
        Callback?.Invoke(code);
    }
    public async UniTaskVoid GetStudentTasks(string softid, string account, Action<List<TaskPostingData>> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.GetStudentTasks, softid, account);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.As<List<TaskPostingData>>();
        Callback?.Invoke(code);
    }
    public async UniTaskVoid DeleteTaskPostingData(List<TaskPostingData> datas, Action<int> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.DeleteTaskPostingData, datas);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        Callback?.Invoke(code);
    }
    #endregion
    #region AI智能体
    public async UniTaskVoid AddAgentData(AgentData data, Action<int> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.AddAgentData, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        Callback?.Invoke(code);
    }
    public async UniTaskVoid DeleteAgentData(List<int> datas, Action<int> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.DeleteAgentDatas, datas);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        Callback?.Invoke(code);
    }
    public async UniTaskVoid GetAgentDatasByTeacher(User data, Action<List<AgentData>> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.GetAgentDatasByTeacher, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var datas = task.model.As<List<AgentData>>();
        Callback?.Invoke(datas);
    }
    public async UniTaskVoid GetAgentDatasByStudent(User data, Action<List<AgentData>> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.GetAgentDatasByStudent, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var datas = task.model.As<List<AgentData>>();
        Callback?.Invoke(datas);
    }
    public async UniTaskVoid UpdateAgentData(AgentData data, Action<int> Callback)
    {
        var task = await ClientBase.Instance.Call((ushort)ProtoType.UpdateAgentData, data);
        if (!task.IsCompleted)
        {
            UIManager.Instance.OpenMessageBoxUI("错误信息", "请求超时!", 5, EnumMessageBoxType.OK);
            return;
        }
        var code = task.model.AsInt;
        Callback?.Invoke(code);
    }
    #endregion
    #endregion
}
