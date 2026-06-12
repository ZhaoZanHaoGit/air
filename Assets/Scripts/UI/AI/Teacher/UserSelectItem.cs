using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserSelectItem : MonoBehaviour
{
    public User user { get; set; }
    private TMP_Text accountText, usernameText;
    private Toggle selectToggle;
    private SelectUser _selectUser;

    // 新增：是否忽略 Toggle 变化事件
    private bool suppressToggleCallback = false;
    private void Awake()
    {
        accountText = transform.Find("account").GetComponent<TMP_Text>();
        usernameText = transform.Find("username").GetComponent<TMP_Text>();
        selectToggle=transform.Find("Toggle").GetComponent<Toggle>();
    }
    private void Start()
    {
        selectToggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (suppressToggleCallback)
            return; // 跳过回调
        if (isOn)
        {
            _selectUser.Add(this);
        }
        else {
            _selectUser.Remove(this);
        }
    }
    public void InitData(SelectUser selectUser,User user) {
        _selectUser = selectUser;
        this.user = user;
        accountText.text = user.Account;
        usernameText.text = user.Username;
    }
    //public void SetSelectState(bool isOn)
    //{
    //    selectToggle.isOn = isOn;
    //}
    public void SetSelectState(bool isOn, bool suppressCallback = false)
    {
        suppressToggleCallback = suppressCallback;
        selectToggle.isOn = isOn;
        suppressToggleCallback = false;
    }
}
