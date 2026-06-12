using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectTask : MonoBehaviour
{
    public AddTaskPanel panel;
    private Toggle allSelectToggle;
    private Button cancelBtn;
    private Button addTasksBtn;
    private List<TaskSelectItem> selecttaskSelectItems = new List<TaskSelectItem>();
    public TaskSelectItem[] taskSelectItems;
    private void Awake()
    {
        allSelectToggle=transform.Find("AllToggle").GetComponent<Toggle>();
        cancelBtn = transform.Find("cancel").GetComponent<Button>();
        addTasksBtn = transform.Find("add").GetComponent<Button>();
    }
    private void Start()
    {
        allSelectToggle.onValueChanged.AddListener(OnAllToggleValueChanged);
        cancelBtn.onClick.AddListener(OnCancelBtnClick);
        addTasksBtn.onClick.AddListener(OnAddTasksBtnClick);
    }

    private void OnAddTasksBtnClick()
    {
        if (selecttaskSelectItems.Count <= 0)
        {
            UIManager.Instance.OpenMessageBoxUI("提示信息", "请选择要添加的任务！", 0, EnumMessageBoxType.OK);
            return;
        }  
        panel.AddTaskModelsList(selecttaskSelectItems);
        Hide();
    }

    private void OnCancelBtnClick()
    {
        Hide();
    }
    
    private void OnAllToggleValueChanged(bool isOn)
    {
        if (isOn) {
            foreach (var item in taskSelectItems)
            {
                item.SetSelectState(true);
            }
        } else {
            foreach (var item in taskSelectItems)
            {
                item.SetSelectState(false);
            }
        }
    }
    public void Add(TaskSelectItem item)
    {
     
        if (selecttaskSelectItems.Find(a => a.CourseID == item.CourseID)) return;
        selecttaskSelectItems.Add(item);
        //foreach (var A in selecttaskSelectItems)
        //{
        //    Debug.Log(A.CourseID);
        //}
    }
    public void Remove(TaskSelectItem item)
    {
        selecttaskSelectItems.Remove(item);
        //foreach (var A in selecttaskSelectItems)
        //{
        //    Debug.Log(A.CourseID);
        //}
    }
    public void Show() {
        this.gameObject.SetActive(true);
    }
    public void  Hide() {
        if (selecttaskSelectItems.Count > 0)
        {
            foreach (var item in selecttaskSelectItems)//报错
            {
                item.SetSelectState(false, suppressCallback: true); // 不触发 Remove
            }
        }
        allSelectToggle.isOn = false;
        selecttaskSelectItems.Clear();
        this.gameObject.SetActive(false);
    }
}
