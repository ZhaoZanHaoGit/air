using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class TaskSelectItem : MonoBehaviour
{
    public SelectTask  selectTask;
    public int CourseID { get; set; }
    public string TaskName { get; set; }
    private TMP_Text nameText;
    private Toggle selectToggle;
    // 新增：是否忽略 Toggle 变化事件
    private bool suppressToggleCallback = false;
    private void Awake()
    {
        nameText = GetComponentInChildren<TMP_Text>();
        selectToggle=transform.Find("Toggle").GetComponent<Toggle>();
    }
    private void Start()
    {
        TaskName = nameText.text;
        ChangedData(Defines.taskType);
        selectToggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    public void OnToggleValueChanged(bool isOn)
    {
        if (suppressToggleCallback)
            return; // 跳过回调
        if (isOn)
        {
            selectTask.Add(this);
        }
        else {
            selectTask.Remove(this);
        }
    }
    public void SetSelectState(bool isOn, bool suppressCallback = false)
    {
        suppressToggleCallback = suppressCallback;
        selectToggle.isOn = isOn;
        suppressToggleCallback = false;
    }

    public void ChangedData(TaskType type)
    {
        CourseID = type == TaskType.练习模式 ? (int)((POTSType)Enum.Parse(typeof(POTSType), TaskName)) : (int)((EVSType)Enum.Parse(typeof(EVSType), TaskName));
    }
    //public void SetSelectState(bool isOn)
    //{
    //    selectToggle.isOn = isOn;
    //}
}
