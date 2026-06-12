using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AddTaskPanel : BaseWindowPanel
{
    private TMP_InputField nameInput;
    private TMP_Dropdown typeDropDown;
    private Transform taskModelItemRoot;
    private Transform userModelItemRoot;
    private GameObject taskModelItem;
    private GameObject userModelItem;
    private Button cancelBtn, saveBtn;
    private Button addTaskModelBtn, addUserModelBtn;
    public SelectTask selectTask;
    public SelectUser selectUser;
    private List<UserModelItem> userModelsList = new List<UserModelItem>();
    private List<TaskModelItem> taskModelsList = new List<TaskModelItem>();
    private List<User> users = new List<User>();
    private List<TaskModelData> taskModelDatas = new List<TaskModelData>();
    public TaskPosting_Panel taskPosting_Panel;
    private void Awake()
    {
        nameInput=transform.Find("bg/infos/2/taskName/InputField (TMP)").GetComponent<TMP_InputField>();
        typeDropDown = transform.Find("bg/infos/3/taskType/Dropdown").GetComponent<TMP_Dropdown>();
        taskModelItemRoot = transform.Find("bg/infos/4/taskInfo/area/Scroll View/Viewport/Content");
        userModelItemRoot = transform.Find("bg/infos/5/addUser/area/Scroll View/Viewport/Content");
        taskModelItem = transform.Find("PrefabT/taskModelItem").gameObject;
        userModelItem = transform.Find("PrefabT/userModelItem").gameObject;
        cancelBtn = transform.Find("bg/infos/6/btns/cancelButton").GetComponent<Button>();
        saveBtn = transform.Find("bg/infos/6/btns/okButton").GetComponent<Button>();
        addTaskModelBtn = transform.Find("bg/infos/4/addButton").GetComponent<Button>();
        addUserModelBtn = transform.Find("bg/infos/5/addButton").GetComponent<Button>();
    }
    private void Start()
    {
        cancelBtn.onClick.AddListener(() =>
        {
            OnHide();
        });
        saveBtn.onClick.AddListener(() =>
        {
            Save();
        });
        addTaskModelBtn.onClick.AddListener(() =>
        {
            AddButtonTaskModelItem();
        });

        addUserModelBtn.onClick.AddListener(() =>
        {
            AddButtonUserModelItem();
        });
        typeDropDown.onValueChanged.AddListener((value) =>
        {
            Defines.taskType = (TaskType)(value);
            UpdateDataType();
        });
    }
    void UpdateDataType() {
        if (taskModelDatas.Count > 0)
        {
            foreach (var item in taskModelDatas)
            {
                item.ChangedData(Defines.taskType);
            }
        }
    }
    private void AddButtonUserModelItem()
    {
        selectUser.Show();
    }

    private void AddButtonTaskModelItem()
    {
        selectTask.Show();
    }
    public void AddTaskModelsList(List<TaskSelectItem> datas) {
        var existingNames = new HashSet<string>();
        //Debug.Log(taskModelDatas.Count);
        for (int j = 0; j < taskModelDatas.Count; j++)
        {
            //Debug.Log(taskModelDatas[j].TaskName);
            existingNames.Add(taskModelDatas[j].TaskName);
        }
        for (int i = 0; i < datas.Count; i++)
        {
            string name = datas[i].TaskName;
            //Debug.Log(name);
            if (!existingNames.Contains(name))
            {
                taskModelDatas.Add(new TaskModelData
                {
                    CourseID = datas[i].CourseID,
                    TaskName = name,
                    //TaskTypeValue = (int)Defines.taskType
                });
                existingNames.Add(name);
            }
        }
        UpdateDataType();
        AddTaskModels();
    }
    private void AddTaskModels() {
        Reset(taskModelItemRoot);
        for (int i = 0; i < taskModelDatas.Count; i++)
        {
            GameObject go = Instantiate(taskModelItem, taskModelItemRoot);
            TaskModelItem item = go.GetComponent<TaskModelItem>();
            item.InitData(this, taskModelDatas[i].CourseID,(i+1),taskModelDatas[i].TaskName);
            taskModelsList.Add(item);
        }
    }

    public void AddUserModelsList(List<UserSelectItem> datas)
    {
        var existingUserIds = new HashSet<string>();
        for (int j = 0; j < users.Count; j++)
        {
            existingUserIds.Add(users[j].Account);
        }
        for (int i = 0; i < datas.Count; i++)
        {
            string account = datas[i].user.Account;
            if (!existingUserIds.Contains(account))
            {
                users.Add(datas[i].user);
                existingUserIds.Add(account);
            }
        }
        AddUserModels();
    }
    private void AddUserModels()
    {
        Reset(userModelItemRoot);
        for (int i = 0; i < users.Count; i++)
        {
            GameObject go = Instantiate(userModelItem, userModelItemRoot);
            UserModelItem item = go.GetComponent<UserModelItem>();
            item.InitData(this, users[i]);
            userModelsList.Add(item);
        }
    }

    public void DeleteTaskModel(TaskModelItem item) {
        taskModelsList.Remove(item);
        taskModelDatas.RemoveAll(t => t.CourseID == item.CourseID);
        Destroy(item.gameObject);
    }
    public void DeleteUserModel(UserModelItem item) {
        userModelsList.Remove(item);
        users.RemoveAll(u => u.Account == item.user.Account);
        Destroy(item.gameObject);
    }

    private void Save()
    {
        if (string.IsNullOrEmpty(nameInput.text)) {
            UIManager.Instance.OpenMessageBoxUI("提示信息", "任务名称不能为空！", 0, EnumMessageBoxType.OK);
            return;
        }
        if (taskModelsList.Count <= 0)
        {
            UIManager.Instance.OpenMessageBoxUI("提示信息", "任务内容不能为空！", 0, EnumMessageBoxType.OK);
            return;
        }
        if (users.Count <= 0)
        {
            UIManager.Instance.OpenMessageBoxUI("提示信息", "学员不能为空！", 0, EnumMessageBoxType.OK);
            return;
        }
        List<UserModelData> usermodels=new List<UserModelData>();
        for (int i = 0; i < users.Count; i++)
        {
            usermodels.Add(new UserModelData
            {
                Account = users[i].Account,
                Username = users[i].Username,
                ClassName = users[i].Classinfo
            });
        }
        if (taskPosting_Panel.DuplicateNames(nameInput.text.Trim())) {
            UIManager.Instance.OpenMessageBoxUI("提示信息", "任务名称重复!", 0, EnumMessageBoxType.OK);
            return;
        }
        TaskPostingData data = new TaskPostingData
        {
            SoftID = (int)Defines.softInfo,
            Schoolname = AppController.Instance.loginUser.School_name,
            FBAccount = AppController.Instance.loginUser.Account,
            FBUsername = AppController.Instance.loginUser.Username,
            TaskName = nameInput.text.Trim(),
            TaskType = (int)Defines.taskType,
            TaskInfos = JsonConvert.SerializeObject(taskModelDatas),
            TaskUsers = JsonConvert.SerializeObject(usermodels),
            CreatTime = DateTime.Now
        };
        //Debug.Log(data.TaskInfos);
        //Debug.Log(data.TaskUsers);
        //向服务器发送保存请求
        _ = NetHelper.Instance.AddTaskPostingData(data, (code) =>
        {
            if (code == 0)
            {
                taskPosting_Panel.SpawnTaskItems(data);
                OnHide();
            }
            else
            {
                UIManager.Instance.OpenMessageBoxUI("提示信息", "任务保存失败，请稍后重试！", 0, EnumMessageBoxType.OK);
            }
        });
        
    }
    public override void OnShow()
    {
        base.OnShow();
    }
 
    public override void OnHide()
    {
        Reset(taskModelItemRoot);
        Reset(userModelItemRoot);
        nameInput.text = "";
        userModelsList.Clear();
        taskModelsList.Clear();
        taskModelDatas.Clear();
        users.Clear();
        base.OnHide();
    }
}
public class TaskModelData
{
    public int CourseID { get; set; }
    public string TaskName { get; set; }

    //public int TaskTypeValue { get; set; }
    public void ChangedData(TaskType type)
    {
        CourseID = type == TaskType.练习模式 ? (int)((POTSType)Enum.Parse(typeof(POTSType), TaskName)) : (int)((EVSType)Enum.Parse(typeof(EVSType), TaskName));
    }
}
public class UserModelData 
{
    public string Account { get; set; }
    public string Username { get; set; }
    public string ClassName { get; set; }
}

