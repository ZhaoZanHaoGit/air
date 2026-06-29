
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.UI基类
 *
 *  Author:				
 *       
 *  Date:              
 * 
 ******************************************************************************/

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class BaseUI : MonoBehaviour
{

    //用户信息显示
    public TMP_Text UserName, ID, loginTime;
    public HorizontalLayoutGroup horizontalLayoutGroup;



    protected virtual int UIOrder => -1;
    public virtual bool EscapeClose => true;
    public virtual float WaitCloseUI => 0.25f;
    public virtual float WaitAnimUI => 0.25f;
    public EnumUIType ToBeOpenUI { get; set; } = EnumUIType.None;
    public virtual object[] UIparams { get; set; }
    public virtual bool IsOnlyClose => false;
    public abstract EnumUIType GetUIType();

    private GameObject _cacheGameObjet;
    public GameObject CacheGameObject
    {
        get
        {
            if (null == _cacheGameObjet)
                _cacheGameObjet = gameObject;
            return _cacheGameObjet;
        }
    }

    private Transform _cacheTransform;
    public Transform CacheTransform
    {
        get
        {
            if (null == _cacheTransform)
                _cacheTransform = transform;
            return _cacheTransform;
        }
    }

    protected EnumObjectState state = EnumObjectState.None;
    public event StateChangeEvent StateChanged;
    public EnumObjectState State
    {
        get => state;
        protected set
        {
            if (value == state) return;
            var oldState = state;
            state = value;
            StateChanged?.Invoke(this, state, oldState);
        }
    }

    private UnityAction _closeAction;


    public void Awake()
    {
        State = EnumObjectState.Initial;
        OnAwake();
        State = EnumObjectState.Loading;
        OnPlayOpenUIAudio();

    }
    private void Start()
    {
        if (UIOrder > -1)
        {
            var canvas = this.GetOrAddComponent<Canvas>();
            this.GetOrAddComponent<GraphicRaycaster>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = UIOrder;
        }
        var btnRelease = UnityHelper.FindTheChild(gameObject, "btnRelease");
        if (btnRelease)
            EventTriggerListener.Get(btnRelease).SetEventHandle(EnumTouchEventType.OnClick, OnBtnRelease,
                UnityHelper.CreateHashtable(EnumHashtableParamsType.LockAllClick, ""));
        OnStart();
    }
    public virtual void InitUserInfo()
    {
        UserName.text = AppController.Instance.loginUser.Username;
        ID.text = "ID:" + AppController.Instance.loginUser.Account;
        loginTime.text = "TIME:" + DateTime.Now.ToString();
        //LayoutRebuilder.ForceRebuildLayoutImmediate(horizontalLayoutGroup.GetComponent<RectTransform>());
    }

    protected virtual void OnBtnRelease(GameObject listener, object eventData, params object[] args)
    {
        CloseUI();
    }

    protected void CloseUI()
    {
        UIManager.Instance.CloseUI(GetUIType());
    }
    protected void CloseUIToBeOpenUI(EnumUIType uiType, params object[] uiParams)
    {
        ToBeOpenUI = uiType;
        UIparams = uiParams;
        CloseUI();
    }

    protected void OpenUI(EnumUIType uiType)
    {

        UIManager.Instance.OpenUI(uiType);
    }
    private void Update()
    {
        if (State == EnumObjectState.Ready)
        {
            OnUpdate(Time.deltaTime);
        }

    }
    public void Release()
    {
        State = EnumObjectState.Closing;
        OnRelease();
        StartCoroutine(closeUICor());
    }
    private IEnumerator closeUICor()
    {

        yield return new WaitForSeconds(WaitCloseUI);
        if (!IsOnlyClose)
        {
            if (ToBeOpenUI != EnumUIType.None)
            {
                UIManager.Instance.OpenUI(ToBeOpenUI, UIparams);
            }
            else
            {
                if (CanvasController.Instance.StackOpenUIOrder.Count > 0)
                {
                    var data = CanvasController.Instance.StackOpenUIOrder.Pop();
                    UIManager.Instance.OpenUI(data.UIType, data.UIparams);
                }
            }
        }
        Destroy(CacheGameObject);
    }

    private void OnDestroy()
    {
        State = EnumObjectState.None;
        OnPlayCloseUIAudio();
    }

    //ui层级设置
    protected virtual void SetDepthToTop() { }

    protected virtual void OnAwake()
    {

    }

    protected virtual void OnStart()
    {
        OnPlayOpenUIAnimation();
    }

    protected virtual void OnUpdate(float deltaTime) { }

    protected virtual void OnLoadData()
    {

    }

    protected virtual void OnRelease()
    {
        OnPlayCloseUIAnimation();
    }

    protected virtual void OnPlayOpenUIAudio() { }
    protected virtual void OnPlayCloseUIAudio() { }
    protected virtual void OnPlayOpenUIAnimation() { }
    protected virtual void OnPlayCloseUIAnimation() { }

    protected virtual void SetUI(params object[] uiParams)
    {
        CacheTransform.SetParent(CanvasController.Instance.MainUIParent, false);
        State = EnumObjectState.Loading;
    }

    public void SetUIWhenOpening(params object[] uiParams)
    {
        SetUI(uiParams);
        State = EnumObjectState.Loading;
        StartCoroutine(LoadDataAsyn());
    }

    private IEnumerator LoadDataAsyn()
    {
        yield return new WaitForSeconds(0f);
        if (State != EnumObjectState.Loading) yield break;
        OnLoadData();
        State = EnumObjectState.Ready;
    }

    public Transform Find(string path)
    {
        return CacheTransform.Find(path);
    }
    public void QuitSoftAndApp()
    {
        if (AppController.Instance.loginUser.Usertype == (int)UserType.学生)
        {
            if (AppController.Instance.softData == null || AppController.Instance.softData.IsOnline == -1)
            {
#if UNITY_EDITOR

                UnityEditor.EditorApplication.isPlaying = false;
#else
       CanvasController.Instance.DoQuitGame(null);
#endif
            }
            else
            {
                _ = NetHelper.Instance.QuitSoft(AppController.Instance.softData, () =>
                {
#if UNITY_EDITOR

                    UnityEditor.EditorApplication.isPlaying = false;
#else
       CanvasController.Instance.DoQuitGame(null);
#endif
                });
            }
        }
        else
        {
#if UNITY_EDITOR

            UnityEditor.EditorApplication.isPlaying = false;
#else
       CanvasController.Instance.DoQuitGame(null);
#endif
        }
    }
    public void ResetPlayer()
    {
        if (AppController.Instance.userType != UserType.游客)
        {
            if (AppController.Instance.loginUser.Usertype == (int)UserType.学生)
            {
                if (AppController.Instance.softData != null || AppController.Instance.softData.IsOnline == 1)
                {
                    _ = NetHelper.Instance.QuitSoft(AppController.Instance.softData, () =>
                    {
                        _ = NetHelper.Instance.OnSignOutClick();
                        AppController.Instance.OnReset();
                    });
                }
            }
            else
            {
                _ = NetHelper.Instance.OnSignOutClick();
                AppController.Instance.OnReset();
            }
        }
    }
}
