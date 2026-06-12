using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditDigitalHumanData_Panel : BaseWindowPanel
{
    private TMP_InputField nameInput, urlInput, keyInput;
    private TMP_Dropdown classDrop;
    private Toggle isOpen;
    private Button cancelBtn, saveBtn;
    private DigitalHumanData Data { get; set; }
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
        cancelBtn.onClick.AddListener(() =>
        {
            OnHide();
        });
        saveBtn.onClick.AddListener(() =>
        {
            UpdateData();
        });
        isOpen.onValueChanged.AddListener(isOn =>
        {
            if (Data != null)
            {
                Data.OnState = isOn ? 1 : 0;
            }
        });
        classDrop.onValueChanged.AddListener(value =>
        {
            if (value != 0)
            {
                if (Data != null)
                {
                    Data.Classinfo = classDrop.options[value].text;
                }
            }
        });
    }
    public override void OnHide()
    {
        Data = null;
        nameInput.text = "";
        urlInput.text = "";
        keyInput.text = "";
        classDrop.value = 0;
        isOpen.isOn = false;
        base.OnHide();
    }
    public void Init(DigitalHumanData data)
    {
        OnShow();
        GetClassNmasDrop();
        Data = data;
        nameInput.text = data.DigitalHumanName;
        urlInput.text = data.DigitalHumanURL;
        keyInput.text = data.DigitalHumanKey;
        isOpen.isOn = data.OnState == 1 ? true : false;
        SelectOptionByText(data.Classinfo);
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
    public void SelectOptionByText(string targetText)
    {
        for (int i = 0; i < classDrop.options.Count; i++)
        {
            if (classDrop.options[i].text == targetText)
            {
                classDrop.value = i;
                classDrop.RefreshShownValue();
                return; 
            }
        }
        Debug.LogWarning($"未找到文本为 '{targetText}' 的选项！");
    }
    private void UpdateData()
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
            Data.SchoolName = AppController.Instance.loginUser.School_name;
            Data.Classinfo = classDrop.options[classDrop.value].text;
            Data.DigitalHumanName = name;
            Data.DigitalHumanURL = url;
            Data.DigitalHumanKey = key;
            Data.OnState = isOpen.isOn ? 1 : 0;
            //Debug.Log("Data.OnState" + Data.OnState);
            //Debug.Log("Data.Classinfo" + Data.Classinfo);
            if (Data.OnState == 1)
            {
                Debug.Log(Data.SchoolName);
                Debug.Log(Data.Classinfo);
                _ = NetHelper.Instance.GetDigitalHumanonStateData(Data.SchoolName, Data.Classinfo, code =>
                {
                    Debug.Log(code);
                    if (code == 1)
                    {
                        UIManager.Instance.OpenMessageBoxUI("提示信息", "该班级已开启数字人服务！", 0, EnumMessageBoxType.OK);
                    }
                    else
                    {
                        aIDigitalHuman_Panel.UpdateData(Data);
                    }
                });
            }
            else
            {
                aIDigitalHuman_Panel.UpdateData(Data);
            }
        }
    }
}
