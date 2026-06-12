using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StudentZHTJItem : MonoBehaviour
{
    private TMP_Text[] _texts;
    private Button _infoBtn;
    private SoftwareLearningData _data;
    private StudentZHTj_Panel _studentZHTJPanel
    {
        get
        {
            return StudentTjPanel.GetPanel((int)PanelUIType.StudentZHTj_Panel) as StudentZHTj_Panel;
        }
    }
    private TeacherStudentTj_Panel _teacherStudentTj_Panel
    {
        get
        {
            return TeacherTjPanel.GetPanel((int)PanelUIType.TeacherStudentTj_Panel) as TeacherStudentTj_Panel;
        }
    }
    private void Awake()
    {
        _texts = transform.GetComponentsInChildren<TMP_Text>();
        _infoBtn = transform.GetComponentInChildren<Button>();
    }
    private void Start()
    {
        _infoBtn.onClick.AddListener(() => {
            if (AppController.Instance.userType == UserType.学生)
            {
                _studentZHTJPanel.ShowDetailWindow(_data);
            }
            else
            {
                _teacherStudentTj_Panel.ShowDetailWindow(_data);
            }
        });
    }
    public void OnInit(int index, SoftwareLearningData data)
    {
        _data = data;
        _texts[0].text = index.ToString();
        _texts[1].text = data.CourseName + GetCourseType(data.CourseType);
        _texts[2].text = data.LearnCount.ToString();
        TimeSpan timeSpan = TimeSpan.FromSeconds((float)data.LearnTime);
        string formattedTime = $"{(int)timeSpan.TotalHours}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        _texts[3].text = formattedTime;
        _texts[4].text = GetCourseTypeByScore(data.CourseType, data.Score);
        _texts[5].text = data.Time.ToString();
    }
    private string GetCourseType(int coursetype)
    {
        return coursetype switch
        {
            int type when type == 1 => " (认知)",
            int type when type == 2 => " (实训)",
            _ => " (考核)"
        };
    }
    private string GetCourseTypeByScore(int coursetype, string score)
    {
        return coursetype switch
        {
            int type when type == 1 => "无",
            //int type when type == 2 => "无",
            _ => score + "分"
        };
    }
}