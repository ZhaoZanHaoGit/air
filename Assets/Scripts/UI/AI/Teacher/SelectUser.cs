using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SelectUser : MonoBehaviour
{
    public AddTaskPanel panel;
    private TMP_Dropdown selectClassDrop;
    private Toggle allSelectUsersToggle;
    private Button cancelBtn, addUsersBtn;
    private List<UserSelectItem> selectUserSelectItems = new List<UserSelectItem>();
    private List<UserSelectItem> currentallUsers = new List<UserSelectItem>();
    private string _currentSelectClass;
    private Transform usermodelRoot;
    private GameObject userModelItemPrefab;

    private void Awake()
    {
        selectClassDrop = transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
        allSelectUsersToggle = transform.Find("Connect/1/Toggle").GetComponent<Toggle>();
        cancelBtn = transform.Find("cancel").GetComponent<Button>();
        addUsersBtn = transform.Find("add").GetComponent<Button>();
        usermodelRoot = transform.Find("Connect/Scroll View/Viewport/Content");
        userModelItemPrefab = transform.Find("PrefabT/selectUserItem").gameObject;

    }
    private void Start()
    {
      
        GetClassDropValue(selectClassDrop);
        selectClassDrop.onValueChanged.AddListener((value) =>
        {
            _currentSelectClass = selectClassDrop.options[value].text;
            List<User> datas = AppController.Instance.currentStudents.FindAll(a => a.Classinfo == _currentSelectClass);
            OnShowClassStudents(datas);
        });
        cancelBtn.onClick.AddListener(() =>
        {
            Hide();
        });
        addUsersBtn.onClick.AddListener(() =>
        {
            if (selectUserSelectItems.Count<=0) {
                UIManager.Instance.OpenMessageBoxUI("提示信息", "学员不能为空！", 0, EnumMessageBoxType.OK);
                return;
            }
            panel.AddUserModelsList(selectUserSelectItems);
            Hide();
        });
        allSelectUsersToggle.onValueChanged.AddListener((isOn) =>
        {
            // 使用副本防止遍历时被修改
            var snapshot = new List<UserSelectItem>(currentallUsers);
            foreach (var item in snapshot)
            {
                item.SetSelectState(isOn, suppressCallback: false);
            }
        });
        SpawnUsers();
    }
    private void GetClassDropValue(TMP_Dropdown drop) {
        string classes = AppController.Instance.loginUser.Classinfo;
        if (!string.IsNullOrEmpty(classes))
        {
            string[] classArray = classes.Split('|');
            drop.ClearOptions();
            List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < classArray.Length; i++)
            {
                TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData
                {
                    text = classArray[i]
                };
                optionDatas.Add(data);
            }
            drop.AddOptions(optionDatas);
            _currentSelectClass = classArray[0];
        }
    }
    public void Add(UserSelectItem item)
    {
        if (selectUserSelectItems.Find(a => a.user.Account == item.user.Account)) return;
        selectUserSelectItems.Add(item);
    }

    public void SpawnUsers() {
        if (AppController.Instance.currentStudents.Count > 0)
        {
            if (usermodelRoot.childCount <= 0)
            {
                List<User> users = AppController.Instance.currentStudents.FindAll(a => a.Classinfo == _currentSelectClass);
                SpawnUser(users);
            }
        }
        else {
            _ = NetHelper.Instance.GetUserByClasses(AppController.Instance.softData, (datas) =>
            {
                AppController.Instance.currentStudents = datas;
                if (AppController.Instance.currentStudents.Count > 0)
                {
                
                    if (usermodelRoot.childCount <= 0)
                    {
                        List<User> users = AppController.Instance.currentStudents.FindAll(a => a.Classinfo == _currentSelectClass);
                        SpawnUser(users);
                    }
                }
            });
         }
    }
    void OnShowClassStudents(List<User> datas) {
        if (datas.Count <= 0) return;
        if (usermodelRoot.childCount>0) {
            for (int i = usermodelRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(usermodelRoot.GetChild(i).gameObject);
            }
            SpawnUser(datas);
        }
    }


    void SpawnUser(List<User> datas) {
        currentallUsers.Clear();
        for (int i = 0; i < datas.Count; i++)
        {
            GameObject go = Instantiate(userModelItemPrefab, usermodelRoot);
            UserSelectItem item = go.GetComponent<UserSelectItem>();
            item.InitData(this, datas[i]);
            currentallUsers.Add(item);
        }
    }
    public void Remove(UserSelectItem item)
    {
        selectUserSelectItems.Remove(item);
    }
    public void Show()
    {
        this.gameObject.SetActive(true);
    }
    public void Hide()
    {
        if (selectUserSelectItems.Count > 0)
        {
            foreach (var item in selectUserSelectItems)
            {
                item.SetSelectState(false, suppressCallback:true);
            }
        }
        allSelectUsersToggle.isOn = false;
        selectUserSelectItems.Clear();
        this.gameObject.SetActive(false);
    }
}
