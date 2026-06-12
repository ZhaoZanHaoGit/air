using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskModelItem : MonoBehaviour
{
    public int CourseID { get; set; }
    public string TaskName { get; set; }
    private TMP_Text nameText;
    private Button deleteBtn;
    private AddTaskPanel _panl;
    private void Awake()
    {
        nameText=transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        deleteBtn = transform.Find("Button").GetComponent<Button>();
    }
    private void Start()
    {
        deleteBtn.onClick.AddListener(OnDeleteBtnClick);
    }
    public void InitData(AddTaskPanel panl,int courseid,int number, string name) {
        _panl = panl;
        CourseID = courseid;
        TaskName = name;
        nameText.text = number+"."+name;
    }
    //public void ChangedData(TaskType type)
    //{
    //    CourseID = type == TaskType.Á·Ï°Ä£Ê½ ? (int)((POTSType)Enum.Parse(typeof(POTSType), TaskName)) : (int)((EVSType)Enum.Parse(typeof(EVSType), TaskName));
    //    Debug.Log(CourseID);
    //}
    private void OnDeleteBtnClick()
    {
        _panl.DeleteTaskModel(this);
    }
}
