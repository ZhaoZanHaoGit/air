using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XCharts.Runtime;

public class StudentCenter_Panel : BasePanel
{
    public override PanelUIType GetUIType()
    {
        return PanelUIType.StudentCenter_Panel;
    }
    #region 
    //个人信息
    private TMP_Text _usernameText, _sexText, _accountText, _passwordText, _schoolnameText, _positionText, _classinfoText, _phonText, _emallText;
    private Button _passwordBtn;
    private bool isShowPassword = false;
    private GameObject[] _passwordStateObjs;
    private TMP_Text _lastLoginTimeText;
    private TMP_Text _logincountText;
    private Button _editorBtn;
    private BarChart _rzcountChart, _sxcountChart, _khcountChart;
    private Serie _rzcountSerie, _sxcountSerie, _khcountSerie;
    #endregion
    public override void Awake()
    {
        base.Awake();
        _lastLoginTimeText = transform.Find("Views/View3/connect/time").GetComponent<TMP_Text>();
        _usernameText = transform.Find("Views/View1/connect/Scroll View/Viewport/Content/name/value").GetComponent<TMP_Text>();
        _sexText = transform.Find("Views/View1/connect/Scroll View/Viewport/Content/sex/value").GetComponent<TMP_Text>();
        _accountText = transform.Find("Views/View1/connect/Scroll View/Viewport/Content/account/value").GetComponent<TMP_Text>();
        _passwordText = transform.Find("Views/View1/connect/Scroll View/Viewport/Content/password/value").GetComponent<TMP_Text>();
        _schoolnameText = transform.Find("Views/View1/connect/Scroll View/Viewport/Content/school/value").GetComponent<TMP_Text>();
        _positionText = transform.Find("Views/View1/connect/Scroll View/Viewport/Content/position/value").GetComponent<TMP_Text>();
        _classinfoText = transform.Find("Views/View1/connect/Scroll View/Viewport/Content/class/value").GetComponent<TMP_Text>();
        _phonText = transform.Find("Views/View1/connect/Scroll View/Viewport/Content/phone/value").GetComponent<TMP_Text>();
        _emallText = transform.Find("Views/View1/connect/Scroll View/Viewport/Content/emall/value").GetComponent<TMP_Text>();
        _logincountText = transform.Find("Views/View2/connect/count").GetComponent<TMP_Text>();
        _passwordBtn = transform.Find("Views/View1/connect/Scroll View/Viewport/Content/password/Button").GetComponent<Button>();
        _editorBtn = transform.Find("Views/View1/top/Button").GetComponent<Button>();
        _rzcountChart = transform.Find("Views/Scroll View/Viewport/Content/View4/BarChart").GetComponent<BarChart>();
        _sxcountChart = transform.Find("Views/Scroll View/Viewport/Content/View5/BarChart").GetComponent<BarChart>();
        _khcountChart = transform.Find("Views/Scroll View/Viewport/Content/View6/BarChart").GetComponent<BarChart>();
        _passwordStateObjs = new GameObject[_passwordBtn.transform.childCount];
        for (int i = 0; i < _passwordBtn.transform.childCount; i++)
        {
            _passwordStateObjs[i] = _passwordBtn.transform.GetChild(i).gameObject;
        }
    }
    private void Start()
    {
        _passwordBtn.onClick.AddListener(() =>
        {
            isShowPassword = !isShowPassword;
            _passwordStateObjs[isShowPassword ? 1 : 0].SetActive(true);
            _passwordStateObjs[isShowPassword ? 0 : 1].SetActive(false);
            _passwordText.text = isShowPassword ? AppController.Instance.loginUser.Password : new string('*', AppController.Instance.loginUser.Password.Length);
        });
        _editorBtn.onClick.AddListener(() =>
        {
            StudentTjPanel panel = UIManager.Instance.GetUI<StudentTjPanel>(EnumUIType.StudentTjPanel);
            panel.OpenEidtorUserPanel();
        });
    }
    void ShowUserInfo()
    {
        if (AppController.Instance.loginUser == null) return;
        User user = AppController.Instance.loginUser;
        _usernameText.text = user.Username;
        _sexText.text = user.Sex == 1 ? "男" : "女";
        _accountText.text = user.Account;
        _passwordText.text = new string('*', user.Password.Length);
        _schoolnameText.text = user.School_name;
        _positionText.text = user.Position;
        _classinfoText.text = user.Classinfo;
        _phonText.text = user.Phonenumber;
        _emallText.text = user.Email;
    }
    void ShowLastLoginTime()
    {
        _lastLoginTimeText.text = AppController.Instance.softData.LoginTime.ToString();
    }
    void ShowLoginCount()
    {

        _logincountText.text = AppController.Instance.softData.LoginCount.ToString();
    }


    public override void Init()
    {
        ShowUserInfo();
        ShowLastLoginTime();
        ShowLoginCount();
        InitLearnDatas();
    }
    private void UpdateLearnDatas()
    {
        _ = NetHelper.Instance.GetSoftLearnDatasByAccount(AppController.Instance.loginUser.Account,(int)Defines.softInfo, (datas) =>
        {
            AppController.Instance.currentUserLearnDatas = datas;
            ChartHelper.UpdateChart<CLSType>(_rzcountSerie, _rzcountChart,LearnType.LearnCount, CLSType.None);
            ChartHelper.UpdateChart<POTSType>(_sxcountSerie, _sxcountChart, LearnType.LearnCount, POTSType.None);
            ChartHelper.UpdateChart<EVSType>(_khcountSerie, _khcountChart, LearnType.LearnCount, EVSType.None);      
        });
    }
    private void InitLearnDatas() {
        if (AppController.Instance.currentUserLearnDatas == null || AppController.Instance.currentUserLearnDatas.Count <= 0)
        {
            UpdateLearnDatas();
        }
        else {
            ChartHelper.UpdateChart<CLSType>(_rzcountSerie, _rzcountChart, LearnType.LearnCount, CLSType.None);
            ChartHelper.UpdateChart<POTSType>(_sxcountSerie, _sxcountChart, LearnType.LearnCount, POTSType.None);
            ChartHelper.UpdateChart<EVSType>(_khcountSerie, _khcountChart, LearnType.LearnCount, EVSType.None);
        }
    }
    public override void OnRefresh()
    {
        ShowUserInfo();
        ShowLastLoginTime();
        ShowLoginCount();
        UpdateLearnDatas();
    }

}
