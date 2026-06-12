using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TeacherInfotItem : MonoBehaviour
{
    private Toggle selectToggle;
    public string schoolname { get; set; }
    public string username { get; set; }
    public string account { get; set; }
    public User _user { get; set; }
    private TMP_Text numberText, schoolnameText, usernameText, accountText, classnameText, registertimeText, positionText;
    private Button handleButton;
    PanelUIType panelnumber;
    AdminTeacherManagement_Panel panel;
    private void Awake()
    {
        panelnumber = PanelUIType.AdminTeacherManagement_Panel;
        selectToggle = transform.Find("Toggle").GetComponent<Toggle>();
        numberText = transform.Find("number").GetComponent<TMP_Text>();
        schoolnameText = transform.Find("schoolname").GetComponent<TMP_Text>();
        usernameText = transform.Find("username").GetComponent<TMP_Text>();
        accountText = transform.Find("account").GetComponent<TMP_Text>();
        classnameText = transform.Find("class").GetComponent<TMP_Text>();
        registertimeText = transform.Find("registertime").GetComponent<TMP_Text>();
        positionText = transform.Find("position").GetComponent<TMP_Text>();
        handleButton = transform.Find("handleButton").GetComponent<Button>();
    }
    private void Start()
    {
        panel = AdministratorPanel.GetPanel((int)panelnumber) as AdminTeacherManagement_Panel;
        selectToggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                panel.selectTeacher.Add(this);

            }
            else
            {
                panel.selectTeacher.Remove(this);
            }
        });
        handleButton.onClick.AddListener(() =>
        {
            AdminTeacherManagement_Panel panel = AdministratorPanel.GetPanel((int)PanelUIType.AdminTeacherManagement_Panel) as AdminTeacherManagement_Panel;
            panel.editorTeacher_Panel.OnShow();
            panel.editorTeacher_Panel.Init(_user);
        });
    }
    public void OnInit(int index, User data)
    {
        this._user = data;
        this.schoolname = data.School_name;
        this.username = data.Username;
        this.account = data.Account;
        numberText.text = index.ToString();
        schoolnameText.text = data.School_name;
        usernameText.text = data.Username;
        accountText.text = data.Account;
        classnameText.text = data.Classinfo;
        registertimeText.text = data.Registertime;
        positionText.text = data.Position;
    }
    public void SetState(bool _isOn)
    {
        selectToggle.isOn = _isOn;
    }
}
