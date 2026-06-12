using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class AddDigitalHumanData_Panel : BaseWindowPanel
{
    private TMP_InputField nameInput, urlInput, keyInput;
    private TMP_Dropdown classDrop;
    private Toggle isOpen;
    private Button cancelBtn, saveBtn;
    private DigitalHumanData data;
    private AIDigitalHuman_Panel aIDigitalHuman_Panel { get { return TeacherTjPanel.GetPanel((int)PanelUIType.AIDigitalHuman_Panel) as AIDigitalHuman_Panel; } }
    private void Awake()
    {
        nameInput = transform.Find("bg/infos/name/InputField (TMP)").GetComponent<TMP_InputField>();
        urlInput = transform.Find("bg/infos/url/InputField (TMP)").GetComponent<TMP_InputField>();
        keyInput = transform.Find("bg/infos/key/InputField (TMP)").GetComponent<TMP_InputField>();
        classDrop = transform.Find("bg/infos/classinfo/Dropdown").GetComponent<TMP_Dropdown>();
        isOpen = transform.Find("bg/infos/OnState/Toggle").GetComponent<Toggle>();
        cancelBtn = transform.Find("bg/infos/btns/cancelButton").GetComponent<Button>();
        saveBtn = transform.Find("bg/infos/btns/okButton").GetComponent<Button>();
    }
    private void Start()
    {
        GetClassNmasDrop();
        cancelBtn.onClick.AddListener(() =>
        {
            OnHide();
        });
        saveBtn.onClick.AddListener(() =>
        {
            Save();
        });
        isOpen.onValueChanged.AddListener(isOn =>
        {
            if (data!=null)
            {
                data.OnState = isOn ? 1 : 0;
            }
        });
        classDrop.onValueChanged.AddListener(value =>
        {
            if (value != 0)
            {
                if (data != null) {
                    data.Classinfo = classDrop.options[value].text;
                }
            }
        });
    }
    public override void OnShow()
    {
        base.OnShow();
        data = new DigitalHumanData();
    }
    public override void OnHide()
    {
        data = null;
        nameInput.text = "";
        urlInput.text = "";
        keyInput.text = "";
        classDrop.value = 0;
        isOpen.isOn = false;
        base.OnHide();
    }
    private void GetClassNmasDrop()
    {
        string classinfo = AppController.Instance.loginUser.Classinfo;
        List<string> classes = string.IsNullOrEmpty(classinfo)
            ? new List<string> { "暂无班级" }
            : classinfo.Contains("|")
                ? new List<string> { "班级筛选" }.Concat(classinfo.Split('|')).ToList()
                : new List<string> { "班级筛选", classinfo };

        classDrop.ClearOptions();
        classDrop.AddOptions(classes);
    }
    private void Save()
    {
        string name = nameInput.text.Trim();
        string url = urlInput.text.Trim();
        string key = keyInput.text.Trim();
        string classname = classDrop.value == 0 ? "" : classDrop.options[classDrop.value].text;
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(url) || string.IsNullOrEmpty(key) || classname == "")
        {
            UIManager.Instance.OpenMessageBoxUI("提示信息", "请完善输入信息！", 0, EnumMessageBoxType.OK);
        }
        else
        {
            data.SchoolName = AppController.Instance.loginUser.School_name;
            data.Classinfo = classDrop.options[classDrop.value].text;
            data.TeacherName = AppController.Instance.loginUser.Username;
            data.TeacherAccount = AppController.Instance.loginUser.Account;
            data.DigitalHumanName = nameInput.text.Trim();
            data.DigitalHumanURL = urlInput.text.Trim();
            data.DigitalHumanKey = keyInput.text.Trim();
            data.OnState = isOpen.isOn ? 1 : 0;
            if (data.OnState == 1)
            {
                _ = NetHelper.Instance.GetDigitalHumanonStateData(data.SchoolName, data.Classinfo, code =>
                {
                    if (code == 1)
                    {
                        UIManager.Instance.OpenMessageBoxUI("提示信息", "该班级已开启数字人服务！", 0, EnumMessageBoxType.OK);
                    }
                    else
                    {
                        aIDigitalHuman_Panel.AddData(data);
                    }
                });
            }
            else
            {
                aIDigitalHuman_Panel.AddData(data);
            }
        }
    }
}
