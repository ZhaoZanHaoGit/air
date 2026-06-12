using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AIDigitalHuman_Panel : BasePanel
{
    public override PanelUIType GetUIType()
    {
        return PanelUIType.AIDigitalHuman_Panel;
    }
    private Button digitalHumanEditBtn;
    private Button addDigitalHumanBtn;
    public Transform itemRooot;
    public GameObject digitalHumanItem;
    private List<DigitalHumanItem> itemlist = new List<DigitalHumanItem>();
    public AddDigitalHumanData_Panel addDigitalHumanData_Panel;
    public EditDigitalHumanData_Panel editDigitalHumanData_Panel;
    public override void Awake()
    {
        base.Awake();
        digitalHumanEditBtn = transform.Find("digitalHumanEditBtn").GetComponent<Button>();
        addDigitalHumanBtn = transform.Find("scoreview/addDigitalHumanBtn").GetComponent<Button>();

    }
    private void Start()
    {
        digitalHumanEditBtn.onClick.AddListener(() =>
        {
            string url = AppController.Instance.pathDatas.Find(a => a.ID == 1).Path;
            Application.OpenURL(url);
        });
        addDigitalHumanBtn.onClick.AddListener(() =>
        {
            addDigitalHumanData_Panel.OnShow();
        });

    }
    public override void Init()
    {
        base.Init();
        if (AppController.Instance.currentDigitalHumanDatas != null && AppController.Instance.currentDigitalHumanDatas.Count > 0)
        {
            SpawnItem(AppController.Instance.currentDigitalHumanDatas);
        }
        else
        {
            GetData();
        }
    }
    private void GetData()
    {
        OnRefresh();
    }
    public void AddData(DigitalHumanData data)
    {
        _ = NetHelper.Instance.AddDigitalHumanData(data, code =>
        {
            //Debug.Log("添加数据是否成功" + code);
            if (code == 0)
            {
                addDigitalHumanData_Panel.OnHide();
                OnRefresh();
            }
            else
            {
                UIManager.Instance.OpenMessageBoxUI("错误信息", "添加数字人失败！", 0, EnumMessageBoxType.OK);
            }

        });
    }
    public void DeleteData(string id)
    {
        UIManager.Instance.OpenMessageBoxUI("提示信息", "确认删除该数字人？", 0, EnumMessageBoxType.OKCancel, Delete, id, null, null, null);
    }
    private void Delete(object args)
    {
        string id = (string)args;
        _ = NetHelper.Instance.DeleteDigitalHumanData(id, code =>
        {
            if (code == 0)
            {
                OnRefresh();
            }
            else
            {
                UIManager.Instance.OpenMessageBoxUI("错误信息", "删除数字人失败！", 0, EnumMessageBoxType.OK);
            }
        });
    }
    public void EditData(DigitalHumanData data) {
        editDigitalHumanData_Panel.Init(data);
    }
    public void UpdateData(DigitalHumanData data)
    {
        _ = NetHelper.Instance.UpdateDigitalHumanData(data, code =>
        {
            //Debug.Log("更新数据是否成功" + code);
            if (code == 0)
            {
                editDigitalHumanData_Panel.OnHide();
                OnRefresh();
            }
            else
            {
                UIManager.Instance.OpenMessageBoxUI("错误信息", "更新数字人失败！", 0, EnumMessageBoxType.OK);
            }
        });
    }


    public override void OnRefresh()
    {
        string account = AppController.Instance.loginUser.Account;
        _ = NetHelper.Instance.GetDigitalHumanData_Teacher(account, (datas) =>
        {
            AppController.Instance.currentDigitalHumanDatas = datas;
            if (datas != null && datas.Count > 0)
            {
                SpawnItem(datas);
            }
            else {
                DeleteChildren(itemRooot);
            }
        });
    }

    private void SpawnItem(List<DigitalHumanData> datas)
    {
        DeleteChildren(itemRooot);
        itemlist.Clear();
        //oldValue = -1;
        if (datas.Count <= 0)    
            return;
        itemlist = datas.Select((data, index) =>
        {
            GameObject obj = Instantiate(digitalHumanItem, itemRooot);
            DigitalHumanItem tempitem = obj.GetComponent<DigitalHumanItem>();
            tempitem.Init(index + 1, data);
            return tempitem;
        }).ToList();
    }
    
    public override void OnShow()
    {
        base.OnShow();
    }
    public override void OnHide()
    {
        base.OnHide();
    }
}
