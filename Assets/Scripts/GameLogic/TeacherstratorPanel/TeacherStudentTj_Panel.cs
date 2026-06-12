using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeacherStudentTj_Panel : BasePanel
{
    public override PanelUIType GetUIType()
    {
        return PanelUIType.TeacherStudentTj_Panel;
    }
    #region
    private Toggle _weekToggle, _monthToggle, _customToggle;
    private TMP_Dropdown _selectClassDrop, _studentsDrop;
    private TMP_Text _usernameText;
    private Button _queryBtn;
    private Toggle _kctjjlToggle, _zsdxxTjToggle, _sxcjtjToggle, _sxkhtjToggle;
    private BasePage[] _pages;
    private BasePage[] pages
    {
        get
        {
            _pages = GetComponentsInChildren<BasePage>();
            return _pages;
        }
    }
    private static Dictionary<int, BasePage> pagesDic = new Dictionary<int, BasePage>();
    public static BasePage currentPage = null;
    private BasePage oldT = null;
    [HideInInspector]
    public DateTime _StartTime, _EndTime;
    [HideInInspector]
    public string _currentSelectClass;
    [HideInInspector]
    public string _currentStudentAccount;
    private TeacherTjPanel _teacherTjPanel
    {
        get { return UIManager.Instance.GetUI<TeacherTjPanel>(EnumUIType.TeacherTjPanel); }
    }
    #endregion
    public override void Awake()
    {
        base.Awake();
        _weekToggle = transform.Find("teacherview/fixed/selectTime/toggles/Toggle_0").GetComponent<Toggle>();
        _monthToggle = transform.Find("teacherview/fixed/selectTime/toggles/Toggle_1").GetComponent<Toggle>();
        _customToggle = transform.Find("teacherview/fixed/selectTime/toggles/Toggle_2").GetComponent<Toggle>();

        _selectClassDrop = transform.Find("teacherview/fixed/selectTime/classDropdown").GetComponent<TMP_Dropdown>();
        _studentsDrop = transform.Find("teacherview/fixed/selectTime/userDropdown").GetComponent<TMP_Dropdown>();
        _usernameText = transform.Find("teacherview/fixed/selectTime/namevalue").GetComponent<TMP_Text>();
        _queryBtn = transform.Find("teacherview/fixed/selectTime/sourButton").GetComponent<Button>();

        _kctjjlToggle = transform.Find("teacherview/fixed/selectMenu/Toggle_0").GetComponent<Toggle>();
        _zsdxxTjToggle = transform.Find("teacherview/fixed/selectMenu/Toggle_1").GetComponent<Toggle>();
        _sxcjtjToggle = transform.Find("teacherview/fixed/selectMenu/Toggle_2").GetComponent<Toggle>();
        _sxkhtjToggle = transform.Find("teacherview/fixed/selectMenu/Toggle_3").GetComponent<Toggle>();
        if (pagesDic.Count > 0)
        {
            pagesDic.Clear();
        }
        for (int i = 0; i < pages.Length; i++)
        {
            pagesDic.Add((int)pages[i].GetUIType(), pages[i]);
        }
        currentPage = DictionaryExtension.TryGet(pagesDic, (int)PageUIType.TeacherCourseStatistics_Page);
        oldT = currentPage;
        TimeSubtraction(7, ref _StartTime, ref _EndTime);
    }
    private void Start()
    {
        _kctjjlToggle.onValueChanged.AddListener((isOn) =>
        {
            OnMenuToggleClick((int)PageUIType.TeacherCourseStatistics_Page);
        });
        _zsdxxTjToggle.onValueChanged.AddListener((isOn) =>
        {
            OnMenuToggleClick((int)PageUIType.TSKnowledgePointTaskStatistics_Page);
        });
        _sxcjtjToggle.onValueChanged.AddListener((isOn) =>
        {
            OnMenuToggleClick((int)PageUIType.TSsxcztj_Page);
        });
        _sxkhtjToggle.onValueChanged.AddListener((isOn) =>
        {
            OnMenuToggleClick((int)PageUIType.TSDetailedPracticalTrainingTasks_Page);
        });
        _weekToggle.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                TimeSubtraction(7, ref _StartTime, ref _EndTime);
            }
        });
        _monthToggle.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                TimeSubtraction(30, ref _StartTime, ref _EndTime);
            }
        });
        _customToggle.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                _teacherTjPanel.ShowSelectTimeWindow();
            }
            else
            {
                _teacherTjPanel.HideSelectTimeWindow();
            }
        });
        _selectClassDrop.onValueChanged.AddListener((value) =>
        {
            UpdateStudentDrop(_selectClassDrop.options[value].text);
            _currentSelectClass = _selectClassDrop.options[value].text;
        });
        _studentsDrop.onValueChanged.AddListener((value) =>
        {
            _usernameText.text = AppController.Instance.currentStudents.Find(a => a.Account == _studentsDrop.options[value].text).Username;
            _currentStudentAccount = _studentsDrop.options[value].text;
        });
        _queryBtn.onClick.AddListener(() =>
        {
            currentPage.Init();
        });
    }
    private void OnMenuToggleClick(int args)
    {
        currentPage = DictionaryExtension.TryGet(pagesDic, args);
        if (oldT == currentPage)
            return;
        if (oldT != null)
        {
            oldT.OnHide();
        }
        if (currentPage)
        {
            currentPage.OnShow();
            currentPage.Init();
        }
        oldT = currentPage;
    }
    public void TimeSubtraction(int day, ref DateTime startTime, ref DateTime endTime)
    {
        TimeSpan ts = new TimeSpan(day, 0, 0, 0);
        DateTime time = DateTime.Now;
        DateTime start = time.Subtract(ts);
        string _endTime = time.Year + "-" + time.Month.ToString("00") + "-" + time.Day.ToString("00") + " " + "23:59:59";
        string _startTime = start.Year + "-" + start.Month.ToString("00") + "-" + start.Day.ToString("00") + " " + "00:00:00";
        startTime = Convert.ToDateTime(_startTime);
        endTime = Convert.ToDateTime(_endTime);
    }
    public void SetQueryTime(DateTime starttime, DateTime endTime)
    {
        _StartTime = starttime;
        _EndTime = endTime;
    }
    public override void Init()
    {
        GetClassesDropValues();
        GetClassesStudents_Init();
    }

    public override void OnRefresh()
    {
        GetClassesDropValues();
        GetClassesStudents_Refresh();
    }
    private void GetClassesDropValues()
    {
        string classes = AppController.Instance.loginUser.Classinfo;
        if (!string.IsNullOrEmpty(classes))
        {
            string[] classArray = classes.Split('|');
            _selectClassDrop.ClearOptions();
            List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < classArray.Length; i++)
            {
                TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData
                {
                    text = classArray[i]
                };
                optionDatas.Add(data);
            }
            _selectClassDrop.AddOptions(optionDatas);
            _currentSelectClass = classArray[0];
        }
        else {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData
            {
                text = "暂无班级"
            };
            List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
            optionDatas.Add(data);
            _selectClassDrop.AddOptions(optionDatas);
        }
    }
    private void GetClassesStudents_Init()
    {
        if (_selectClassDrop.options[0].text == "暂无班级") return;
        if (AppController.Instance.currentStudents != null && AppController.Instance.currentStudents.Count > 0)
        {
            List<User> users = AppController.Instance.currentStudents.FindAll(a => a.Classinfo == _currentSelectClass);
            if (users != null && users.Count > 0)
            {
                _studentsDrop.ClearOptions();
                List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
                for (int i = 0; i < users.Count; i++)
                {
                    TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData
                    {
                        text = users[i].Account
                    };
                    optionDatas.Add(data);
                }
                _studentsDrop.AddOptions(optionDatas);
                _currentStudentAccount = _studentsDrop.options[0].text;
                _usernameText.text = AppController.Instance.currentStudents.Find(a => a.Account == _currentStudentAccount).Username;
                currentPage.Init();
            }
        }
        else {
            _ = NetHelper.Instance.GetUserByClasses(AppController.Instance.softData, (datas) =>
            {
                AppController.Instance.currentStudents = datas;
                List<User> users = AppController.Instance.currentStudents.FindAll(a => a.Classinfo == _currentSelectClass);
                if (users != null && users.Count > 0)
                {
                    _studentsDrop.ClearOptions();
                    List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
                    for (int i = 0; i < users.Count; i++)
                    {
                        TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData
                        {
                            text = users[i].Account
                        };
                        optionDatas.Add(data);
                    }
                    _studentsDrop.AddOptions(optionDatas);
                    _currentStudentAccount = _studentsDrop.options[0].text;
                    _usernameText.text = AppController.Instance.currentStudents.Find(a => a.Account == _currentStudentAccount).Username;
                    currentPage.OnRefresh();
                }
            });
        }
    }
    private void GetClassesStudents_Refresh() {
        if (_selectClassDrop.options[0].text == "暂无班级") return;
        _ = NetHelper.Instance.GetUserByClasses(AppController.Instance.softData, (datas) =>
        {
            AppController.Instance.currentStudents = datas;
            List<User> users = AppController.Instance.currentStudents.FindAll(a => a.Classinfo == _currentSelectClass);
            if (users != null && users.Count > 0)
            {
                _studentsDrop.ClearOptions();
                List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
                for (int i = 0; i < users.Count; i++)
                {
                    TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData
                    {
                        text = users[i].Account
                    };
                    optionDatas.Add(data);
                }
                _studentsDrop.AddOptions(optionDatas);
                _currentStudentAccount = _studentsDrop.options[0].text;
                _usernameText.text = AppController.Instance.currentStudents.Find(a => a.Account == _currentStudentAccount).Username;
                currentPage.OnRefresh();
            }
        });
    }


    private void UpdateStudentDrop(string classname) {
        List<User> users = AppController.Instance.currentStudents.FindAll(a => a.Classinfo == classname);
        if (users != null && users.Count > 0)
        {
            _studentsDrop.ClearOptions();
            List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < users.Count; i++)
            {
                TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData
                {
                    text = users[i].Account
                };
                optionDatas.Add(data);
            }
            _studentsDrop.AddOptions(optionDatas);
            _currentStudentAccount = _studentsDrop.options[0].text;
            _usernameText.text = AppController.Instance.currentStudents.Find(a => a.Account == _currentStudentAccount).Username;
        }
    }
    public void ShowDetailWindow(SoftwareLearningData data)
    {
        _teacherTjPanel.ShowDetailWindow(data);
    }
}
