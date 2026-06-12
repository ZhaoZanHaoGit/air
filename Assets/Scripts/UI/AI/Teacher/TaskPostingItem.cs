using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskPostingItem : MonoBehaviour
{
    public TaskPostingData Data { get; set; }
    private Toggle selectToggle;
    private TMP_Text numberText;
    private TMP_Text taskNameText;
    private TMP_Text taskType;
    private Button seeTaskInfoBtn;
    private Button seeUsersInfoBtn;
    private TMP_Text creatTimeText;
    private TaskPosting_Panel panel;
    private bool suppressToggleCallback = false;
    private void Awake()
    {
        selectToggle = transform.Find("Toggle").GetComponent<Toggle>();
        numberText = transform.Find("number").GetComponent<TMP_Text>();
        taskNameText = transform.Find("taskName").GetComponent<TMP_Text>();
        taskType = transform.Find("taskType").GetComponent<TMP_Text>();
        seeTaskInfoBtn = transform.Find("taskInfo/Button").GetComponent<Button>();
        seeUsersInfoBtn = transform.Find("userInfo/Button").GetComponent<Button>();
        creatTimeText = transform.Find("creatime").GetComponent<TMP_Text>();
    }
    private void Start()
    {
        seeTaskInfoBtn.onClick.AddListener(OnSeeTaskInfoBtnClick);
        seeUsersInfoBtn.onClick.AddListener(OnSeeUsersInfoBtnClick);
        selectToggle.onValueChanged.AddListener(OnSelectToggleValueChanged);
    }

    private void OnSelectToggleValueChanged(bool isOn)
    {
        if (suppressToggleCallback)
            return; // 跳过回调
        if (isOn)
        {
            panel.Add(this);
        }
        else
        {
            panel.Remove(this);
        }
    }

    private void OnSeeUsersInfoBtnClick()
    {
        List<UserModelData> datas = JsonConvert.DeserializeObject<List<UserModelData>>(Data.TaskUsers);
        if (datas != null && datas.Count > 0)
        {
            panel.ShowUserInfo("学员详情", Data.TaskName, datas);
        }
    }

    private void OnSeeTaskInfoBtnClick()
    {
        List<TaskModelData> datas = JsonConvert.DeserializeObject<List<TaskModelData>>(Data.TaskInfos);
        if (datas != null && datas.Count > 0)
        {
            panel.ShowTaskInfo("任务详情", Data.TaskName, datas);
        }
    }
    public void InitData(TaskPostingData _data, int _number, TaskPosting_Panel _panel)
    {
        panel = _panel;
        Data = _data;
        numberText.text = _number.ToString();
        taskNameText.text = Data.TaskName;
        taskType.text = ((TaskType)Data.TaskType).ToString();
        creatTimeText.text = Data.CreatTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
    public void SetSelectState(bool isOn, bool suppressCallback = false)
    {
        suppressToggleCallback = suppressCallback;
        selectToggle.isOn = isOn;
        suppressToggleCallback = false;
    }
}
