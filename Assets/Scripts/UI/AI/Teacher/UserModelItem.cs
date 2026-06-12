using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserModelItem : MonoBehaviour
{
    public User user { get; set; }
    private TMP_Text className, usernameText;
    private Button deleteBtn;
    private AddTaskPanel _panel;
    private void Awake()
    {
        className=transform.Find("classname").GetComponent<TMP_Text>();
        usernameText = transform.Find("username").GetComponent<TMP_Text>();
        deleteBtn = transform.Find("Button").GetComponent<Button>();
    }
    private void Start()
    {
        deleteBtn.onClick.AddListener(OnDeleteBtnClick);
    }
    public void InitData(AddTaskPanel panl,User _user) {
        user = _user;
        _panel = panl;
        className.text = user.Classinfo;
        usernameText.text = user.Username;
    }
    private void OnDeleteBtnClick()
    {
        _panel.DeleteUserModel(this);
    }
}
