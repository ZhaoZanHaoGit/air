using SFB;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddAgentPanel : BaseWindowPanel
{
    #region UI References

    private TMP_InputField agentNameInput;
    private TMP_Dropdown agentTypeDropDown;
    private Button selectTexturePathBtn;
    private TMP_InputField texturePathInput;
    private TMP_InputField desInput;
    private TMP_InputField agentURLInput;
    private Toggle isCommonToggle;
    private Button cancelBtn;
    private Button saveBtn;

    public Transform itemRoot;
    public GameObject item;

    #endregion

    public AIAgent_Panel agentPanel;

    private readonly List<AddAgentClassInfoItem> classInfoItems = new();
    private readonly List<string> selectedClassList = new();

    private string folderName = "";
    private string folderPath = "";

    private bool isSubmitting = false;

    #region Unity Lifecycle

    private void Awake()
    {
        agentNameInput = transform.Find("bg/agentname/InputField (TMP)").GetComponent<TMP_InputField>();
        agentTypeDropDown = transform.Find("bg/type/Dropdown").GetComponent<TMP_Dropdown>();
        selectTexturePathBtn = transform.Find("bg/texturepath/Button").GetComponent<Button>();
        texturePathInput = transform.Find("bg/texturepath/InputField (TMP)").GetComponent<TMP_InputField>();
        agentURLInput = transform.Find("bg/agentURL/InputField (TMP)").GetComponent<TMP_InputField>();
        cancelBtn = transform.Find("bg/btns/cancelButton").GetComponent<Button>();
        saveBtn = transform.Find("bg/btns/saveButton").GetComponent<Button>();
        desInput = transform.Find("bg/des/InputField (TMP)").GetComponent<TMP_InputField>();
        isCommonToggle = transform.Find("bg/isCommon/Toggle").GetComponent<Toggle>();
    }

    private void Start()
    {
        selectTexturePathBtn.onClick.AddListener(OnSelectTextureClicked);
        cancelBtn.onClick.AddListener(OnHide);
        saveBtn.onClick.AddListener(HandleSave);

        LoadClassInfo();
    }

    #endregion

    #region Class Info

    private void LoadClassInfo()
    {
        ClearClassItems();

        string classInfo = AppController.Instance.loginUser.Classinfo;
        if (string.IsNullOrEmpty(classInfo)) return;

        string[] classes = classInfo.Split('|');

        foreach (string c in classes)
        {
            GameObject obj = Instantiate(item, itemRoot);
            AddAgentClassInfoItem itemComp = obj.GetComponent<AddAgentClassInfoItem>();
            itemComp.Init(c);
            classInfoItems.Add(itemComp);
        }
    }

    private void ClearClassItems()
    {
        foreach (Transform child in itemRoot)
        {
            Destroy(child.gameObject);
        }

        classInfoItems.Clear();
        selectedClassList.Clear();
    }

    public void AddClass(string classStr)
    {
        if (!selectedClassList.Contains(classStr))
            selectedClassList.Add(classStr);
    }

    public void RemoveClass(string classStr)
    {
        selectedClassList.Remove(classStr);
    }

    #endregion

    #region Button Events

    private void OnSelectTextureClicked()
    {
        var extensions = new[]
        {
            new ExtensionFilter("Image Files", "jpg","jpeg","png")
        };

        string[] paths = StandaloneFileBrowser.OpenFilePanel("选择图片", "", extensions, false);
        if (paths == null || paths.Length == 0) return;

        string path = paths[0];

        if (!System.IO.File.Exists(path)) return;

        float sizeMB = GetFileSizeMB(path);
        if (sizeMB > 10f)
        {
            UIManager.Instance.OpenMessageBoxUI("错误提示", "图像大小不能超过10MB！", 0, EnumMessageBoxType.OK);
            return;
        }

        texturePathInput.text = path;
    }

    private void HandleSave()
    {
        if (isSubmitting) return;

        if (!ValidateInput()) return;

        isSubmitting = true;
        saveBtn.interactable = false;

        if (string.IsNullOrEmpty(texturePathInput.text))
        {
            SubmitData("", "");
        }
        else
        {
            UploadTexture(texturePathInput.text);
        }
    }

    #endregion

    #region Validation

    private bool ValidateInput()
    {
        if (string.IsNullOrEmpty(agentNameInput.text))
        {
            ShowMessage("请输入智能体名称！");
            return false;
        }

        if (string.IsNullOrEmpty(agentURLInput.text))
        {
            ShowMessage("请输入智能体URL！");
            return false;
        }

        if (!string.IsNullOrEmpty(texturePathInput.text) &&
            !System.IO.File.Exists(texturePathInput.text))
        {
            ShowMessage("请选择正确的图片路径！");
            return false;
        }

        return true;
    }

    #endregion

    #region Upload & Submit

    private void UploadTexture(string path)
    {
        PhpHelper.Instance.OnCreateSoftWareTextureFolder((fName, fPath) =>
        {
            if (string.IsNullOrEmpty(fPath))
            {
                ResetSubmitState();
                return;
            }

            folderName = fName;
            folderPath = fPath;

            List<string> paths = new() { path };

            PhpHelper.Instance.OnUploadFiles(paths, folderPath, (filename, filepath, type) =>
            {
                SubmitData(filepath, folderName);
            }, null);
        });
    }

    private void SubmitData(string thumbnailPath, string folder)
    {
        AgentData data = CreateAgentData(thumbnailPath, folder);

        _ = NetHelper.Instance.AddAgentData(data, code =>
        {
            if (code == 0)
            {
                agentPanel?.OnRefresh();
            }
            else if (code == -1)
            {
                ShowMessage("智能体名称已存在！");
            }
            else
            {
                ShowMessage("上传失败！");
            }

            ResetSubmitState();
            OnHide();
        });
    }

    private AgentData CreateAgentData(string thumbnailPath, string folder)
    {
        return new AgentData
        {
            SchoolName = AppController.Instance.loginUser.School_name,
            Account = AppController.Instance.loginUser.Account,
            AgentName = agentNameInput.text.Trim(),
            Type = agentTypeDropDown.value,
            ThumBnailPath = thumbnailPath,
            ThumbnailFolder = folder,
            AgentURL = agentURLInput.text.Trim(),
            CreateTime = DateTime.Now,
            Classinfo = string.Join("|", selectedClassList),
            Des = desInput.text.Trim(),
            IsCommon = isCommonToggle.isOn ? 1 : 0
        };
    }

    #endregion

    #region Utils

    private float GetFileSizeMB(string path)
    {
        long bytes = new System.IO.FileInfo(path).Length;
        return bytes / 1024f / 1024f;
    }

    private void ShowMessage(string msg)
    {
        UIManager.Instance.OpenMessageBoxUI("提示信息", msg, 0, EnumMessageBoxType.OK);
    }

    private void ResetSubmitState()
    {
        isSubmitting = false;
        saveBtn.interactable = true;
    }

    private void ResetUI()
    {
        agentNameInput.text = "";
        agentTypeDropDown.value = 0;
        texturePathInput.text = "";
        agentURLInput.text = "";
        desInput.text = "";
        isCommonToggle.isOn = false;

        folderName = "";
        folderPath = "";

        selectedClassList.Clear();
    }

    #endregion

    public override void OnHide()
    {
        ResetUI();
        base.OnHide();
    }
}
