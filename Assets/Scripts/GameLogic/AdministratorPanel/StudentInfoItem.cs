using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StudentInfoItem : MonoBehaviour
{
    private Toggle selectToggle;
    public string schoolname { get; set; }
    public string username { get; set; }
    public string account { get; set; }
    public string classname { get; set; }
    public string position { get; set; }
    public User _user { get; set; }
    private TMP_Text numberText, schoolnameText, usernameText, accountText, classnameText, registertimeText, positionText;
    private Button handleButton;
    PanelUIType panelnumber;

    private void Awake()
    {     
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
        switch (AppController.Instance.loginUser.Usertype)
        {
            case (int)UserType.管理员:
                panelnumber = PanelUIType.AdminStudentManagement_Panel;
                AdminStudentManagement_Panel panel = AdministratorPanel.GetPanel((int)panelnumber) as AdminStudentManagement_Panel;
                selectToggle.onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        panel.selectUsers.Add(this);

                    }
                    else
                    {
                        panel.selectUsers.Remove(this);
                    }
                });
                handleButton.onClick.AddListener(() =>
                {
                    AdminStudentManagement_Panel panel = AdministratorPanel.GetPanel((int)PanelUIType.AdminStudentManagement_Panel) as AdminStudentManagement_Panel;
                    panel.editorStudent_Panel.OnShow();
                    panel.editorStudent_Panel.Init(_user);
                });
                break;
            case (int)UserType.教师:
                //panelnumber = PanelUIType.StudentManagement_Panel;
                //StudentManagement_Panel panel2 = TeacherPanel.GetPanel((int)panelnumber) as StudentManagement_Panel;
                selectToggle.onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        //panel2.selectUsers.Add(this);

                    }
                    else
                    {
                        //panel2.selectUsers.Remove(this);
                    }
                });
                handleButton.onClick.AddListener(() =>
                {
                    //StudentManagement_Panel panel = TeacherPanel.GetPanel((int)PanelUIType.StudentManagement_Panel) as StudentManagement_Panel;
                    //panel.editorStudent_Panel.OnShow();
                    //panel.editorStudent_Panel.Init(_user);
                });
                break;
        }
      
    }
    public void OnInit(int index, User data)
    {
        this._user = data;
        this.schoolname = data.School_name;
        this.username = data.Username;
        this.account = data.Account;
        this.classname = data.Classinfo;
        this.position = data.Position;
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
