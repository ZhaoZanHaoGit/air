using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class StudentZHTj_Panel : BasePanel
{
    public override PanelUIType GetUIType()
    {
        return PanelUIType.StudentZHTj_Panel;
    }
    #region
    private Toggle _weekToggle, _monthToggle, _customToggle;
    private Button _queryBtn;
    private Toggle _kctjjlToggle, _zsdxxtjoggle, _sxcztjToggle, _sxcjtjToggle;
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
    private StudentTjPanel _studentTjPanel {
        get { return UIManager.Instance.GetUI<StudentTjPanel>(EnumUIType.StudentTjPanel); }
    }
    #endregion
    public override void Awake()
    {
        base.Awake();
        _kctjjlToggle = transform.Find("view/fixed/selectMenu/Toggle_0").GetComponent<Toggle>();
        _zsdxxtjoggle = transform.Find("view/fixed/selectMenu/Toggle_1").GetComponent<Toggle>();
        _sxcztjToggle = transform.Find("view/fixed/selectMenu/Toggle_2").GetComponent<Toggle>();
        _sxcjtjToggle = transform.Find("view/fixed/selectMenu/Toggle_3").GetComponent<Toggle>();
        _weekToggle=transform.Find("view/fixed/selectTime/toggles/Toggle").GetComponent<Toggle>();
        _monthToggle = transform.Find("view/fixed/selectTime/toggles/Toggle_1").GetComponent<Toggle>();
        _customToggle = transform.Find("view/fixed/selectTime/toggles/Toggle_2").GetComponent<Toggle>();
        _queryBtn = transform.Find("view/fixed/selectTime/sourButton").GetComponent<Button>();
        _weekToggle.onValueChanged.AddListener((isOn) =>
        {
            if (isOn) {
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
                _studentTjPanel.ShowSelectTimeWindow();
            }
            else {
                _studentTjPanel.HideSelectTimeWindow();
            }
        });
        _queryBtn.onClick.AddListener(() =>
        {
            //Debug.Log(currentPage.name);
            currentPage.Init();
        });
        if (pagesDic.Count > 0)
        {
            pagesDic.Clear();
        }
        for (int i = 0; i < pages.Length; i++)
        {
            pagesDic.Add((int)pages[i].GetUIType(), pages[i]);
        }
        currentPage = DictionaryExtension.TryGet(pagesDic, (int)PageUIType.StudentCourseStatistics_Page);
        oldT = currentPage;
        TimeSubtraction(7, ref _StartTime, ref _EndTime);
    }
    private void Start()
    {
       
        _kctjjlToggle.onValueChanged.AddListener((isOn) =>
        {
            OnMenuToggleClick((int)PageUIType.StudentCourseStatistics_Page);
        });
        _zsdxxtjoggle.onValueChanged.AddListener((isOn) =>
        {
            OnMenuToggleClick((int)PageUIType.SSKnowledgePointTaskStatistics_Page);
        });
        _sxcztjToggle.onValueChanged.AddListener((isOn) =>
        {
            OnMenuToggleClick((int)PageUIType.SSsxcztj_Page);
        });
        _sxcjtjToggle.onValueChanged.AddListener((isOn) =>
        {
            OnMenuToggleClick((int)PageUIType.SSDetailedPracticalTrainingTasks_Page);
        });
       // currentPage.Init();
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
    public void SetQueryTime(DateTime starttime,DateTime endTime) {
        _StartTime=starttime;
        _EndTime=endTime;
    }
    //时间相减计算
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
    public override void Init()
    {
        currentPage.Init();
    }
 
    public override void OnRefresh()
    {
        currentPage.OnRefresh();    
    }
    public void ShowDetailWindow(SoftwareLearningData data) {
        _studentTjPanel.ShowDetailWindow (data);
    }


    public static BasePage GetPage(int index)
    {
        return DictionaryExtension.TryGet(pagesDic, index);
    }
}
