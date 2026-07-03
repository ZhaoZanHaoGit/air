using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AdministratorPanel : BaseUI
{
    public override EnumUIType GetUIType()
    {
        return EnumUIType.AdministratorPanel;
    }
    #region 
    private TMP_Text _usernameText;
    private TMP_Text usernameText=>_usernameText??= UnityHelper.GetTheChildComponent<TMP_Text>(CacheGameObject, "username_Title");
    private TMP_Text _timetitleText;
    private TMP_Text timetitleText => _timetitleText ??= UnityHelper.GetTheChildComponent<TMP_Text>(CacheGameObject, "time_title");
 
    private Toggle _teacherInfoToggle;
    private Toggle teacherInfoToggle => _teacherInfoToggle ??= UnityHelper.GetTheChildComponent<Toggle>(CacheGameObject, "Toggle_0");
    private Toggle _studentInfoToggle;
    private Toggle studentInfoToggle => _studentInfoToggle ??= UnityHelper.GetTheChildComponent<Toggle>(CacheGameObject, "Toggle_1");
   

    private Button _backBtn;
    private Button backBtn => _backBtn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "backButton");
    private VerticalLayoutGroup _toggleGroup;
    private VerticalLayoutGroup toggleGroup => _toggleGroup ??= UnityHelper.GetTheChildComponent<VerticalLayoutGroup>(CacheGameObject, "Group");

    private Button _refreshButton;
    private Button refreshButton => _refreshButton ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "refreshButton");
    private Button _minimumButton;
    private Button minimumButton => _minimumButton ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "minimumButton");
    private Button _fullscreenButton;
    private Button fullscreenButton => _fullscreenButton ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "fullscreenButton");
    private Button _quitButton;
    private Button quitButton => _quitButton ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "quitButton");


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
    public static BasePanel currentPanel;
    BasePanel oldT = null;
    private MenuToggleInfo[] _menuToggles;
    private MenuToggleInfo[] menuToggles
    {
        get
        {
            _menuToggles = toggleGroup.transform.GetComponentsInChildren<MenuToggleInfo>();
            return _menuToggles;
        }
    }
    //private TMP_InputField _topSearchInput;
    //private TMP_InputField topSearchInput=>_topSearchInput?? UnityHelper.GetTheChildComponent<TMP_InputField>(CacheGameObject, "TopSearch_InputField");
    //private Button _topSearchBtn;
    //private Button topSearchBtn=>_topSearchBtn?? UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "TopSearchButton");
    #endregion
    protected override void OnAwake()
    {
        base.OnAwake();
        usernameText.text = "欢迎您，" + AppController.Instance.loginUser.Username+"(管理员)";
        timetitleText.text = DateTime.Now.ToString("yyyy年MM月dd日");
        if (panelsDic.Count>0) {
            panelsDic.Clear();
        }
        for (int i = 0; i < panels.Length; i++)
        {
            panelsDic.Add((int)panels[i].GetUIType(), panels[i]);
        }
        Debug.Log("panels---" + panels.Length);
        Debug.Log("panelsDic---" + panelsDic.Count);
        currentPanel = DictionaryExtension.TryGet(panelsDic, 0);
        oldT = currentPanel;
    }
    protected override void OnStart()
    {
        base.OnStart();   
        EventTriggerListener.Get(teacherInfoToggle).SetEventHandle(EnumTouchEventType.OnClick, onToggleClick, null, (int)PanelUIType.AdminTeacherManagement_Panel);
        EventTriggerListener.Get(studentInfoToggle).SetEventHandle(EnumTouchEventType.OnClick, onToggleClick, null, (int)PanelUIType.AdminStudentManagement_Panel);
       

        EventTriggerListener.Get(backBtn).SetEventHandle(EnumTouchEventType.OnClick, onBackClick, null);
        EventTriggerListener.Get(refreshButton).SetEventHandle(EnumTouchEventType.OnClick, onRefreshClick, UnityHelper.CreateHashtable(EnumHashtableParamsType.LockSelfClick, 5f));
        EventTriggerListener.Get(minimumButton).SetEventHandle(EnumTouchEventType.OnClick, onMinimumkClick, null);
        EventTriggerListener.Get(fullscreenButton).SetEventHandle(EnumTouchEventType.OnClick, onFullscreenClick, null);
        EventTriggerListener.Get(quitButton).SetEventHandle(EnumTouchEventType.OnClick, onQuitAppClick, null);

        //topSearchBtn.onClick.AddListener(() =>
        //{
        //    MenuSearch(topSearchInput.text.Trim());
        //});
        //topSearchInput.onEndEdit.AddListener(value =>
        //{
        //    MenuSearch(value);
        //});

        currentPanel.OnRefresh();
    }
    string oldValue;
    private void MenuSearch(string value) {
        if (oldValue == value) return;   
        foreach (var item in menuToggles)
        {
            if (item.name.Contains( value)) {
                item.GetComponent<Toggle>().isOn = true;           
                currentPanel= GetPanel((int)item.type);
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
        }
        oldValue = value;
    }
    private void onQuitAppClick(GameObject listener, object eventData, object[] args)
    {
        UIManager.Instance.OpenMessageBoxUI("提示", "确认退出！", 0, EnumMessageBoxType.OKCancel, OnQuit);
    }

    private void OnQuit(object args)
    {
#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;
#else
       CanvasController.Instance.DoQuitGame(null);
#endif
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
    private void onBackClick(GameObject listener, object eventData, object[] args)
    {
        UIManager.Instance.OpenMessageBoxUI("提示", "确认退出登录！", 0, EnumMessageBoxType.OKCancel, OnBack);
    }

    private void OnBack(object args)
    {
        CloseUIToBeOpenUI(EnumUIType.LoginUI);
        AppController.Instance.OnReset();
        _ = NetHelper.Instance.OnSignOutClick();
    }
    private void onToggleClick(GameObject listener, object eventData, object[] args)
    {
        oldValue = "";
        //topSearchInput.text = "";
        int temp = (int)args[0];

        currentPanel = DictionaryExtension.TryGet(panelsDic, temp);
        Debug.Log("currentPanel----"+currentPanel);
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
    public static BasePanel GetPanel(int index) {
       return DictionaryExtension.TryGet(panelsDic, index);
    }
}
