using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class MainMenu : BaseUI
{
    #region sin
    #region 标题按钮
    private Button _btnRelease;

    private Button btnRelease =>
        _btnRelease ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btnRelease");



    //    //最小化
    private Button _smallBtn;

    private Button smallBtn => _smallBtn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btnSmall");

    //    /// <summary>
    //    /// 窗口化
    //    /// </summary>
    private Button _windowBtn;

    private Button windowBtn => _windowBtn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btnWindow");

    ////退出
    private Button _exitBtn;

    private Button exitBTN => _exitBtn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btnExit");

    //返回模式选择
    private Button _backtoModeBtn;

    private Button backtoMode =>
        _backtoModeBtn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "ContentBackButton");
    #endregion
    #region 模式切换按钮
    /// <summary>
    /// 认知按钮
    /// </summary>
    private Button _renzhi_btn;

    private Button renzhi_btn => _renzhi_btn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "renzhi_btn");
    /// <summary>
    /// 实训按钮
    /// </summary>
    private Button _shixun_btn;

    private Button shixun_btn => _shixun_btn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "shixun_btn");

    /// <summary>
    /// 个人中心按钮
    /// </summary>
    private Button _zhuye_btn;

    private Button zhuye_btn => _zhuye_btn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "zhuye_btn");



    /// <summary>
    /// 设置按钮
    /// </summary>
    private Button _setBtn;

    private Button setBtn => _setBtn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btnSetting");



    #endregion
    #endregion
    public override EnumUIType GetUIType()
    {
        return EnumUIType.MainMenu;
    }
    protected override void OnStart()
    {
        base.OnStart();
        InitUserInfo();
        //标题菜单按钮绑定
        EventTriggerListener.Get(setBtn).SetEventHandle(EnumTouchEventType.OnClick, OnSetBtnClick, null); //打开设置界面
        EventTriggerListener.Get(smallBtn).SetEventHandle(EnumTouchEventType.OnClick, onMinimumkClick, null); //最小化
        EventTriggerListener.Get(windowBtn).SetEventHandle(EnumTouchEventType.OnClick, onFullscreenClick, null); //窗口化
        EventTriggerListener.Get(exitBTN).SetEventHandle(EnumTouchEventType.OnClick, onQuitAppClick, null); //退出
        //信息菜单绑定
        EventTriggerListener.Get(zhuye_btn).SetEventHandle(EnumTouchEventType.OnClick, onMessageClick, null); //用户信息
        //切换模式按钮绑定
        EventTriggerListener.Get(renzhi_btn).SetEventHandle(EnumTouchEventType.OnClick, onrenzhiClick, null); //用户信息
        EventTriggerListener.Get(shixun_btn).SetEventHandle(EnumTouchEventType.OnClick, onshixunClick, null); //用户信息
    }

    public virtual void InitUserInfo()
    {
        UserName.text = AppController.Instance.loginUser.Username;
        ID.text = "ID:" + AppController.Instance.loginUser.Account;
        loginTime.text = "TIME:" + DateTime.Now.ToString();
        AppController.Instance.courseType = CourseType.None;
        AppController.Instance.menuPanelType = MenuPanelType.None;
       // LayoutRebuilder.ForceRebuildLayoutImmediate(horizontalLayoutGroup.GetComponent<RectTransform>());
    }
    private void OnSetBtnClick(GameObject listener, object eventData, object[] args)
    {
        OpenUI(EnumUIType.SettingPanel);
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
    private void onMessageClick(GameObject listener, object eventData, object[] args)
    {
        Debug.Log("个人主页");
        if (AppController.Instance.userType != UserType.游客)
        {
            if (AppController.Instance.userType == UserType.学生)
            {
                CloseUIToBeOpenUI(EnumUIType.StudentTjPanel);
            }
            else
            {
                CloseUIToBeOpenUI(EnumUIType.TeacherTjPanel);
            }

            AppController.Instance.CurrentTaskMessage.gamemode = Gamemode.ZhuChangJing;
        }
        else
        {
            UIManager.Instance.OpenMessageBoxUI("提示", "请登录!", 5, EnumMessageBoxType.OK);
        }
    }
    private void onrenzhiClick(GameObject listener, object eventData, object[] args)
    {
        CloseUIToBeOpenUI(EnumUIType.CognitiveMenuPanel);
    }
    private void onshixunClick(GameObject listener, object eventData, object[] args)
    {
        CloseUIToBeOpenUI(EnumUIType.LoadingUI, true, EnumUIType.TraniningPanel, EnumSceneType.TrainScene_PC);
    }

    protected override void OnBtnRelease(GameObject listener, object eventData, params object[] args)
    {
        //Debug.Log("Clikc Back but");
        //返回按钮功能记录包含返回登录场景   一级  二级  三级 菜单


        CloseUIToBeOpenUI(EnumUIType.LoginUI);
        ResetPlayer();
        AppController.Instance.CurrentTaskMessage.gamemode = Gamemode.ZhuChangJing;

    }
}
