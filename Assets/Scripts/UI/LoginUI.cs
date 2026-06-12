using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LoginUI : BaseUI
{
    public override EnumUIType GetUIType()
    {
        return EnumUIType.LoginUI;
    }
    #region 组件
    private Toggle _studentToggle;
    private Toggle studentToggle => _studentToggle ??= UnityHelper.GetTheChildComponent<Toggle>(CacheGameObject, "StudentToggle");
    private Toggle _teahcerToggle;
    private Toggle teahcerToggle => _teahcerToggle ??= UnityHelper.GetTheChildComponent<Toggle>(CacheGameObject, "TeacherToggle");
    private Toggle _adminToggle;
    private Toggle adminToggle => _adminToggle ??= UnityHelper.GetTheChildComponent<Toggle>(CacheGameObject, "AdminToggle");
    private Transform _loginmessgae;
    private Transform loginmessage => _loginmessgae ??= UnityHelper.FindTheChild(CacheGameObject, "Loginmessage");
    private TMP_InputField _accountInput;
    private TMP_InputField accountInput => _accountInput ??= UnityHelper.GetTheChildComponent<TMP_InputField>(CacheGameObject, "AccountInput");
    private TMP_InputField _passwordInput;
    private TMP_InputField passwordInput => _passwordInput ??= UnityHelper.GetTheChildComponent<TMP_InputField>(CacheGameObject, "PasswordInput");
    private Button _ykloginBtn;
    private Button ykloginBtn => _ykloginBtn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "YKLoginButton");
    private Button _loginBtn;
    private Button loginBtn => _loginBtn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "LoginButton");
    private Button _quitBtn;
    private Button quitBtn => _quitBtn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "QuitButton");

    private GameObject _loginObj;
    private GameObject loginObg => _loginObj ??= transform.Find("LoginBg").gameObject;
    private GameObject _forgotPawObj;
    private GameObject forgotPawObj => _forgotPawObj ??= transform.Find("ForgotPawBg").gameObject;
    private Toggle _remPawToggle;
    private Toggle remPawToggle => _remPawToggle ??= UnityHelper.GetTheChildComponent<Toggle>(CacheGameObject, "remPawToggle");
    private Button _forgotPawBtn;
    private Button forgotPawBtn => _forgotPawBtn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "forgotPawButton");
    private ToggleGroup _toggleGroup;
    private ToggleGroup toggleGroup => _toggleGroup ??= UnityHelper.GetTheChildComponent<ToggleGroup>(CacheGameObject, "UsertypeSelect");

    private TMP_InputField _forgotPaw_AccountInput;
    private TMP_InputField forgotPaw_AccountInput => _forgotPaw_AccountInput ??= UnityHelper.GetTheChildComponent<TMP_InputField>(CacheGameObject, "forgotPaw_AccountInput");
    private TMP_InputField _forgotPaw_EmailInput;
    private TMP_InputField forgotPaw_EmailInput => _forgotPaw_EmailInput ??= UnityHelper.GetTheChildComponent<TMP_InputField>(CacheGameObject, "forgotPaw_EmailInput");
    private TMP_InputField _forgotPaw_NewPawInput;
    private TMP_InputField forgotPaw_NewPawInput => _forgotPaw_NewPawInput ??= UnityHelper.GetTheChildComponent<TMP_InputField>(CacheGameObject, "forgotPaw_NewPawInput");
    private TMP_InputField _forgotPaw_PasswordInput;
    private TMP_InputField forgotPaw_PasswordInput => _forgotPaw_PasswordInput ??= UnityHelper.GetTheChildComponent<TMP_InputField>(CacheGameObject, "forgotPaw_PasswordInput");
    private Button _okBtn;
    private Button okBtn => _okBtn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "okButton");
    private Button _cancelBtn;
    private Button cancelBtn => _cancelBtn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "cancelButton");

    private Button _seeBtn;
    private Button seeBtn => _seeBtn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "seeButton");
    private bool isSee = false;

    private GameObject _maskpanel;
    private GameObject maskpanel => _maskpanel ??= transform.Find("maskpanel").gameObject;
    private CanvasGroup _LoginCanvasGroup, _forgotpawCanvasGroup;
    #endregion
    protected override void OnStart()
    {
        base.OnStart();
        EventTriggerListener.Get(loginBtn).SetEventHandle(EnumTouchEventType.OnClick, onBtnLoginClick, UnityHelper.CreateHashtable(EnumHashtableParamsType.LockSelfClick, 2f));
        EventTriggerListener.Get(ykloginBtn).SetEventHandle(EnumTouchEventType.OnClick, onYKBtnLoginClick, UnityHelper.CreateHashtable(EnumHashtableParamsType.LockSelfClick, 2f));
        EventTriggerListener.Get(quitBtn).SetEventHandle(EnumTouchEventType.OnClick, onQuitClick, UnityHelper.CreateHashtable(EnumHashtableParamsType.LockSelfClick, 0.5f));
        EventTriggerListener.Get(studentToggle).SetEventHandle(EnumTouchEventType.OnClick, onToggleClick, null, UserType.学生);
        EventTriggerListener.Get(teahcerToggle).SetEventHandle(EnumTouchEventType.OnClick, onToggleClick, null, UserType.教师);
        EventTriggerListener.Get(adminToggle).SetEventHandle(EnumTouchEventType.OnClick, onToggleClick, null, UserType.管理员);
        EventTriggerListener.Get(remPawToggle).SetEventHandle(EnumTouchEventType.OnClick, onRemPawToggleClick, null);
        EventTriggerListener.Get(forgotPawBtn).SetEventHandle(EnumTouchEventType.OnClick, onForgotPawClick, null);
        EventTriggerListener.Get(cancelBtn).SetEventHandle(EnumTouchEventType.OnClick, onCancelRemPawClick, null);
        EventTriggerListener.Get(okBtn).SetEventHandle(EnumTouchEventType.OnClick, onOkRemPawClick, null);
        EventTriggerListener.Get(seeBtn).SetEventHandle(EnumTouchEventType.OnClick, onSeePawClick, null);
        _LoginCanvasGroup = transform.Find("LoginBg").GetComponent<CanvasGroup>();
        _forgotpawCanvasGroup = transform.Find("ForgotPawBg").GetComponent<CanvasGroup>();
        User user = WriteAndReadFileData.ReadFromFileExample();
        if (user != null)
        {
            maskpanel.SetActive(true);
            AppController.Instance.userType = (UserType)user.Usertype;
            StartCoroutine(WaitLogin(user));
        }
        else
        {
            initView();
        }
    }

    private void onSeePawClick(GameObject listener, object eventData, object[] args)
    {
        isSee = !isSee;
        GameObject ste1 = seeBtn.transform.GetChild(0).gameObject;
        GameObject ste2 = seeBtn.transform.GetChild(1).gameObject;
        ste1.SetActive(isSee);
        ste2.SetActive(!isSee);
        passwordInput.contentType = isSee ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
        passwordInput.ForceLabelUpdate();
        passwordInput.ActivateInputField();
        passwordInput.caretPosition = _passwordInput.text.Length;
    }

    private void onYKBtnLoginClick(GameObject listener, object eventData, object[] args)
    {
        AppController.Instance.userType = UserType.游客;
        AppController.Instance.loginUser = new User
        {
            Username = "游客",
            Account = "0000",
            Usertype = 4
        };
        CloseUIToBeOpenUI(EnumUIType.MainMenu);
    }

    IEnumerator WaitLogin(User user)
    {
        yield return new WaitForSeconds(1f);
        _ = NetHelper.Instance.Login(user.Account, user.Password, CallbackSuccesslogin, ShowLoginError);
        user = null;
    }
    private void onOkRemPawClick(GameObject listener, object eventData, object[] args)
    {
        string account = forgotPaw_AccountInput.text.Trim();
        string email = forgotPaw_EmailInput.text.Trim();
        string newpassword = forgotPaw_NewPawInput.text.Trim();
        string password = forgotPaw_PasswordInput.text.Trim();
        if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newpassword) || string.IsNullOrEmpty(password))
        {
            ShowLoginError("请输完整信息！");
            return;
        }
        if (newpassword != password)
        {
            ShowLoginError("两次密码输入不一致!");
            return;
        }
        _ = NetHelper.Instance.ForgotPassword(account, email, newpassword, (temp) =>
        {
            if (temp == 0)
            {
                HideForgotPanel();
            }
            else
            {
                ShowLoginError("修改密码失败！");
            }
        });
    }

    private void onCancelRemPawClick(GameObject listener, object eventData, object[] args)
    {
        HideForgotPanel();
    }
    void HideForgotPanel()
    {
        _forgotpawCanvasGroup.DOFade(0, 0.5f).OnComplete(() => {
            forgotPaw_AccountInput.text = "";
            forgotPaw_EmailInput.text = "";
            forgotPaw_NewPawInput.text = "";
            forgotPaw_PasswordInput.text = "";
            _forgotpawCanvasGroup.gameObject.SetActive(false);
        });
        _LoginCanvasGroup.gameObject.SetActive(true);
        _LoginCanvasGroup.DOFade(1, 0.5f);
    }

    void initView()
    {
        if (PlayerPrefs.HasKey("REMPAW"))
        {
            int temp = PlayerPrefs.GetInt("REMPAW");
            remPawToggle.isOn = temp == 1 ? true : false;
            if (temp == 1)
            {
                if (PlayerPrefs.HasKey("USERACCOUNT"))
                    accountInput.text = PlayerPrefs.GetString("USERACCOUNT");
                if (PlayerPrefs.HasKey("USERPASSWORD"))
                    passwordInput.text = PlayerPrefs.GetString("USERPASSWORD");
                if (PlayerPrefs.HasKey("USERTYPE"))
                {
                    int type = PlayerPrefs.GetInt("USERTYPE");
                    AppController.Instance.userType = (UserType)type;
                    toggleGroup.SetAllTogglesOff();
                    studentToggle.isOn = type == 3 ? true : false;
                    teahcerToggle.isOn = type == 2 ? true : false;
                    adminToggle.isOn = type == 1 ? true : false;
                }
            }
        }
        else
        {
            AppController.Instance.userType = UserType.学生;
        }
    }

    private void onForgotPawClick(GameObject listener, object eventData, object[] args)
    {
        _LoginCanvasGroup.DOFade(0, 0.5f).OnComplete(() => {
            _LoginCanvasGroup.gameObject.SetActive(false);
        });
        _forgotpawCanvasGroup.gameObject.SetActive(true);
        _forgotpawCanvasGroup.DOFade(1, 0.5f);
        //loginObg.SetActive(false);
        //forgotPawObj.SetActive(true);
    }

    private void onRemPawToggleClick(GameObject listener, object eventData, object[] args)
    {
        if (remPawToggle.isOn)
        {
            PlayerPrefs.SetInt("REMPAW", 1);
        }
        else
        {
            PlayerPrefs.SetInt("REMPAW", -1);
        }
    }

    private void onToggleClick(GameObject listener, object eventData, object[] args)
    {
        AppController.Instance.userType = (UserType)args[0];
    }

    private void onQuitClick(GameObject listener, object eventData, object[] args)
    {
        UIManager.Instance.OpenMessageBoxUI("提示信息", "确认退出?", 0, EnumMessageBoxType.OKCancel, AppQuit);
    }

    private void AppQuit(object args)
    {
        CanvasController.Instance.DoQuitGame(null);
    }


    private void onBtnLoginClick(GameObject listener, object eventData, object[] args)
    {
        if (string.IsNullOrEmpty(accountInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            ShowLoginError("请输入账号或密码!");
            return;
        }
        //发送请求     
        _ = NetHelper.Instance.Login(accountInput.text.Trim(), passwordInput.text.Trim(), CallbackSuccesslogin, ShowLoginError);
    }
    private void CallbackSuccesslogin()
    {
        maskpanel.SetActive(false);
        if (remPawToggle.isOn)
        {
            PlayerPrefs.SetInt("USERTYPE", (int)AppController.Instance.userType);
            PlayerPrefs.SetString("USERACCOUNT", accountInput.text.Trim());
            PlayerPrefs.SetString("USERPASSWORD", passwordInput.text.Trim());
        }
        else
        {
            if (PlayerPrefs.HasKey("USERTYPE"))
                PlayerPrefs.DeleteKey("USERTYPE");
            if (PlayerPrefs.HasKey("USERPASSWORD"))
                PlayerPrefs.DeleteKey("USERPASSWORD");
            if (PlayerPrefs.HasKey("USERACCOUNT"))
                PlayerPrefs.DeleteKey("USERACCOUNT");
        }
        if (AppController.Instance.userType == UserType.学生 || AppController.Instance.userType == UserType.教师)
        {
            SoftData data = new SoftData
            {
                SoftID = (int)Defines.softInfo,
                SoftName = Defines.softInfo.ToString(),
                Account = AppController.Instance.loginUser.Account,
                UserName = AppController.Instance.loginUser.Username,
                SchoolName = AppController.Instance.loginUser.School_name,
                ClassInfo = AppController.Instance.loginUser.Classinfo,
                UserType = AppController.Instance.loginUser.Usertype
            };
            _ = NetHelper.Instance.AddSoftData(data, (_data) =>
            {
                AppController.Instance.softData = _data;
                if (_data.UserType == 3)
                {
                    if (_data.IsOnline == -1)
                    {
                        UIManager.Instance.OpenMessageBoxUI("提示信息", "该账号已在线，请联系管理员。", 5, EnumMessageBoxType.OKCancel, AppQuit);
                    }
                    else
                    {
                        maskpanel.SetActive(true);
                        //Debug.Log("学生登录成功");
                        //CloseUIToBeOpenUI(EnumUIType.MainMenu);
                        string softID = ((int)Defines.softInfo).ToString();
                        string account = AppController.Instance.loginUser.Account;
                        _ = NetHelper.Instance.GetStudentTasks(softID, account, (datas) =>
                        {
                            if (datas != null && datas.Count > 0)
                            {
                                AppController.Instance.currentStudentTasksDatas = datas;
                                Debug.Log(AppController.Instance.currentStudentTasksDatas.Count);
                            }
                            maskpanel.SetActive(false);
                            CloseUIToBeOpenUI(EnumUIType.MainMenu);
                        });
                    }
                }
                else
                {
                    CloseUIToBeOpenUI(EnumUIType.MainMenu);
                }

            });
        }
        else
        {
            CloseUIToBeOpenUI(EnumUIType.AdministratorPanel);
        }

    }

    #region Login消息
    private bool isFunctionRunning = false;
    void ShowLoginError(string errorStr)
    {
        if (isFunctionRunning)
        {
            CancelInvoke("HideLoginError");
            isFunctionRunning = false;
        }
        loginmessage.gameObject.SetActive(true);
        loginmessage.transform.Find("MessageText").GetComponent<TMP_Text>().text = errorStr;
        Invoke("HideLoginError", 3f);
        isFunctionRunning = true;
    }
    void HideLoginError()
    {
        loginmessage.transform.Find("MessageText").GetComponent<TMP_Text>().text = "";
        loginmessage.gameObject.SetActive(false);
    }
    #endregion
    protected override void OnPlayOpenUIAnimation()
    {
        _LoginCanvasGroup.DOFade(1, 0.25f).SetEase(Ease.InSine);
    }
    protected override void OnPlayCloseUIAnimation()
    {
        _LoginCanvasGroup.DOFade(0, 0.25f).SetEase(Ease.OutSine);
    }
}
