using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskPosting_Panel : BasePanel
{
    public override PanelUIType GetUIType()
    {
        return PanelUIType.TaskPosting_Panel;
    }
    private Button  addTaskButton, deleteTaskButton;
    private Toggle allSelectToggle;
    private Transform taskItemRoot;
    private GameObject taskItemPrefab;
    private List<TaskPostingItem> selectTaskPostingItems = new List<TaskPostingItem>();
    private List<TaskPostingItem> allTaskPostingItems = new List<TaskPostingItem>();
    private int number = 0;
    public AddTaskPanel addTaskPanel;
    public TaskInfoPanel taskInfoPanel;
    public override void Awake()
    {
        base.Awake();
        addTaskButton=transform.Find("addTaskBtn").GetComponent<Button>();
      
        deleteTaskButton = transform.Find("deleteTaskBtn").GetComponent<Button>();
        allSelectToggle = transform.Find("scoreview/titlename/Toggle").GetComponent<Toggle>();
        taskItemRoot = transform.Find("scoreview/Scroll View/Viewport/Content");
        taskItemPrefab = transform.Find("PrefabT/TaskPostingItem").gameObject;
    }
    private void Start()
    {
        addTaskButton.onClick.AddListener(AddTaskClick);
      
        deleteTaskButton.onClick.AddListener(DeleteTaskClick);
        allSelectToggle.onValueChanged.AddListener(AllSelectClick);
    }
    public override void Init()
    {
        if (taskItemRoot.childCount>0) {
            return;
        }
        if (AppController.Instance.currentTeacherTasksDatas != null && AppController.Instance.currentTeacherTasksDatas.Count > 0)
        {
            foreach (var item in AppController.Instance.currentTeacherTasksDatas)
            {
                SpawnTaskItems(item);
            }
        }
        else
        {
            GetData();
        }
    }

    private void GetData()
    {
        OnRefresh();
    }
    public override void OnRefresh()
    {
        if (taskItemRoot.childCount > 0)
        {
            DeleteChildren(taskItemRoot);
            allTaskPostingItems.Clear();
            selectTaskPostingItems.Clear();
        }
        TaskPostingData data=new TaskPostingData
        {
            SoftID = (int)Defines.softInfo,
            FBAccount = AppController.Instance.loginUser.Account,
        };
        _ = NetHelper.Instance.GetTeacherTasks(data, (datas) =>
        {
            if (datas != null && datas.Count > 0)
            {
              
                AppController.Instance.currentTeacherTasksDatas = datas;
                foreach (var item in datas)
                {
                    SpawnTaskItems(item);
                }
            }
        });
    }
    public void Add(TaskPostingItem item)
    {
        if (selectTaskPostingItems.Contains(item)) return;
        selectTaskPostingItems.Add(item);
        UpdateButtonState();
    }
    public void Remove(TaskPostingItem item)
    {
        selectTaskPostingItems.Remove(item);
        UpdateButtonState();
    }
    private void AllSelectClick(bool isOn)
    {

        if (allTaskPostingItems.Count <= 0) return;

        // 创建副本防止遍历时被修改
        var snapshot = new List<TaskPostingItem>(allTaskPostingItems);
        foreach (var item in snapshot)
        {
            item.SetSelectState(isOn, suppressCallback: false); // 关键：抑制回调
        }
        UpdateButtonState();
    } 
    void UpdateButtonState() {
        //Debug.Log(selectTaskPostingItems.Count > 0);
        deleteTaskButton.interactable = selectTaskPostingItems.Count > 0;
    }
    private void DeleteTaskClick()
    {
        if (selectTaskPostingItems.Count<=0) {
            UIManager.Instance.OpenMessageBoxUI("提示信息", "请选择要删除的任务！", 0, EnumMessageBoxType.OK);
            return;
        }
        UIManager.Instance.OpenMessageBoxUI("提示信息", "确认删除所选任务？", 0, EnumMessageBoxType.OKCancel,OnSoureDeleteTask);
    }

    private void OnSoureDeleteTask(object args)
    {
        List<TaskPostingData> datas = new List<TaskPostingData>();
        for (int i = 0; i < selectTaskPostingItems.Count; i++)
        {
            datas.Add(selectTaskPostingItems[i].Data);
        }
        _ = NetHelper.Instance.DeleteTaskPostingData(datas, (code) =>
        {
            if (code == 0)
            {
                foreach (var item in selectTaskPostingItems)
                {
                    allTaskPostingItems.Remove(item);
                    Destroy(item.gameObject);
                }
                selectTaskPostingItems.Clear();
                deleteTaskButton.interactable = false;
            }
            else
            {
                UIManager.Instance.OpenMessageBoxUI("提示信息", "任务删除失败，请稍后重试！", 0, EnumMessageBoxType.OK);
            }
        });
        
    }

    private void AddTaskClick()
    {
        addTaskPanel.OnShow();
    }
    public void SpawnTaskItems(TaskPostingData data)
    {
        number = allTaskPostingItems.Count+1;
        GameObject go = Instantiate(taskItemPrefab, taskItemRoot);
        TaskPostingItem item = go.GetComponent<TaskPostingItem>();
        item.InitData(data, number,this);
        allTaskPostingItems.Add(item);
    }
    public void ShowTaskInfo(string _title, string _name, List<TaskModelData> datas) {
        taskInfoPanel.OnShow();
        taskInfoPanel.ShowTaskInfo(_title,_name, datas);
    }
    public void ShowUserInfo(string _title, string _name, List<UserModelData> datas)
    {
        taskInfoPanel.OnShow();
        taskInfoPanel.ShowUserInfo(_title, _name, datas);
    }
    public bool DuplicateNames(string taskname) {
       var name=  allTaskPostingItems.Find(a => a.Data.TaskName == taskname);
        if (name) {
            return true;
        } else { 
            return false;
        }
    }
}
