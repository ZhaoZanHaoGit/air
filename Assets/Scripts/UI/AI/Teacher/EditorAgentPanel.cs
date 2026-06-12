//using SFB;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using TMPro;
//using UnityEngine;
//using UnityEngine.Networking;
//using UnityEngine.UI;

//public class EditorAgentPanel : BaseWindowPanel
//{
//    private TMP_InputField agentNameInput;
//    private TMP_Dropdown agentTypeDropDown;
//    private Button selectTexturePathBtn;
//    private TMP_InputField agentURLInput;
//    private Button cancelBtn, saveBtn;
//    private TMP_InputField agentDesInput;
//    private RawImage agentIcon;
//    private Toggle isCommonToggle;
//    private List<AgentClassinfoItem> classinfoItem = new List<AgentClassinfoItem>();
//    private List<string> classlist = new List<string>();
//    public Transform itemRoot;
//    public GameObject item;
//    private AgentData Data { get; set; }
//    private string agentTexturePath;
//    public AIAgent_Panel agent_Panel;
//    private void Awake()
//    {
//        agentNameInput = transform.Find("bg/agentname/InputField (TMP)").GetComponent<TMP_InputField>();
//        agentTypeDropDown = transform.Find("bg/type/Dropdown").GetComponent<TMP_Dropdown>();
//        selectTexturePathBtn = transform.Find("bg/RawImage/Button").GetComponent<Button>();
//        agentURLInput = transform.Find("bg/agentURL/InputField (TMP)").GetComponent<TMP_InputField>();
//        agentDesInput = transform.Find("bg/des/InputField (TMP)").GetComponent<TMP_InputField>();
//        agentIcon = transform.Find("bg/RawImage").GetComponent<RawImage>();
//        isCommonToggle = transform.Find("bg/isCommon/Toggle").GetComponent<Toggle>();
//        cancelBtn = transform.Find("bg/btns/cancelButton").GetComponent<Button>();
//        saveBtn = transform.Find("bg/btns/saveButton").GetComponent<Button>();
//    }
//    private void Start()
//    {
//        selectTexturePathBtn.onClick.AddListener(() =>
//        {
//            string path = "";
//            var extensions = new[] {
//                new ExtensionFilter("Files", "jpg","jpeg","png")
//            };
//            path = WriteResult(StandaloneFileBrowser.OpenFilePanel("打开文件", "", extensions, true));
//            if (string.IsNullOrEmpty(path)) return;
//            float resstr = GetFileInfoSize(path);
//            if (resstr > 10)
//            {
//                //Debug.Log("文件大小大于150MB，请重新上传");
//                UIManager.Instance.OpenMessageBoxUI("错误提示", "图像大小大于10MB，请重新上传！", 0, EnumMessageBoxType.OK);
//                return;
//            }
//            if (!string.IsNullOrEmpty(Data.ThumbnailFolder))
//            {
//                string deleteTexture = Data.ThumbnailFolder;
//                PhpHelper.Instance.DeleteSoftWareTxtureFolder(deleteTexture, null);
//            }
//            LoadImageFromPath(path);
//            agentTexturePath = path;
//        });

//        saveBtn.onClick.AddListener(() =>
//        {
//            if (string.IsNullOrEmpty(agentNameInput.text))
//            {
//                UIManager.Instance.OpenMessageBoxUI("提示信息", "请输入Agent名称", 0, EnumMessageBoxType.OK);
//                return;
//            }
//            if (string.IsNullOrEmpty(agentURLInput.text))
//            {
//                UIManager.Instance.OpenMessageBoxUI("提示信息", "请输入Agent地址", 0, EnumMessageBoxType.OK);
//                return;
//            }
//            Data.AgentName = agentNameInput.text.Trim();
//            Data.AgentURL = agentURLInput.text.Trim();
//            Data.Des = agentDesInput.text.Trim();
//            Data.IsCommon = isCommonToggle.isOn ? 1 : 0;
//            if (classlist.Count <= 0)
//            {
//                Data.Classinfo = "";
//            }
//            else
//            {
//                Data.Classinfo = string.Join("|", classlist);
//            }
//            if (!string.IsNullOrEmpty(agentTexturePath))
//            {
//                if (!System.IO.File.Exists(agentTexturePath))
//                {
//                    UIManager.Instance.OpenMessageBoxUI("提示信息", "请选择正确的图片路径！", 0, EnumMessageBoxType.OK);
//                    return;
//                }
//                PhpHelper.Instance.OnCreateSoftWareTextureFolder(CreateFolderCallBack);
//            }
//            else
//            {
//                _ = NetHelper.Instance.UpdateAgentData(Data, code =>
//                {
//                    if (code == 0)
//                    {
//                        agent_Panel.OnRefresh();
//                    }
//                    else
//                    {
//                        UIManager.Instance.OpenMessageBoxUI("提示信息", "上传智能体数据失败！", 0, EnumMessageBoxType.OK);
//                    }
//                    OnHide();
//                });
//            }
//        });
//        cancelBtn.onClick.AddListener(() =>
//        {
//            OnHide();
//        });
//    }
//    string _FolderName = "";
//    string _FolderPath = "";
//    private void CreateFolderCallBack(string foldername, string folderpath)
//    {
//        if (string.IsNullOrEmpty(folderpath)) return;
//        _FolderName = foldername;
//        _FolderPath = folderpath;
//        List<string> paths = new List<string>();
//        paths.Add(agentTexturePath.Trim());
//        PhpHelper.Instance.OnUploadFiles(paths, _FolderPath, UpLoadCourseImageCallBack, null);
//    }
//    private void UpLoadCourseImageCallBack(string filename, string filepath, FileType type)
//    {
//        Data.ThumBnailPath = filepath;
//        Data.ThumbnailFolder = _FolderName;
//        _ = NetHelper.Instance.UpdateAgentData(Data, code =>
//        {
//            if (code == 0)
//            {
//                agent_Panel.OnRefresh();
//            }
//            else if(code == -1){
//                UIManager.Instance.OpenMessageBoxUI("提示信息", "智能体名称已存在，请重新输入！", 0, EnumMessageBoxType.OK);
//            }
//            else
//            {
//                UIManager.Instance.OpenMessageBoxUI("提示信息", "上传智能体数据失败！", 0, EnumMessageBoxType.OK);
//            }
//            OnHide();
//        });
//    }
//    public void Init(Texture2D texture, AgentData data)
//    {
//        Data = data;
//        agentIcon.texture = texture;
//        agentNameInput.text = data.AgentName;
//        agentURLInput.text = data.AgentURL;
//        agentDesInput.text = data.Des;
//        isCommonToggle.isOn = data.IsCommon == 1 ? true : false;
//        agentTypeDropDown.value = data.Type;
//        GetClassInfo();
//    }
//    private void GetClassInfo()
//    {
//        classinfoItem.Clear();
//        string classinfo = AppController.Instance.loginUser.Classinfo;
//        List<string> classes = new List<string>();
//        if (string.IsNullOrEmpty(classinfo))
//        {
//            return;
//        }
//        else if (classinfo.Contains("|"))
//        {
//            string[] classArray = classinfo.Split('|');
//            for (int i = 0; i < classArray.Length; i++)
//            {
//                classes.Add(classArray[i]);
//            }
//        }
//        else
//        {
//            classes.Add(classinfo);
//        }
//        for (int i = 0; i < classes.Count; i++)
//        {
//            GameObject obj = Instantiate(item, itemRoot);
//            AgentClassinfoItem a = obj.GetComponent<AgentClassinfoItem>();
//            a.Init(classes[i]);
//            classinfoItem.Add(a);
//        }
//        List<string> classinfodatas = new List<string>();
//        if (!string.IsNullOrEmpty(Data.Classinfo))
//        {
//            if (Data.Classinfo.Contains("|"))
//            {
//                string[] classinfoArray = Data.Classinfo.Split('|');
//                for (int i = 0; i < classinfoArray.Length; i++)
//                {
//                    classinfodatas.Add(classinfoArray[i]);
//                }
//            }
//            else
//            {
//                classinfodatas.Add(Data.Classinfo);
//            }
//        }
//        for (int i = 0; i < classinfoItem.Count; i++)
//        {
//            if (classinfodatas.Contains(classinfoItem[i].classname))
//            {
//                classinfoItem[i].SetSelect(true);
//            }
//            else
//            {
//                classinfoItem[i].SetSelect(false);
//            }
//        }
//    }
//    public void AddClass(string classstr)
//    {
//        if (classlist.Contains(classstr)) return;
//        classlist.Add(classstr);
//    }
//    public void RemoveClass(string classstr)
//    {
//        classlist.Remove(classstr);
//    }
//    public override void OnHide()
//    {
//        agentNameInput.text = "";
//        agentURLInput.text = "";
//        agentDesInput.text = "";
//        _FolderName = "";
//        _FolderPath = "";
//        classinfoItem.Clear();
//        classlist.Clear();
//        Data = null;
//        Reset(itemRoot);
//        base.OnHide();
//    }



//    public void LoadImageFromPath(string fullPath)
//    {
//        StartCoroutine(LoadImageCoroutine(fullPath));
//    }

//    private IEnumerator LoadImageCoroutine(string path)
//    {
//        if (!File.Exists(path))
//        {
//            Debug.LogError($"文件不存在: {path}");
//            yield break;
//        }

//        string url = "file://" + path;
//        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
//        {
//            url = "file:///" + path.Replace("\\", "/");
//        }
//        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
//        {
//            yield return request.SendWebRequest();

//            if (request.result == UnityWebRequest.Result.Success)
//            {
//                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
//                agentIcon.texture= texture;
//            }
//            else
//            {
//                Debug.LogError("加载失败: " + request.error);
//            }
//        }
//    }
//}

using SFB;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorAgentPanel : BaseWindowPanel
{
    private TMP_InputField agentNameInput;
    private TMP_Dropdown agentTypeDropDown;
    private Button selectTexturePathBtn;
    private TMP_InputField agentURLInput;
    private Button cancelBtn, saveBtn;
    private TMP_InputField agentDesInput;
    private RawImage agentIcon;
    private Toggle isCommonToggle;

    private List<AgentClassinfoItem> classinfoItem = new();
    private List<string> classlist = new();

    public Transform itemRoot;
    public GameObject item;

    private AgentData Data { get; set; }
    private string agentTexturePath;

    public AIAgent_Panel agent_Panel;

    private string _FolderName = "";
    private string _FolderPath = "";

    // 只管理运行时创建的纹理
    private Texture2D runtimeTexture;

    #region 初始化

    private void Awake()
    {
        agentNameInput = transform.Find("bg/agentname/InputField (TMP)").GetComponent<TMP_InputField>();
        agentTypeDropDown = transform.Find("bg/type/Dropdown").GetComponent<TMP_Dropdown>();
        selectTexturePathBtn = transform.Find("bg/RawImage/Button").GetComponent<Button>();
        agentURLInput = transform.Find("bg/agentURL/InputField (TMP)").GetComponent<TMP_InputField>();
        agentDesInput = transform.Find("bg/des/InputField (TMP)").GetComponent<TMP_InputField>();
        agentIcon = transform.Find("bg/RawImage").GetComponent<RawImage>();
        isCommonToggle = transform.Find("bg/isCommon/Toggle").GetComponent<Toggle>();
        cancelBtn = transform.Find("bg/btns/cancelButton").GetComponent<Button>();
        saveBtn = transform.Find("bg/btns/saveButton").GetComponent<Button>();
    }

    private void Start()
    {
        selectTexturePathBtn.onClick.AddListener(SelectImage);
        saveBtn.onClick.AddListener(OnSaveClicked);
        cancelBtn.onClick.AddListener(OnHide);
    }

    #endregion

    #region 图片选择

    private void SelectImage()
    {
        if (Data == null)
        {
            UIManager.Instance.OpenMessageBoxUI("错误", "数据未初始化！", 0, EnumMessageBoxType.OK);
            return;
        }

        var extensions = new[]
        {
            new ExtensionFilter("Image Files", "jpg","jpeg","png")
        };

        var paths = StandaloneFileBrowser.OpenFilePanel("选择图片", "", extensions, false);
        if (paths.Length == 0) return;

        string path = paths[0];

        if (!File.Exists(path)) return;

        float fileSizeMB = new FileInfo(path).Length / 1024f / 1024f;
        if (fileSizeMB > 10f)
        {
            UIManager.Instance.OpenMessageBoxUI("错误提示", "图像大小大于10MB，请重新上传！", 0, EnumMessageBoxType.OK);
            return;
        }

        if (!string.IsNullOrEmpty(Data.ThumbnailFolder))
        {
            PhpHelper.Instance.DeleteSoftWareTxtureFolder(Data.ThumbnailFolder, null);
        }

        LoadLocalImage(path);
        agentTexturePath = path;
    }

    private void LoadLocalImage(string path)
    {
        if (!File.Exists(path)) return;

        byte[] bytes = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);

        // 只销毁我们自己创建的纹理
        if (runtimeTexture != null)
        {
            Destroy(runtimeTexture);
        }

        runtimeTexture = tex;
        agentIcon.texture = runtimeTexture;
    }

    #endregion

    #region 保存逻辑

    private void OnSaveClicked()
    {
        if (Data == null)
        {
            UIManager.Instance.OpenMessageBoxUI("错误", "数据异常！", 0, EnumMessageBoxType.OK);
            return;
        }

        if (string.IsNullOrWhiteSpace(agentNameInput.text))
        {
            UIManager.Instance.OpenMessageBoxUI("提示", "请输入Agent名称", 0, EnumMessageBoxType.OK);
            return;
        }

        if (string.IsNullOrWhiteSpace(agentURLInput.text))
        {
            UIManager.Instance.OpenMessageBoxUI("提示", "请输入Agent地址", 0, EnumMessageBoxType.OK);
            return;
        }

        Data.AgentName = agentNameInput.text.Trim();
        Data.AgentURL = agentURLInput.text.Trim();
        Data.Des = agentDesInput.text.Trim();
        Data.Type = agentTypeDropDown.value;
        Data.IsCommon = isCommonToggle.isOn ? 1 : 0;
        Data.Classinfo = classlist.Count == 0 ? "" : string.Join("|", classlist);

        if (!string.IsNullOrEmpty(agentTexturePath))
        {
            if (!File.Exists(agentTexturePath))
            {
                UIManager.Instance.OpenMessageBoxUI("提示", "图片路径无效！", 0, EnumMessageBoxType.OK);
                return;
            }

            PhpHelper.Instance.OnCreateSoftWareTextureFolder(CreateFolderCallBack);
        }
        else
        {
            UpdateAgentData();
        }
    }

    private void CreateFolderCallBack(string foldername, string folderpath)
    {
        if (string.IsNullOrEmpty(folderpath)) return;

        _FolderName = foldername;
        _FolderPath = folderpath;

        PhpHelper.Instance.OnUploadFiles(
            new List<string> { agentTexturePath },
            _FolderPath,
            UploadImageCallBack,
            null);
    }

    private void UploadImageCallBack(string filename, string filepath, FileType type)
    {
        Data.ThumBnailPath = filepath;
        Data.ThumbnailFolder = _FolderName;

        UpdateAgentData();
    }

    private void UpdateAgentData()
    {
        _ = NetHelper.Instance.UpdateAgentData(Data, code =>
        {
            if (code == 0)
            {
                agent_Panel?.OnRefresh();
            }
            else if (code == -1)
            {
                UIManager.Instance.OpenMessageBoxUI("提示", "智能体名称已存在！", 0, EnumMessageBoxType.OK);
                return;
            }
            else
            {
                UIManager.Instance.OpenMessageBoxUI("提示", "保存失败！", 0, EnumMessageBoxType.OK);
                return;
            }

            OnHide();
        });
    }

    #endregion

    #region 初始化数据

    public void Init(Texture2D texture, AgentData data)
    {
        Data = data;

        // 释放旧运行时纹理
        if (runtimeTexture != null)
        {
            Destroy(runtimeTexture);
            runtimeTexture = null;
        }

        agentIcon.texture = texture;

        agentNameInput.text = data.AgentName;
        agentURLInput.text = data.AgentURL;
        agentDesInput.text = data.Des;
        agentTypeDropDown.value = data.Type;
        isCommonToggle.isOn = data.IsCommon == 1;

        GetClassInfo();

        for (int i = 0; i < classlist.Count; i++)
        {
            Debug.Log(classlist[i]);
        }

    }

    private void GetClassInfo()
    {
        Reset(itemRoot);
        classinfoItem.Clear();
        classlist.Clear();

        string classinfo = AppController.Instance.loginUser.Classinfo;
        if (string.IsNullOrEmpty(classinfo)) return;

        List<string> allClasses = new(classinfo.Split('|'));

        foreach (var c in allClasses)
        {
            GameObject obj = Instantiate(item, itemRoot);
            var a = obj.GetComponent<AgentClassinfoItem>();
            a.Init(c);
            classinfoItem.Add(a);
        }

        if (!string.IsNullOrEmpty(Data.Classinfo))
        {
            var selected = new List<string>(Data.Classinfo.Split('|'));

            foreach (var item in classinfoItem)
            {
                bool isSelected = selected.Contains(item.classname);
                item.SetSelect(isSelected);

                if (isSelected)
                {
                    classlist.Add(item.classname);
                }
            }
        }
    }

    #endregion

    #region 班级接口

    public void AddClass(string classstr)
    {
        if (!classlist.Contains(classstr))
            classlist.Add(classstr);
    }

    public void RemoveClass(string classstr)
    {
        classlist.Remove(classstr);
    }

    #endregion

    #region 关闭清理

    public override void OnHide()
    {
        agentNameInput.text = "";
        agentURLInput.text = "";
        agentDesInput.text = "";

        classinfoItem.Clear();
        classlist.Clear();

        agentTexturePath = "";
        _FolderName = "";
        _FolderPath = "";

        if (runtimeTexture != null)
        {
            Destroy(runtimeTexture);
            runtimeTexture = null;
        }

        agentIcon.texture = null;
        Reset(itemRoot);

        Data = null;

        base.OnHide();
    }

    #endregion
}