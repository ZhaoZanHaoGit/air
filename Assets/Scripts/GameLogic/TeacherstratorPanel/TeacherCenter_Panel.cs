using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XCharts.Runtime;

public class TeacherCenter_Panel : BasePanel
{
    #region 
    //个人信息
    private TMP_Text _usernameText, _sexText, _accountText, _passwordText, _schoolnameText, _positionText, _classinfoText, _phonText, _emallText;
    private Button _passwordBtn;
    private bool isShowPassword = false;
    private GameObject[] _passwordStateObjs;
    private TMP_Text _lastLoginTimeText;
    private TMP_Text _onlineCountText;
    private Button _editorBtn;
    private BarChart _rzcountChart, _sxcountChart, _khcountChart;
    private Serie _rzcountSerie, _sxcountSerie, _khcountSerie;
    #endregion

    public override PanelUIType GetUIType()
    {
        return PanelUIType.TeacherCenter_Panel;
    }
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
        _onlineCountText = transform.Find("Views/View2/connect/count").GetComponent<TMP_Text>();
        _passwordBtn = transform.Find("Views/View1/connect/Scroll View/Viewport/Content/password/Button").GetComponent<Button>();
        _editorBtn = transform.Find("Views/View1/top/Button").GetComponent<Button>();
        _rzcountChart = transform.Find("Views/View4/Scroll View/Viewport/Content/BarChart_1").GetComponent<BarChart>();
        _sxcountChart = transform.Find("Views/View4/Scroll View/Viewport/Content/BarChart_2").GetComponent<BarChart>();
        _khcountChart = transform.Find("Views/View4/Scroll View/Viewport/Content/BarChart_3").GetComponent<BarChart>();
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
            TeacherTjPanel panel = UIManager.Instance.GetUI<TeacherTjPanel>(EnumUIType.TeacherTjPanel);
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
        string temp = user.Classinfo.Replace("|", "，");
        _classinfoText.text = $"[{temp}]";
        _phonText.text = user.Phonenumber;
        _emallText.text = user.Email;
    }
    void ShowLastLoginTime()
    {
        _lastLoginTimeText.text = AppController.Instance.softData.LoginTime.ToString();
    }
    void ShowOnlineCount()
    {
        _ = NetHelper.Instance.GetSoftStateOnline(AppController.Instance.softData, (count) =>
        {
            if (count < 0)
            {

            }
            else
            {
                //Debug.Log($"在线人数是{count}");
                _onlineCountText.text = count.ToString();
            }
        });
    }

    public override void Init()
    {
        ShowUserInfo();
        ShowLastLoginTime();
        ShowOnlineCount();
        InitLearnDatas();

    }
    public override void OnRefresh()
    {
        ShowUserInfo();
        ShowLastLoginTime();
        ShowOnlineCount();
        UpdateLearnDatas();
    }
    public void EditorUser(User user)
    {
        _ = NetHelper.Instance.UpdateUser(user, EditorUserCallBack);
    }
    private void EditorUserCallBack()
    {
        ShowUserInfo();
        //editorTeacher_Panel.OnHide();
    }
    private void UpdateLearnDatas()
    {
        _ = NetHelper.Instance.GetSoftLearnDatasByClasses(AppController.Instance.softData, (datas) =>
        {
            AppController.Instance.classesLearnDatas = datas;
            ChartHelper.UpdateChart_AllUserCount<CLSType>(datas,_rzcountSerie, _rzcountChart,LearnType.LearnCount, CLSType.None);
            ChartHelper.UpdateChart_AllUserCount<POTSType>(datas, _sxcountSerie, _sxcountChart, LearnType.LearnCount, POTSType.None);
            ChartHelper.UpdateChart_AllUserCount<EVSType>(datas, _khcountSerie, _khcountChart, LearnType.LearnCount, EVSType.None);
        });
    }
    private void InitLearnDatas()
    {
        if (AppController.Instance.classesLearnDatas == null || AppController.Instance.classesLearnDatas.Count <= 0)
        {
            UpdateLearnDatas();
        }
        else
        {
            ChartHelper.UpdateChart_AllUserCount<CLSType>(AppController.Instance.classesLearnDatas,_rzcountSerie, _rzcountChart, LearnType.LearnCount, CLSType.None);
            ChartHelper.UpdateChart_AllUserCount<POTSType>(AppController.Instance.classesLearnDatas,_sxcountSerie, _sxcountChart, LearnType.LearnCount, POTSType.None);
            ChartHelper.UpdateChart_AllUserCount<EVSType>(AppController.Instance.classesLearnDatas, _khcountSerie, _khcountChart, LearnType.LearnCount, EVSType.None);
        }
    }
}
