using TMPro;
using UnityEngine.UI;

public class EditorStudent_Panel : BaseWindowPanel
{
    private TMP_InputField nameInput, schoolnameInput, classinfoInput, accountInput, positionInput, passwordInput, phoneImput, emallinput;
    private Button cancelBtn, okBtn;
    private TMP_Dropdown sexDrop;
    private void Awake()
    {
        nameInput = transform.Find("bg/infos/name/InputField (TMP)").GetComponent<TMP_InputField>();
        schoolnameInput = transform.Find("bg/infos/schoolname/InputField (TMP)").GetComponent<TMP_InputField>();
        classinfoInput = transform.Find("bg/infos/classname/InputField (TMP)").GetComponent<TMP_InputField>();
        accountInput = transform.Find("bg/infos/account/InputField (TMP)").GetComponent<TMP_InputField>();
        positionInput = transform.Find("bg/infos/position/InputField (TMP)").GetComponent<TMP_InputField>();
        passwordInput = transform.Find("bg/infos/password/InputField (TMP)").GetComponent<TMP_InputField>();
        phoneImput = transform.Find("bg/infos/phone/InputField (TMP)").GetComponent<TMP_InputField>();
        emallinput = transform.Find("bg/infos/emall/InputField (TMP)").GetComponent<TMP_InputField>();
        cancelBtn = transform.Find("bg/infos/btns/cancelButton").GetComponent<Button>();
        okBtn = transform.Find("bg/infos/btns/okButton").GetComponent<Button>();
        sexDrop = transform.Find("bg/infos/sex/Dropdown").GetComponent<TMP_Dropdown>();
        if (AppController.Instance.userType==UserType.学生) {
            nameInput.interactable = false; 
            classinfoInput.interactable = false;
            accountInput.interactable=false;
            schoolnameInput.interactable = false;
        }
    }
    private void Start()
    {
        schoolnameInput.text = AppController.Instance.loginUser.School_name;
        cancelBtn.onClick.AddListener(() =>
        {
            OnHide();
        });
        okBtn.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(nameInput.text.Trim()) || string.IsNullOrEmpty(classinfoInput.text.Trim()) || string.IsNullOrEmpty(accountInput.text.Trim())
                || string.IsNullOrEmpty(positionInput.text.Trim()) || string.IsNullOrEmpty(passwordInput.text.Trim()))
            {
                UIManager.Instance.OpenMessageBoxUI("提示信息", "请输入完整用户信息。", 0, EnumMessageBoxType.OK);
                return;
            }
            User user = new User
            {
                Username = nameInput.text.Trim(),
                School_name = schoolnameInput.text.Trim(),
                Classinfo = classinfoInput.text.Trim(),
                Account = accountInput.text.Trim(),
                Position = positionInput.text.Trim(),
                Password = passwordInput.text.Trim(),
                Phonenumber=phoneImput.text.Trim(),
                Email=emallinput.text.Trim(),
                Usertype = 3,
                Sex = sexDrop.value
            };
            if (AppController.Instance.userType == UserType.管理员)
            {
                UIManager.Instance.OpenMessageBoxUI("提示信息", "确认修改？", 0, EnumMessageBoxType.OKCancel, OnUpdateUser, user);
            }
            if (AppController.Instance.userType == UserType.学生)
            {
                UIManager.Instance.OpenMessageBoxUI("提示信息", "确认修改？修改后需重新登录。", 0, EnumMessageBoxType.OKCancel, OnStudentUpateUser, user);
            }
        });
    }

    private void OnUpdateUser(object args)
    {
        User user = args as User;
        AdminStudentManagement_Panel panel_admin = AdministratorPanel.GetPanel((int)PanelUIType.AdminStudentManagement_Panel) as AdminStudentManagement_Panel;
        panel_admin.EditorUser(user);
    }
    private void OnStudentUpateUser(object args)
    {
        User user = args as User;
        _ = NetHelper.Instance.UpdateUser(user, EditorUserCallBack);
    }
    private void EditorUserCallBack()
    {
       StudentTjPanel panel= UIManager.Instance.GetUI<StudentTjPanel>(EnumUIType.StudentTjPanel);
       panel.ResetLogin();
    }
    public void Init(User user)
    {
        nameInput.text = user.Username;
        schoolnameInput.text = user.School_name;
        classinfoInput.text = user.Classinfo;
        accountInput.text = user.Account;
        positionInput.text = user.Position;
        passwordInput.text = user.Password;
        phoneImput.text=user.Phonenumber;
        emallinput.text = user.Email;
        sexDrop.value = user.Sex;
    }
    public void Reset()
    {
        nameInput.text = "";
        schoolnameInput.text = "";
        classinfoInput.text = "";
        accountInput.text = "";
        positionInput.text = "";
        passwordInput.text = "";
        phoneImput.text = "";
        emallinput.text = "";
        sexDrop.value = 0;
    }
    public override void OnHide()
    {
        Reset();
        base.OnHide();
    }
}
