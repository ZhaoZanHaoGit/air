using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeacherClassTj_Panel : BasePanel
{
    public override PanelUIType GetUIType()
    {
        return PanelUIType.TeacherClassTj_Panel;
    }
    #region
    private Toggle _weekToggle, _monthToggle, _customToggle;
    private Button _queryBtn;
    private TMP_Dropdown _selectClassDrop;

    private Toggle _learnCountToggle, _zsdrwTjToggle, _sxrwToggle;
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
    public static BasePage currentPage= null;
    private BasePage oldT = null;
    [HideInInspector]
    public DateTime _StartTime, _EndTime;
    [HideInInspector]
    public string _currentSelectClass;
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
        _queryBtn = transform.Find("teacherview/fixed/selectTime/sourButton").GetComponent<Button>();
        _learnCountToggle =transform.Find("teacherview/fixed/selectMenu/Toggle_0").GetComponent<Toggle>();
        _zsdrwTjToggle = transform.Find("teacherview/fixed/selectMenu/Toggle_1").GetComponent<Toggle>();
        _sxrwToggle = transform.Find("teacherview/fixed/selectMenu/Toggle_2").GetComponent<Toggle>();
        _selectClassDrop = transform.Find("teacherview/fixed/selectTime/Dropdown").GetComponent<TMP_Dropdown>();
        if (pagesDic.Count > 0)
        {
            pagesDic.Clear();
        }
        for (int i = 0; i < pages.Length; i++)
        {
            pagesDic.Add((int)pages[i].GetUIType(), pages[i]);
        }
        currentPage = DictionaryExtension.TryGet(pagesDic, (int)PageUIType.TSKnowledgePointTaskStatistics_Page);
        oldT = currentPage;
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
        TimeSubtraction(7, ref _StartTime, ref _EndTime);
    }
    private void Start()
    {
        _learnCountToggle.onValueChanged.AddListener((isOn) =>
        {
            OnMenuToggleClick((int)PageUIType.TSKnowledgePointTaskStatistics_Page);
        });
        _zsdrwTjToggle.onValueChanged.AddListener((isOn) =>
        {
            OnMenuToggleClick((int)PageUIType.TSsxcztj_Page);
        });
        _sxrwToggle.onValueChanged.AddListener((isOn) =>
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
        _queryBtn.onClick.AddListener(() =>
        {
            currentPage.Init();
        });
        _selectClassDrop.onValueChanged.AddListener((value) =>
        {
            _currentSelectClass= _selectClassDrop.options[value].text;
        });
    }
    private void OnMenuToggleClick(int args) {
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
        currentPage.Init();
    }

    public override void OnRefresh()
    {
        currentPage.OnRefresh();
    }
}
