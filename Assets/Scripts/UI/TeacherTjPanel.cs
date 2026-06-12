using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeacherTjPanel : BaseUI
{
    #region 
    private TMP_Text _loginnameText;
    private TMP_Text loginnameText => _loginnameText ??= UnityHelper.GetTheChildComponent<TMP_Text>(CacheGameObject, "username_Title");
    private TMP_Text _timetitleText;
    private TMP_Text timetitleText => _timetitleText ??= UnityHelper.GetTheChildComponent<TMP_Text>(CacheGameObject, "time_title");

    private BasePanel[] _panels;
    private BasePanel[] panels
    {
        get
        {
            _panels = GetComponentsInChildren<BasePanel>();
            return _panels;
        }
    }
    private static Dictionary<int, BasePanel> panelsDic = new Dictionary<int, BasePanel>();
    public static BasePanel currentPanel = null;
    private BasePanel oldT = null;

    private Toggle _teacherCenterToggle;
    private Toggle teacherCenterToggle => _teacherCenterToggle ??= UnityHelper.GetTheChildComponent<Toggle>(CacheGameObject, "teacherCenterToggle");

    private Toggle _classStatisticsToggle;
    private Toggle classStatisticsToggle => _classStatisticsToggle ??= UnityHelper.GetTheChildComponent<Toggle>(CacheGameObject, "classStatisticsToggle");

    private Toggle _studentStatisticsToggle;
    private Toggle studentStatisticsToggle => _studentStatisticsToggle ??= UnityHelper.GetTheChildComponent<Toggle>(CacheGameObject, "studentStatisticsToggle");

    private Toggle _methumanAIToggle;
    private Toggle methumanAIToggle => _methumanAIToggle ??= UnityHelper.GetTheChildComponent<Toggle>(CacheGameObject, "methumanAIToggle");

    private Toggle _kaoheSetToggle;
    private Toggle kaoheSetToggle => _kaoheSetToggle ??= UnityHelper.GetTheChildComponent<Toggle>(CacheGameObject, "kaoheToggle");

    private Toggle _taskPostingToggle;
    private Toggle taskPostingToggle => _taskPostingToggle ??= UnityHelper.GetTheChildComponent<Toggle>(CacheGameObject, "taskPostingToggle");



    private Button _refreshButton;
    private Button refreshButton => _refreshButton ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "refreshButton");
    private Button _minimumButton;
    private Button minimumButton => _minimumButton ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "minimumButton");
    private Button _fullscreenButton;
    private Button fullscreenButton => _fullscreenButton ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "fullscreenButton");
    private Button _quitButton;
    private Button quitButton => _quitButton ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "quitButton");
    private EditorTeacher_Panel _editorTeacher_Panel;
    private EditorTeacher_Panel editorTeacher_Panel => _editorTeacher_Panel ??= UnityHelper.GetTheChildComponent<EditorTeacher_Panel>(CacheGameObject, "EditorTeacher_Panel");

    private SelectTimeForm _selectTimeForm;
    private SelectTimeForm selectTimeForm => _selectTimeForm ??= UnityHelper.GetTheChildComponent<SelectTimeForm>(CacheGameObject, "SelectTimeForm");
    private ProductDetailPage _productDetailPage;
    private ProductDetailPage productDetailPage => _productDetailPage ??= UnityHelper.GetTheChildComponent<ProductDetailPage>(CacheGameObject, "DetailPage​");
    #endregion
    protected override void OnAwake()
    {
        base.OnAwake();
        loginnameText.text = "欢迎您，" + AppController.Instance.loginUser.Username + "(教师)";
        timetitleText.text = DateTime.Now.ToString("yyyy年MM月dd日");
        if (panelsDic.Count > 0)
        {
            panelsDic.Clear();
        }
        for (int i = 0; i < panels.Length; i++)
        {
            panelsDic.Add((int)panels[i].GetUIType(), panels[i]);
        }
        currentPanel = DictionaryExtension.TryGet(panelsDic, (int)PanelUIType.TeacherCenter_Panel);
        oldT = currentPanel;

    }
    protected override void OnStart()
    {
        base.OnStart();
        EventTriggerListener.Get(teacherCenterToggle).SetEventHandle(EnumTouchEventType.OnClick, onToggleClick, null, (int)PanelUIType.TeacherCenter_Panel);
        EventTriggerListener.Get(classStatisticsToggle).SetEventHandle(EnumTouchEventType.OnClick, onToggleClick, null, (int)PanelUIType.TeacherClassTj_Panel);
        EventTriggerListener.Get(studentStatisticsToggle).SetEventHandle(EnumTouchEventType.OnClick, onToggleClick, null, (int)PanelUIType.TeacherStudentTj_Panel);
        EventTriggerListener.Get(methumanAIToggle).SetEventHandle(EnumTouchEventType.OnClick, onToggleClick, null, (int)PanelUIType.AIAgent_Panel);
        EventTriggerListener.Get(taskPostingToggle).SetEventHandle(EnumTouchEventType.OnClick, onToggleClick, null, (int)PanelUIType.TaskPosting_Panel);
        EventTriggerListener.Get(kaoheSetToggle).SetEventHandle(EnumTouchEventType.OnClick, onToggleClick, null, (int)PanelUIType.ExamConfiguration_Panel);
        EventTriggerListener.Get(refreshButton).SetEventHandle(EnumTouchEventType.OnClick, onRefreshClick, UnityHelper.CreateHashtable(EnumHashtableParamsType.LockSelfClick, 5f));
        EventTriggerListener.Get(minimumButton).SetEventHandle(EnumTouchEventType.OnClick, onMinimumkClick, null);
        EventTriggerListener.Get(fullscreenButton).SetEventHandle(EnumTouchEventType.OnClick, onFullscreenClick, null);
        EventTriggerListener.Get(quitButton).SetEventHandle(EnumTouchEventType.OnClick, onQuitAppClick, null);
        currentPanel.Init();
    }

    private void onQuitAppClick(GameObject listener, object eventData, object[] args)
    {
        UIManager.Instance.OpenMessageBoxUI("提示", "确认退出！", 0, EnumMessageBoxType.OKCancel, OnQuit);
    }

    private void OnQuit(object args)
    {
        QuitSoftAndApp();
    }

    private void onFullscreenClick(GameObject listener, object eventData, object[] args)
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    private void onMinimumkClick(GameObject listener, object eventData, object[] args)
    {
        Minimze.Minmized();
    }

    private void onRefreshClick(GameObject listener, object eventData, object[] args)
    {
        currentPanel.OnRefresh();
    }

    private void onToggleClick(GameObject listener, object eventData, object[] args)
    {
        int temp = (int)args[0];
        currentPanel = DictionaryExtension.TryGet(panelsDic, temp);
        if (oldT == currentPanel)
            return;
        if (oldT != null)
        {
            oldT.OnHide();
        }
        if (currentPanel)
        {
            currentPanel.OnShow();
            currentPanel.Init();
        }
        oldT = currentPanel;
    }

    public override EnumUIType GetUIType()
    {
        return EnumUIType.TeacherTjPanel;
    }
    protected override void OnBtnRelease(GameObject listener, object eventData, params object[] args)
    {
        CloseUIToBeOpenUI(EnumUIType.MainMenu);
    }
    public static BasePanel GetPanel(int index)
    {
        return DictionaryExtension.TryGet(panelsDic, index);
    }
    public void ResetLogin()
    {
        CloseUIToBeOpenUI(EnumUIType.LoginUI);
        ResetPlayer();
    }
    public void OpenEidtorUserPanel()
    {
        editorTeacher_Panel.OnShow();
        editorTeacher_Panel.Init(AppController.Instance.loginUser);
    }
    public void ShowDetailWindow(SoftwareLearningData data)
    {
        productDetailPage.OnShow();
        productDetailPage.Init(data);
    }
    public void ShowSelectTimeWindow()
    {
        selectTimeForm.Show();
    }
    public void HideSelectTimeWindow()
    {
        selectTimeForm.Hide();
    }
}
