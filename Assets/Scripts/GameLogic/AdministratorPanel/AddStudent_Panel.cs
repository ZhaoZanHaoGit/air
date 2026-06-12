using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class AddStudent_Panel : BaseWindowPanel
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
    }
    private void Start()
    {
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
            List<User> data = new List<User>();
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
            data.Add(user);
            AdminStudentManagement_Panel panel_admin = AdministratorPanel.GetPanel((int)PanelUIType.AdminStudentManagement_Panel) as AdminStudentManagement_Panel;
            panel_admin.AddUser(data);
        });
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
    public override void OnShow()
    {
        base.OnShow();
        schoolnameInput.text = AppController.Instance.loginUser.School_name;
    }
    public override void OnHide()
    {
        Reset();
        base.OnHide();
    }
}
