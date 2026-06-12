using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StudentTjPanel : BaseUI
{
    public override EnumUIType GetUIType()
    {
        return EnumUIType.StudentTjPanel;   
    }
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

    private Toggle _studentCenterToggle;
    private Toggle studentCenterToggle => _studentCenterToggle ??= UnityHelper.GetTheChildComponent<Toggle>(CacheGameObject, "studentCenterToggle");

    private Toggle _integratedStatisticsToggle;
    private Toggle integratedStatisticsToggle => _integratedStatisticsToggle ??= UnityHelper.GetTheChildComponent<Toggle>(CacheGameObject, "integratedStatisticsToggle");
   
    private Toggle _aiAgentToggle;
    private Toggle aiAgentToggle => _aiAgentToggle ??= UnityHelper.GetTheChildComponent<Toggle>(CacheGameObject, "agentToggle");


    private Button _refreshButton;
    private Button refreshButton => _refreshButton ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "refreshButton");
    private Button _minimumButton;
    private Button minimumButton => _minimumButton ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "minimumButton");
    private Button _fullscreenButton;
    private Button fullscreenButton => _fullscreenButton ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "fullscreenButton");
    private Button _quitButton;
    private Button quitButton => _quitButton ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "quitButton");
    private EditorStudent_Panel _editorStudent_Panel;
    private EditorStudent_Panel editorStudent_Panel => _editorStudent_Panel ??= UnityHelper.GetTheChildComponent<EditorStudent_Panel>(CacheGameObject, "EditorStudent_Panel");
    private SelectTimeForm _selectTimeForm;
    private SelectTimeForm selectTimeForm=>_selectTimeForm??= UnityHelper.GetTheChildComponent<SelectTimeForm>(CacheGameObject, "SelectTimeForm");
    private ProductDetailPage _productDetailPage;
    private ProductDetailPage productDetailPage=> _productDetailPage??= UnityHelper.GetTheChildComponent<ProductDetailPage>(CacheGameObject, "DetailPage​");
    #endregion
    protected override void OnAwake()
    {
        base.OnAwake();
        loginnameText.text = "欢迎你，" + AppController.Instance.loginUser.Username + "(同学)";
        timetitleText.text = DateTime.Now.ToString("yyyy年MM月dd日");
        if (panelsDic.Count > 0)
        {
            panelsDic.Clear();
        }
        for (int i = 0; i < panels.Length; i++)
        {
            panelsDic.Add((int)panels[i].GetUIType(), panels[i]);
        }
        currentPanel = DictionaryExtension.TryGet(panelsDic, (int)PanelUIType.StudentCenter_Panel);
        oldT = currentPanel;
    }
    protected override void OnStart()
    {
        base.OnStart();
        EventTriggerListener.Get(studentCenterToggle).SetEventHandle(EnumTouchEventType.OnClick, onToggleClick, null, (int)PanelUIType.StudentCenter_Panel);
        EventTriggerListener.Get(integratedStatisticsToggle).SetEventHandle(EnumTouchEventType.OnClick, onToggleClick, null, (int)PanelUIType.StudentZHTj_Panel);
        EventTriggerListener.Get(aiAgentToggle).SetEventHandle(EnumTouchEventType.OnClick, onToggleClick, null, (int)PanelUIType.AIAgent_Panel);
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
    protected override void OnBtnRelease(GameObject listener, object eventData, params object[] args)
    {
        CloseUIToBeOpenUI(EnumUIType.MainMenu);
    }
    public void ResetLogin() {
        CloseUIToBeOpenUI(EnumUIType.LoginUI);
        ResetPlayer();
    }
    public void OpenEidtorUserPanel()
    {
        editorStudent_Panel.OnShow();
        editorStudent_Panel.Init(AppController.Instance.loginUser);
    }
    public void ShowSelectTimeWindow() {
        selectTimeForm.Show();
    }
    public void HideSelectTimeWindow()
    {
        selectTimeForm.Hide();
    }
    public void ShowDetailWindow(SoftwareLearningData data) {
        productDetailPage.OnShow();
        productDetailPage.Init(data);
    }
    public static BasePanel GetPanel(int index)
    {
        return DictionaryExtension.TryGet(panelsDic, index);
    }
  
}
