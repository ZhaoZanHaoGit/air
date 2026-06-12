using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Xml.Linq;
using System.Runtime.CompilerServices;

public class LoginManager : MonoBehaviour
{
    public static ExamUserData user = new ExamUserData();

    public TMP_InputField input_ID, input_department, input_name, input_phonenumber;
    public Button loginButton;
    public GameObject loginPanel, logfinishpanel, examPanel;
    public static bool islogin = false;
    public TextMeshProUGUI logError;
    private void Start()
    {

        loginButton.onClick.AddListener(() =>
        {
            user.id = input_ID.text;
            user.department = input_department.text;
            user.name = input_name.text;
            user.phonenumber = input_phonenumber.text;
            if (user.id == "" || user.department == "" || user.name == "" || user.phonenumber == "")
            { logError.text = "请将登陆信息填写完整"; }
            else
            {
                logError.text = "";
                 islogin = true;
            }
           
            changelogstate();


        });
    }
    public void changelogstate()
    {
        if (islogin)

        {
            logfinishpanel.SetActive(true);
        }
        else { logfinishpanel.SetActive(false); }

    }
    public void logout()
    {
        islogin = false;
        user.id = "";
        user.department = "";
        user.name = "";
        user.phonenumber = "";
    }
    public void StartExam()
    {
        if (islogin)
        {
            loginPanel.gameObject.SetActive(false);

            examPanel.gameObject.SetActive(true);
        }

    }
}
public class ExamUserData
{
    public string id;
    public string department;
    public string name;
    public string phonenumber;
    public string Date;
    public string Score;

}
