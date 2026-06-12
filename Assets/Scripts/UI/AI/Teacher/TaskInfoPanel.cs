using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskInfoPanel : BaseWindowPanel
{
    private Transform root;
    private GameObject prefab;
    private Button closeBtn;
    private TMP_Text taskNameText;
    private TMP_Text titleText;
    private void Awake()
    {
        root = transform.Find("bg/Scroll View/Viewport/Content");
        prefab = transform.Find("PrefabT/taskInfoModelItem").gameObject;
        closeBtn = transform.Find("bg/closeBtn").GetComponent<Button>();
        taskNameText = transform.Find("bg/title/taskname").GetComponent<TMP_Text>();
        titleText = transform.Find("bg/title/Image/title").GetComponent<TMP_Text>();
    }
    private void Start()
    {
        closeBtn.onClick.AddListener(ClosePanel);
    }

    private void ClosePanel()
    {
        Reset(root);
        taskNameText.text = "";
        OnHide();
    }
    public void ShowTaskInfo(string _title,string _name, List<TaskModelData> datas)
    {
        titleText.text = _title;
        taskNameText.text = "-->" + _name;
        CreateTaskInfoItem(datas);
    }
    public void ShowUserInfo(string _title, string _name, List<UserModelData> datas)
    {
        titleText.text = _title;
        taskNameText.text = "-->" + _name;
        CreateUserInfoItem(datas);
    }
    private void CreateTaskInfoItem(List<TaskModelData> datas)
    {
        int index = 0;
        foreach (var d in datas)
        {
            index++;
            GameObject item = Instantiate(prefab, root);
            TaskInfoModelItem taskInfoModelItem = item.GetComponent<TaskInfoModelItem>();
            taskInfoModelItem.SetData(index + "." + d.TaskName);
        }
    }
    private void CreateUserInfoItem(List<UserModelData> datas)
    {
        int index = 0;
        foreach (var d in datas)
        {
            index++;
            GameObject item = Instantiate(prefab, root);
            TaskInfoModelItem taskInfoModelItem = item.GetComponent<TaskInfoModelItem>();
            taskInfoModelItem.SetData(index + "." + d.ClassName+"   "+d.Username);
        }
    }
}
