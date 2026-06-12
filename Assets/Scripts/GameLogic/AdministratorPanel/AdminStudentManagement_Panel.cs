using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdminStudentManagement_Panel : BasePanel
{
    #region 组件
    private Button deleteBtn, bulkimportusersBtn, addBtn;
    private TMP_InputField searchInput;
    private Button searchBtn;
    private Toggle allSelectToggle;
    private Transform itemRooot;
    private StudentInfoItem item;
    [HideInInspector]
    public List<StudentInfoItem> studentList = new List<StudentInfoItem>();
    [HideInInspector]
    public List<StudentInfoItem> currentShowItem = new List<StudentInfoItem>();
    [HideInInspector]
    public List<StudentInfoItem> selectUsers = new List<StudentInfoItem>();
    [HideInInspector]
    public List<StudentInfoItem> filteredUserList = new List<StudentInfoItem>();
    public Bulkimport_Panel bulkimport_Panel;
    public AddStudent_Panel addStudent_Panel;
    public EditorStudent_Panel editorStudent_Panel;
    [HideInInspector]
    private List<User> students = new List<User>();
    private int remainder;
    private int startIndex;
    private int endIndex;
    private PageGrid pageGrid;
    private int opcode = 0; //默认1搜索
    private TMP_Dropdown classDrop;
    #endregion
    public override PanelUIType GetUIType()
    {
        return PanelUIType.AdminStudentManagement_Panel;
    }
    public override void Awake()
    {
        base.Awake();
        deleteBtn = transform.Find("btns/deleteuser").GetComponent<Button>();
        bulkimportusersBtn = transform.Find("btns/bulkimportusers").GetComponent<Button>();
        addBtn = transform.Find("btns/adduser").GetComponent<Button>();
        itemRooot = transform.Find("studentview/data").transform;
        item = transform.Find("studentview/PrefabT/StudentItem").GetComponent<StudentInfoItem>();
        pageGrid = transform.Find("studentview/Page_Info/PageGrid").GetComponent<PageGrid>();
        allSelectToggle = transform.Find("studentview/titlename/Toggle").GetComponent<Toggle>();
        searchInput = transform.Find("studentview/InputField (TMP)").GetComponent<TMP_InputField>();
        searchBtn = transform.Find("studentview/Button").GetComponent<Button>();
        classDrop = transform.Find("studentview/Dropdown").GetComponent<TMP_Dropdown>();
    }
    private void Start()
    {
        MyEventSystem.OnValueChange += UIPageChange;
        deleteBtn.onClick.AddListener(() =>
        {
            if (selectUsers.Count <= 0)
            {
                UIManager.Instance.OpenMessageBoxUI("提示信息", "请选择要删除的用户。", 0, EnumMessageBoxType.OK);
                return;
            }
            UIManager.Instance.OpenMessageBoxUI("提示信息", "确认删除所选用户？", 0, EnumMessageBoxType.OKCancel, DeleteSelectUsers);
        });
        bulkimportusersBtn.onClick.AddListener(() =>
        {
            bulkimport_Panel.OnShow();
        });
        addBtn.onClick.AddListener(() =>
        {
            addStudent_Panel.OnShow();
        });
        allSelectToggle.onValueChanged.AddListener(isOn =>
        {
            for (int i = 0; i < currentShowItem.Count; i++)
            {
                currentShowItem[i].SetState(isOn);
            }
        });
        searchBtn.onClick.AddListener(() =>
        {
            SearchUsers();
        });
        searchInput.onEndEdit.AddListener(value =>
        {
            SearchUsers();
        });
        classDrop.onValueChanged.AddListener(index =>
        {       
            SearchUsers();
        });

    }
    public void SearchUsers()
    {
        string selectedClassName = classDrop.options[classDrop.value].text;
        string searchKeyword = searchInput.text.ToLower();
        bool isClassNameFilterDisabled = classDrop.value == 0;
        filteredUserList = studentList.Where(a =>        
           (isClassNameFilterDisabled || a.classname == selectedClassName) &&
           (string.IsNullOrEmpty(searchKeyword) ||
            a.username.ToLower().Contains(searchKeyword) ||
            a.account.ToLower().Contains(searchKeyword))
       ).ToList();
        if (filteredUserList.Count > 0 && filteredUserList.Count == studentList.Count)
        {
            for (int i = 0; i < studentList.Count; i++)
            {
                studentList[i].gameObject.SetActive(false);
            }
            oldValue = -1;
            opcode = 0;
            pageGrid.Reset(true);
            pageGrid.Init(studentList.Count);
            UIPageChange(1);
        }
        else if (filteredUserList.Count > 0 && filteredUserList.Count != studentList.Count)
        {
            for (int i = 0; i < studentList.Count; i++)
            {
                studentList[i].gameObject.SetActive(false);
            }
            oldValue = -1;
            opcode = 1;
            pageGrid.Reset(true);
            pageGrid.Init(filteredUserList.Count);
            UIPageChange(1);
        }
        else
        {
            for (int i = 0; i < studentList.Count; i++)
            {
                studentList[i].gameObject.SetActive(false);
            }
            pageGrid.Reset(false);
        }
    }

    public override void Init()
    {
        base.Init();
        if (AppController.Instance.users.Count <= 0)
        {
            _ = NetHelper.Instance.GetUsers(AppController.Instance.loginUser.School_name, CallBack);
        }
        else
        {
            if (itemRooot.childCount <= 0)
            {
                CallBack();
            }
        }
    }
    private void CallBack()
    {
        SpawnItem();
    }
    public void SpawnItem()
    {
        List<string> options = new List<string>();
        if (itemRooot.childCount > 0)
        {
            DeleteChildren(itemRooot);
        }
        studentList.Clear();
        opcode = 0;
        oldValue = -1;
        List<User> students = AppController.Instance.users.FindAll(a => a.Usertype == 3);
        if (students.Count<=0) {
            pageGrid.Reset(true);
            pageGrid.Init(0);         
            options.Add("暂无班级");
            classDrop.ClearOptions();
            classDrop.AddOptions(options);
            UIManager.Instance.OpenMessageBoxUI("提示信息", "暂无数据！", 0, EnumMessageBoxType.OK);
            return;
        }
        students.Sort(new NumericStringComparer());
        for (int i = 0; i < students.Count; i++)
        {
            GameObject obj = Instantiate(item.gameObject, itemRooot);
            obj.GetComponent<StudentInfoItem>().OnInit(i+1, students[i]);
            studentList.Add(obj.GetComponent<StudentInfoItem>());
            obj.SetActive(false);
        }
        List<StudentInfoItem> classes = studentList.GroupBy(p => p.classname)
                                             .Select(g => g.First())
                                             .ToList();  
        if (classes.Count <= 0)
        {
            options.Add("暂无班级");
        }
        else
        {
            options.Add("班级筛选");
        }
        for (int i = 0; i < classes.Count; i++)
        {
            options.Add(classes[i].classname);
        }
        classDrop.ClearOptions();
        classDrop.AddOptions(options);
        pageGrid.Reset(true);
        pageGrid.Init(students.Count);
        UIPageChange(1);
    }
    public void AddUsers(DataSet dataSet)
    {
        if (dataSet != null && dataSet.Tables[0].Rows.Count > 1)
        {
            students.Clear();
            for (int i = 1; i < dataSet.Tables[0].Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(dataSet.Tables[0].Rows[i][1].ToString().Trim()))
                {
                    break;
                }
                User user = new User
                {
                    Number=int.Parse(dataSet.Tables[0].Rows[i][0].ToString()),
                    School_name = dataSet.Tables[0].Rows[i][1].ToString(),
                    Username = dataSet.Tables[0].Rows[i][2].ToString(),
                    Sex = dataSet.Tables[0].Rows[i][3].ToString() == "女" ? 0 : 1,
                    Account = dataSet.Tables[0].Rows[i][4].ToString(),
                    Password = dataSet.Tables[0].Rows[i][5].ToString(),
                    Classinfo = dataSet.Tables[0].Rows[i][6].ToString(),
                    Position = dataSet.Tables[0].Rows[i][7].ToString(),
                    Phonenumber = dataSet.Tables[0].Rows[i][8].ToString(),
                    Email = dataSet.Tables[0].Rows[i][9].ToString(),
                    Usertype = 3
                };
                students.Add(user);
                bool hasDuplicates = students.GroupBy(x => x.Account)
                                  .Any(g => g.Count() > 1);
                if (hasDuplicates)
                {
                    UIManager.Instance.OpenMessageBoxUI("提示信息", "有重复的用户账号，请修改。", 0, EnumMessageBoxType.OK);
                    return;
                }
            }
        }
        else
        {
            UIManager.Instance.OpenMessageBoxUI("提示信息", "无数据导入！", 0, EnumMessageBoxType.OK);
            return;
        }
        _ = NetHelper.Instance.AddUsers(students, AddUserCallBack);
    }
    private void AddUserCallBack()
    {
        OnRefresh();
        if (bulkimport_Panel.gameObject.activeSelf)
        {
            bulkimport_Panel.OnHide();
        }
        if (addStudent_Panel.gameObject.activeSelf)
        {
            addStudent_Panel.OnHide();
        }
    }
    public void AddUser(List<User> user)
    {
        _ = NetHelper.Instance.AddUsers(user, AddUserCallBack);
    }
    public void EditorUser(User user)
    {
        _ = NetHelper.Instance.UpdateUser(user, EditorUserCallBack);
    }
    private void EditorUserCallBack()
    {
        OnRefresh();
        editorStudent_Panel.OnHide();
    }
    private void DeleteSelectUsers(object args)
    {
        List<ExamResult> userExamReses = new List<ExamResult>();
        List<User> deleteusers = new List<User>();
        for (int i = 0; i < selectUsers.Count; i++)
        {
            User user = new User
            {
                School_name = selectUsers[i].schoolname,
                Account = selectUsers[i].account
            };
            ExamResult resultdata = new ExamResult
            {
                School_name = selectUsers[i].schoolname,
                Account = selectUsers[i].account
            };
            deleteusers.Add(user);
            userExamReses.Add(resultdata);
        }
        searchInput.text = "";
        _ = NetHelper.Instance.DeleteUsers(deleteusers, () =>
        {

            _ = NetHelper.Instance.DeleteExamResults(userExamReses, () =>
            {
                //删除学习进度
                _ = NetHelper.Instance.DeleteStudentsLearnDatas(deleteusers, code =>
                {
                    //删除任务成绩
                    _ = NetHelper.Instance.DeleteTaskDataByUsers(deleteusers, code =>
                    {
                        OnRefresh();
                    });
                });
            });
        });
    }
    private void UIPageChange(int value)
    {
        if (opcode == 0)
        {
            PageChange(value, studentList);
        }
        else
        {
            PageChange(value, filteredUserList);
        }

    }
    int oldValue = -1;
    private void PageChange(int value, List<StudentInfoItem> datas)
    {
        if (oldValue == value)
        {
            return;
        }
        allSelectToggle.isOn = false;
        remainder = datas.Count % pageGrid.rowCount;
        startIndex = value * pageGrid.rowCount - pageGrid.rowCount;
        endIndex = value * pageGrid.rowCount - 1;
        if (endIndex > datas.Count - 1)
        {
            endIndex = startIndex + remainder - 1;
        }
        currentShowItem.Clear();
        for (int i = 0; i < datas.Count; i++)
        {
            if (i >= startIndex && i <= endIndex)
            {
                datas[i].gameObject.SetActive(true);
                currentShowItem.Add(datas[i]);
            }
            else
            {
                datas[i].gameObject.SetActive(false);
            }
        }
        oldValue = value;
    }
    public override void OnShow()
    {
        base.OnShow();  
        MyEventSystem.ClearAll();
        MyEventSystem.OnValueChange += UIPageChange;    
    }
    public override void OnHide()
    {
        MyEventSystem.OnValueChange -= UIPageChange;    
        base.OnHide();
    }
    public override void OnRefresh()
    {
        base.OnRefresh();
        _ = NetHelper.Instance.GetUsers(AppController.Instance.loginUser.School_name, CallBack);      
    }
    private void OnDisable()
    {
        MyEventSystem.OnValueChange -= UIPageChange;   
    }
}
