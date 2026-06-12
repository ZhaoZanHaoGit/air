using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdminTeacherManagement_Panel : BasePanel
{
    #region 组件
    private Button deleteBtn, bulkimportusersBtn, addBtn;
    private TMP_InputField searchInput;
    private Button searchBtn;
    private Toggle allSelectToggle;
    private Transform itemRooot;
    private TeacherInfotItem item;
    [HideInInspector]
    public List<TeacherInfotItem> teacherList = new List<TeacherInfotItem>();
    [HideInInspector]
    public List<TeacherInfotItem> currentShowItem = new List<TeacherInfotItem>();
    [HideInInspector]
    public List<TeacherInfotItem> selectTeacher = new List<TeacherInfotItem>();
    [HideInInspector]
    public List<TeacherInfotItem> filteredUserList = new List<TeacherInfotItem>();
    public Bulkimport_Panel bulkimport_Panel;
    public AddTeacher_Panel addTeacher_Panel;
    public EditorTeacher_Panel editorTeacher_Panel;
    [HideInInspector]
    private List<User> teachers = new List<User>();
    private int remainder;
    private int startIndex;
    private int endIndex;
    private PageGrid pageGrid;
    private int opcode = 0; //默认1搜索
    #endregion
    public override void Awake()
    {
        base.Awake();
        deleteBtn = transform.Find("btns/deleteuser").GetComponent<Button>();
        bulkimportusersBtn = transform.Find("btns/bulkimportusers").GetComponent<Button>();
        addBtn = transform.Find("btns/adduser").GetComponent<Button>();
        itemRooot = transform.Find("teacherview/data").transform;
        item = transform.Find("teacherview/PrefabT/TeacherInfotItem").GetComponent<TeacherInfotItem>();
        pageGrid = transform.Find("teacherview/Page_Info/PageGrid").GetComponent<PageGrid>();
        allSelectToggle = transform.Find("teacherview/titlename/Toggle").GetComponent<Toggle>();
        searchInput = transform.Find("teacherview/InputField (TMP)").GetComponent<TMP_InputField>();
        searchBtn = transform.Find("teacherview/Button").GetComponent<Button>();
    }

    private void UIPageChange(int value)
    {
        if (opcode == 0)
        {
            PageChange(value, teacherList);
        }
        else
        {
            PageChange(value, filteredUserList);
        }

    }
    int oldValue = -1;
    private void PageChange(int value, List<TeacherInfotItem> datas)
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

    private void OnDisable()
    {
        MyEventSystem.OnValueChange -= UIPageChange;
    }
    private void Start()
    {

        MyEventSystem.OnValueChange += UIPageChange;
        deleteBtn.onClick.AddListener(() =>
         {
             if (selectTeacher.Count <= 0)
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
            addTeacher_Panel.OnShow();
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
            SearchUsers(searchInput.text.Trim());
        });
        searchInput.onEndEdit.AddListener(value =>
        {
            SearchUsers(value);
        });
    }

    private void DeleteSelectUsers(object args)
    {
        List<PresetCourseData> psdatas = new List<PresetCourseData>();
        List<User> deleteusers = new List<User>();
        for (int i = 0; i < selectTeacher.Count; i++)
        {
            User user = new User
            {
                School_name = selectTeacher[i].schoolname,
                Account = selectTeacher[i].account
            };
            PresetCourseData data = new PresetCourseData
            {
                School_name = selectTeacher[i].schoolname,
                TeacherAccount = selectTeacher[i].account
            };
            deleteusers.Add(user);
            psdatas.Add(data);
        }
        searchInput.text = "";
        _ = NetHelper.Instance.DeleteUsers(deleteusers, () =>
        {
            _ = NetHelper.Instance.DeletePresetCourseDatas(psdatas, () =>
            {
                _ = NetHelper.Instance.DeleteCurrentAllExamPapers(deleteusers, () =>
                {
                    _ = NetHelper.Instance.DeleteCurrentAllQuestions(deleteusers, () =>
                    {
                        OnRefresh();
                    });
                });              
            });
        });
    }

    public void SearchUsers(string searchKeyword)
    {
        filteredUserList = teacherList.Where(user => user.username.Contains(searchKeyword) || user.account.Contains(searchKeyword)).ToList();
        if (filteredUserList.Count > 0 && filteredUserList.Count == teacherList.Count)
        {
            for (int i = 0; i < teacherList.Count; i++)
            {
                teacherList[i].gameObject.SetActive(false);
            }
            oldValue = -1;
            opcode = 0;
            pageGrid.Reset(true);
            pageGrid.Init(teacherList.Count);
            UIPageChange(1);
        }
        else if (filteredUserList.Count > 0 && filteredUserList.Count != teacherList.Count)
        {
            for (int i = 0; i < teacherList.Count; i++)
            {
                teacherList[i].gameObject.SetActive(false);
            }
            oldValue = -1;
            opcode = 1;
            pageGrid.Reset(true);
            pageGrid.Init(filteredUserList.Count);
            UIPageChange(1);
        }
        else
        {
            for (int i = 0; i < teacherList.Count; i++)
            {
                teacherList[i].gameObject.SetActive(false);
            }
            pageGrid.Reset(false);
        }
    }
    //默认数据
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
    public override PanelUIType GetUIType()
    {
        return PanelUIType.AdminTeacherManagement_Panel;
    }
    public void SpawnItem()
    {
        if (itemRooot.childCount > 0)
        {
            DeleteChildren(itemRooot);
        }
        teacherList.Clear();
        opcode = 0;
        oldValue = -1;
        List<User> teachers = AppController.Instance.users.FindAll(a => a.Usertype == 2);
        if (teachers.Count <= 0)
        {
            pageGrid.Reset(true);
            pageGrid.Init(0);
            UIManager.Instance.OpenMessageBoxUI("提示信息", "暂无数据！", 0, EnumMessageBoxType.OK);
            return;
        }   
        teachers.Sort(new NumericStringComparer());
        for (int i = 0; i < teachers.Count; i++)
        {
            GameObject obj = Instantiate(item.gameObject, itemRooot);
            obj.GetComponent<TeacherInfotItem>().OnInit(i+1, teachers[i]);
            teacherList.Add(obj.GetComponent<TeacherInfotItem>());
            obj.SetActive(false);
        }     
        pageGrid.Reset(true);
        pageGrid.Init(teachers.Count);
        UIPageChange(1);
    }

    public void AddUsers(DataSet dataSet)
    {
        if (dataSet != null && dataSet.Tables[0].Rows.Count > 1)
        {
            teachers.Clear();
            for (int i = 1; i < dataSet.Tables[0].Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(dataSet.Tables[0].Rows[i][1].ToString().Trim()))
                {
                    break;
                }
                User user = new User
                {
                    Number = int.Parse(dataSet.Tables[0].Rows[i][0].ToString()),
                    School_name = dataSet.Tables[0].Rows[i][1].ToString(),
                    Username = dataSet.Tables[0].Rows[i][2].ToString(),
                    Sex = dataSet.Tables[0].Rows[i][3].ToString() == "女" ? 0 : 1,
                    Account = dataSet.Tables[0].Rows[i][4].ToString(),
                    Password = dataSet.Tables[0].Rows[i][5].ToString(),
                    Classinfo = dataSet.Tables[0].Rows[i][6].ToString(),
                    Position = dataSet.Tables[0].Rows[i][7].ToString(),
                    Phonenumber = dataSet.Tables[0].Rows[i][8].ToString(),
                    Email = dataSet.Tables[0].Rows[i][9].ToString(),
                    Usertype = 2
                };
                teachers.Add(user);
                bool hasDuplicates = teachers.GroupBy(x => x.Account)
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
        _ = NetHelper.Instance.AddUsers(teachers, AddUserCallBack);
    }

    private void AddUserCallBack()
    {
        OnRefresh();
        if (bulkimport_Panel.gameObject.activeSelf)
        {
            bulkimport_Panel.OnHide();
        }
        if (addTeacher_Panel.gameObject.activeSelf)
        {
            addTeacher_Panel.OnHide();
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
        editorTeacher_Panel.OnHide();
    }

    public override void OnShow()
    {
        base.OnShow();     
        MyEventSystem.ClearAll();
        MyEventSystem.OnValueChange += UIPageChange;
    }
    public override void OnHide()
    {
        base.OnHide();
        MyEventSystem.OnValueChange -= UIPageChange;
    }
    public override void OnRefresh()
    {
        base.OnRefresh();
        _ = NetHelper.Instance.GetUsers(AppController.Instance.loginUser.School_name, CallBack);
    }
}
