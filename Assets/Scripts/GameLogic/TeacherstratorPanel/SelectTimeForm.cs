using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SelectTimeForm : MonoBehaviour
{
    private TMP_Text _startTimeText, _endTimeText;
    private Button _startTimeBtn, _endTimeBtn;
    private Button _cancelBtn, _okBtn;
    public GameObject _pickerStart, _pickerEnd;
    public DateTime _StartTime, _EndTime;
    private StudentTjPanel _studentTjPanel
    {
        get { return UIManager.Instance.GetUI<StudentTjPanel>(EnumUIType.StudentTjPanel); }
    }
    private TeacherTjPanel _teacherTjPanel
    {
        get { return UIManager.Instance.GetUI<TeacherTjPanel>(EnumUIType.TeacherTjPanel); }
    }
    private void Awake()
    {
        _startTimeText = transform.Find("starttime/Button/Text (TMP)").GetComponent<TMP_Text>();
        _endTimeText = transform.Find("endtime/Button/Text (TMP)").GetComponent<TMP_Text>();
        _startTimeBtn = transform.Find("starttime/Button").GetComponent<Button>();
        _endTimeBtn = transform.Find("endtime/Button").GetComponent<Button>();
        _cancelBtn = transform.Find("btns/cancelButton").GetComponent<Button>();
        _okBtn = transform.Find("btns/okButton").GetComponent<Button>();
    }
    private void Start()
    {
        _startTimeBtn.onClick.AddListener(() =>
        {
            if (!_pickerStart.activeSelf)
            {
                _pickerStart.SetActive(true);
            }
            if (_pickerEnd.activeSelf)
            {
                _pickerEnd.SetActive(false);
            }
        });
        _endTimeBtn.onClick.AddListener(() =>
        {
            if (!_pickerEnd.activeSelf)
            {
                _pickerEnd.SetActive(true);
            }
            if (_pickerStart.activeSelf)
            {
                _pickerStart.SetActive(false);
            }
        });
        _cancelBtn.onClick.AddListener(() =>
        {
            Hide();
        });
        _okBtn.onClick.AddListener(() =>
        {
            if (_startTimeText.text == "0000-00-00(00:00:00)" || _endTimeText.text == "0000-00-00(00:00:00)")
            {
                UIManager.Instance.OpenMessageBoxUI("错误提示", "请选择开始时间或结束时间！", 0, EnumMessageBoxType.OK);
                return;
            }
            if (_StartTime > _EndTime)
            {
                UIManager.Instance.OpenMessageBoxUI("错误提示", "开始时间不能晚于结束时间!", 0, EnumMessageBoxType.OK);
                return;
            }
            if (AppController.Instance.userType == UserType.学生)
            {
                StudentZHTj_Panel panel = StudentTjPanel.GetPanel((int)PanelUIType.StudentZHTj_Panel) as StudentZHTj_Panel;
                panel.SetQueryTime(_StartTime, _EndTime);
            }
            else
            {
                if (TeacherTjPanel.currentPanel.name == "TeacherClassTj_Panel")
                {
                    TeacherClassTj_Panel panel = TeacherTjPanel.GetPanel((int)PanelUIType.TeacherClassTj_Panel) as TeacherClassTj_Panel;
                    panel.SetQueryTime(_StartTime, _EndTime);
                }
                else if (TeacherTjPanel.currentPanel.name == "TeacherStudentTj_Panel")
                {
                    TeacherStudentTj_Panel panel = TeacherTjPanel.GetPanel((int)PanelUIType.TeacherStudentTj_Panel) as TeacherStudentTj_Panel;
                    panel.SetQueryTime(_StartTime, _EndTime);
                }
            }

            Hide();
        });
    }
    public void Show()
    {
        this.gameObject.SetActive(true);
    }
    public void Hide()
    {
        if (_pickerStart.activeSelf)
        {
            _pickerStart.SetActive(false);
        }
        if (_pickerEnd.activeSelf)
        {
            _pickerEnd.SetActive(false);
        }
        this.gameObject.SetActive(false);
    }
}
