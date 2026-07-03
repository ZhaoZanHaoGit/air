using Net.Component;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TraniningPanel : BaseUI
{

    public GameObject window1, window2, window3, window4, window5, window6;
    public GameObject testPanel, selectpanel, toolPanel;
    [Header("JSON 文件名")]
    public string jsonFileName = "cases.json";

    // 存储解析后的所有气路训练案例
    public List<PneumaticTrainingCase> trainingCases = new List<PneumaticTrainingCase>();


    [Header("列表按钮生成区")]
    public Transform buttonContainer;       // 按钮的父节点 (通常是 ScrollView -> Content)
    public GameObject caseButtonPrefab;     // 带有 Button 和 TMP_Text 的按钮预制体

    [Header("任务书显示区")]
    public TMP_Text taskDocumentText1, taskDocumentText2, taskDocumentText3;       // 显示 TaskDocument 信息的 TextMeshPro
    public TMP_Text taskName;       // 显示 TaskDocument 信息的 TextMeshPro

    [Header("回路图显示区")]
    public Transform schematicContainer;    // 生成图片的父节点
    public GameObject schematicImagePrefab; // 带有 Image 组件的预制体

    [Header("元件列表显示区")]
    public Transform componentContent;      // 显示 ComponentList 信息的 TextMeshPro
    public GameObject ItemcountPrefab; // 所需元器件预制体
    [Header("连线信息区")]
    public TMP_Text DebugData;      // 显示 ComponentList 信息的 TextMeshPro

    [Header("当前状态")]
    // 存储当前点击的按钮所对应的信息
    public PneumaticTrainingCase currentTrainingCase;
    private trainType trainType;
    public UIManager_qidong uIManager_qidong;

    public trainType TrainType
    {
        get { return trainType; }
        set
        {
            trainType = value;
            switch (trainType)
            {
                case trainType.free:
                    window2.SetActive(false);
                    window3.SetActive(false);
                    window4.SetActive(false);
                    window5.SetActive(false);
                    window6.SetActive(false);
                    if (SimulationLoop.Instance)
                    {
                        SimulationLoop.Instance.InitTrainType(currentTrainingCase, trainType);
                    }
                    break;
                case trainType.test:
                    window2.SetActive(true);
                    window3.SetActive(true);
                    window4.SetActive(true);
                    window5.SetActive(true);
                    window6.SetActive(false);
                    if (SimulationLoop.Instance)
                    {
                        SimulationLoop.Instance.InitTrainType(currentTrainingCase, trainType);
                    }
                    break;
                case trainType.exam:
                    window2.SetActive(true);
                    window3.SetActive(true);
                    window4.SetActive(false);
                    window5.SetActive(false);
                    window6.SetActive(true);
                    if (SimulationLoop.Instance)
                    {
                        SimulationLoop.Instance.InitTrainType(currentTrainingCase, trainType);
                    }
                    break;


            }
            // 无需 switch 时可移除
        }
    }
    float m_Timer = 0;
    public bool startTraining { get; set; }
    //界面切换相关逻辑定义
    #region  settingBtn

    private Button _setBtn;

    private Button setBtn => _setBtn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btnSetting");

    //    //最小化
    private Button _smallBtn;

    private Button smallBtn => _smallBtn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btnSmall");

    //    /// <summary>
    //    /// 窗口化
    //    /// </summary>
    private Button _windowBtn;

    private Button windowBtn => _windowBtn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btnWindow");

    ////退出
    private Button _exitBtn;

    private Button exitBTN => _exitBtn ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btnExit");
    #endregion

    #region LogicBtns
    private Button _freeModleBTN;

    private Button freeModleBTN => _freeModleBTN ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btnfreeModle");

    private Button _examBTN;

    private Button examBTN => _examBTN ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btnexam");

    private Button _testBTN;
    private Button testBTN => _testBTN ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btntest");

    private Button _qiguan;
    private Button qiguan => _qiguan ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btnqiguan");

    private Button _xixian;
    private Button xixian => _xixian ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btnxixian");

    private Button _cuxian;
    private Button cuxian => _cuxian ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btncuxian");
    private Button _deleteOne;
    private Button deleteOne => _deleteOne ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btndeleteOne");

    private Button _deleteAir;
    private Button deleteAir => _deleteAir ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btndeleteAir");

    private Button _deleteAll;
    private Button deleteAll => _deleteAll ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "deleteAll");
    #endregion
    public override EnumUIType GetUIType()
    {
        return EnumUIType.TraniningPanel;
    }
    private void Update()
    {
        if (startTraining)
        {

            m_Timer += Time.deltaTime;
        }

    }

    protected override void OnStart()
    {
        selectpanel.SetActive(true);
        testPanel.SetActive(false);
        toolPanel.SetActive(false);
        base.OnStart();
        InitUserInfo();
        LoadPneumaticCases();
        // 延迟调用或确保在 jsonReader 读取完成后调用
        // 也可以由 jsonReader 解析完毕后主动调用此方法
        taskDocumentText1.text = ""; taskDocumentText2.text = ""; taskDocumentText3.text = "";
        if (trainingCases.Count > 0)
        {
            InitializeCaseButtons();
        }
        else
        {
            Debug.LogWarning("未找到气动案例数据，请检查读取脚本是否先执行完毕。");
        }
        // Start is called before the first frame update
        #region settingevent
        EventTriggerListener.Get(setBtn).SetEventHandle(EnumTouchEventType.OnClick, OnSetBtnClick, null); //打开设置界面
        EventTriggerListener.Get(smallBtn).SetEventHandle(EnumTouchEventType.OnClick, onMinimumkClick, null); //最小化
        EventTriggerListener.Get(windowBtn).SetEventHandle(EnumTouchEventType.OnClick, onFullscreenClick, null); //窗口化
        EventTriggerListener.Get(exitBTN).SetEventHandle(EnumTouchEventType.OnClick, onQuitAppClick, null); //退出
        #endregion
        #region buttonevent
        EventTriggerListener.Get(freeModleBTN).SetEventHandle(EnumTouchEventType.OnClick, onfreeBtnClick, null); //自由模式
        EventTriggerListener.Get(testBTN).SetEventHandle(EnumTouchEventType.OnClick, ontestBtnClick, null); //练习模式
        EventTriggerListener.Get(examBTN).SetEventHandle(EnumTouchEventType.OnClick, onexamBtnClick, null); //考核模式
        EventTriggerListener.Get(qiguan).SetEventHandle(EnumTouchEventType.OnClick, onqiBtnClick, null); //连气管
        EventTriggerListener.Get(xixian).SetEventHandle(EnumTouchEventType.OnClick, onxidianBtnClick, null); //连细线
        EventTriggerListener.Get(cuxian).SetEventHandle(EnumTouchEventType.OnClick, oncudianBtnClick, null); //连粗线
        EventTriggerListener.Get(deleteOne).SetEventHandle(EnumTouchEventType.OnClick, ondeleteOneBtnClick, null); //删除单个管线
        EventTriggerListener.Get(deleteAll).SetEventHandle(EnumTouchEventType.OnClick, ondeleteAllBtnClick, null); //删除所有管线
        EventTriggerListener.Get(deleteAir).SetEventHandle(EnumTouchEventType.OnClick, ondeleteAirBtnClick, null); //删除所有管线
        #endregion
    }
    public override void InitUserInfo()
    {

        if (AppController.Instance != null && AppController.Instance.loginUser != null && AppController.Instance.loginUser.Usertype != (int)UserType.游客)
        {
            UserName.text = "用户名：" + AppController.Instance.loginUser.Username;
        }
        else
        {
            UserName.text = "游客模式";
        }
        AppController.Instance.menuPanelType = MenuPanelType.任务;
        // ID.text = "ID:" + AppController.Instance.loginUser.Account;
        //loginTime.text = "TIME:" + DateTime.Now.ToString();
        // LayoutRebuilder./ForceRebuildLayoutImmediate(horizontalLayoutGroup.GetComponent<RectTransform>());
    }
    public void LoadPneumaticCases()
    {
        // 拼接 StreamingAssets 路径
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);

        if (File.Exists(filePath))
        {
            try
            {
                // 读取 JSON 文件内容
                string jsonContent = File.ReadAllText(filePath);

                // 反序列化为 C# 对象列表
                trainingCases = JsonConvert.DeserializeObject<List<PneumaticTrainingCase>>(jsonContent);

                Debug.Log($"成功加载了 {trainingCases.Count} 个气动案例。");

                // 打印测试以验证读取结果
                if (trainingCases.Count > 0)
                {
                    Debug.Log($"首个案例校验 - 编号: {trainingCases[0].CaseNamber}, 名称: {trainingCases[0].Casname}");
                    if (trainingCases[0].CircuitSchematicNames != null && trainingCases[0].CircuitSchematicNames.Count > 0)
                    {
                        Debug.Log($"图纸名称: {trainingCases[0].CircuitSchematicNames[0]}");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"解析 JSON 时发生错误: {e.Message}");
            }
        }
        else
        {
            Debug.LogError($"未找到 JSON 文件，请确保文件存在于: {filePath}");
        }
        currentTrainingCase = null;
    }


    /// <summary>
    /// 初始化时根据 PneumaticTrainingCase 的数量生成按钮
    /// </summary>
    public void InitializeCaseButtons()
    {
        int index = 0;
        foreach (var trainingCase in trainingCases)
        {
            // 1. 生成按钮预制体
            GameObject btnObj = Instantiate(caseButtonPrefab, buttonContainer);
            // 2. 设置按钮显示的文本 (如需要，可使用 Casname)
            btnObj.GetComponent<caseBtn>().UpdateBtnDData("TASK·" + (index + 1).ToString(), trainingCase.Casname);
            // 3. 绑定点击事件，注意解决 Lambda 闭包问题
            Button btn = btnObj.GetComponent<Button>();
            PneumaticTrainingCase cachedCase = trainingCase; // 缓存局部变量
            btn.onClick.AddListener(() => OnCaseButtonClicked(cachedCase));
        }
    }/// <summary>
     /// 按钮点击响应逻辑
     /// </summary>
    private void OnCaseButtonClicked(PneumaticTrainingCase selectedCase)
    {
        // 记录当前点击对应的案例信息
        currentTrainingCase = selectedCase;
        taskName.text = selectedCase.Casname;
        UpdateTaskDocumentUI(selectedCase.TaskDocument);
        UpdateCircuitSchematicsUI(selectedCase.CircuitSchematicNames);
        UpdateComponentListUI(selectedCase.ComponentList);
    }

    /// <summary>
    /// 更新任务书 UI (TaskDocument)
    /// </summary>
    private void UpdateTaskDocumentUI(TaskDocument doc)
    {
        if (doc == null)
        {
            taskDocumentText1.text = "暂无任务书数据。";
            taskDocumentText2.text = "暂无任务书数据。";
            taskDocumentText3.text = "暂无任务书数据。";
            return;
        }


        // 将任务书内的不同部分拼接显示，若部分字段为空则自动忽略
        string content = "";
        /*
        if (!string.IsNullOrEmpty(doc.TaskRequirements))
            content += $"<b>任务要求：</b>\n{doc.TaskRequirements}\n\n";

        if (!string.IsNullOrEmpty(doc.TechnicalDocumentation))
            content += $"<b>技术资料：</b>\n{doc.TechnicalDocumentation}\n\n";

        if (!string.IsNullOrEmpty(doc.TaskImplementation))
            content += $"<b>任务实施：</b>\n{doc.TaskImplementation}";
        */
        if (!string.IsNullOrEmpty(doc.TaskRequirements))
            taskDocumentText1.text = doc.TaskRequirements;

        if (!string.IsNullOrEmpty(doc.TechnicalDocumentation))
            taskDocumentText2.text = doc.TechnicalDocumentation;

        if (!string.IsNullOrEmpty(doc.TaskImplementation))
            taskDocumentText3.text = doc.TaskImplementation;
        RectTransform Rect1 = (RectTransform)taskDocumentText1.transform;
        if (Rect1 != null)
        {
            // 强行重建当前物体以及所有子物体的布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(Rect1);
        }
        RectTransform Rect2 = (RectTransform)taskDocumentText2.transform;
        if (Rect2 != null)
        {
            // 强行重建当前物体以及所有子物体的布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(Rect2);
        }
        RectTransform Rect3 = (RectTransform)taskDocumentText3.transform;
        if (Rect3 != null)
        {
            // 强行重建当前物体以及所有子物体的布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(Rect3);
        }
        RectTransform Rect = (RectTransform)taskDocumentText1.transform.parent.parent;
        if (Rect != null)
        {
            // 强行重建当前物体以及所有子物体的布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
        }
      

    }

    /// <summary>
    /// 更新回路图 UI (CircuitSchematicNames)
    /// </summary>
    private void UpdateCircuitSchematicsUI(List<string> schematicNames)
    {
        // 每次点击新案例时，清空之前生成的旧图片
        foreach (Transform child in schematicContainer)
        {
            Destroy(child.gameObject);
        }

        if (schematicNames == null || schematicNames.Count == 0) return;

        foreach (string schematicName in schematicNames)
        {
            if (string.IsNullOrEmpty(schematicName)) continue;

            // 在 Assets/Resources/CircuitSchematic 目录下加载同名 Sprite 资源
            Sprite loadedSprite = Resources.Load<Sprite>($"CircuitSchematic/{schematicName}");

            if (loadedSprite != null)
            {
                // 创建图片对象并设置资源
                GameObject imgObj = Instantiate(schematicImagePrefab, schematicContainer);
                Image img = imgObj.GetComponent<Image>();
                img.sprite = loadedSprite;

                // 关键：大小按照资源的原始尺寸自适应
                img.SetNativeSize();
            }
            else
            {
                Debug.LogWarning($"缺少回路图资源: Assets/Resources/CircuitSchematic/{schematicName}");
            }
        }
    }

    /// <summary>
    /// 更新元件列表 UI (ComponentList)
    /// </summary>
    private void UpdateComponentListUI(List<Dictionary<string, int>> components)
    {
        for (int i = 0;i< componentContent.childCount; i++)
        {
            Destroy(componentContent.GetChild(i).gameObject);
        }

        if (components == null || components.Count == 0)
        {
           // componentListText.text = "暂无所需元件数据。";
            return;
        }

        //string result = "";

        // 解析格式类似 [{"2":2}, {"2":2}] 的结构
        foreach (var dict in components)
        {
            foreach (var kvp in dict)
            {
                GameObject btnObj = Instantiate(ItemcountPrefab, componentContent);
                // 2. 设置按钮显示的文本 (如需要，可使用 Casname)
                btnObj.GetComponent<caseBtn>().UpdateBtnDData("*" + kvp.Value.ToString(), kvp.Key);

                // 拼接 "string*数量" 并换行
                // result += $"{kvp.Key}*{kvp.Value}\n";
            }
        }

       // componentListText.text = result;
    }
    private void onfreeBtnClick(GameObject listener, object eventData, object[] args)
    {
        selectpanel.SetActive(false);
        testPanel.gameObject.SetActive(true);

        TrainType = trainType.free;


    }
    private void onexamBtnClick(GameObject listener, object eventData, object[] args)
    {
        if (currentTrainingCase == null)
            return;
        selectpanel.SetActive(false);
        testPanel.gameObject.SetActive(true);

        TrainType = trainType.exam;
        int num = currentTrainingCase.CaseNamber + 200;

        AppController.Instance.eVSType = (EVSType)Enum.Parse(typeof(EVSType), num.ToString());
        startTraining = true;
    }
    private void ontestBtnClick(GameObject listener, object eventData, object[] args)
    {
        if (currentTrainingCase == null)
            return;
        selectpanel.SetActive(false);
        testPanel.gameObject.SetActive(true);

        TrainType = trainType.test;
        AppController.Instance.courseType = CourseType.实训;
        int num = currentTrainingCase.CaseNamber + 300;
        AppController.Instance.pOTSType = (POTSType)Enum.Parse(typeof(POTSType), num.ToString());
        startTraining = true;
    }
    private void onqiBtnClick(GameObject listener, object eventData, object[] args)
    {
        if (TubeBuilderManager.Instance)
            TubeBuilderManager.Instance.StartBuildPneumaticTube();
    }
    private void onxidianBtnClick(GameObject listener, object eventData, object[] args)
    {
        if (TubeBuilderManager.Instance)
            TubeBuilderManager.Instance.StartBuildtinckwireTube();
    }
    private void oncudianBtnClick(GameObject listener, object eventData, object[] args)
    {
        if (TubeBuilderManager.Instance)
            TubeBuilderManager.Instance.StartBuildwireTube();
    }
    private void ondeleteOneBtnClick(GameObject listener, object eventData, object[] args)
    {
        if (SimulationLoop.Instance && SimulationLoop.Instance.currentLine)
        {
            Destroy(SimulationLoop.Instance.currentLine);
            SimulationLoop.Instance.currentLine = null;
        }
    }
    private void ondeleteAllBtnClick(GameObject listener, object eventData, object[] args)
    {
        if (SimulationLoop.Instance)
        {

            SimulationLoop.Instance.DeleteAllValves();
        }
    }
    private void ondeleteAirBtnClick(GameObject listener, object eventData, object[] args)
    { SimulationLoop.Instance.DeleteAllLines(); }

    public void toggleObj(GameObject obj)
    { obj.SetActive(!obj.activeSelf); }


    public void ExamUIFinish()
    {
        SimulationLoop.Instance.DeleteAllLines();
        currentTrainingCase = null;
        TrainType = trainType.None;
        AppController.Instance.courseType = CourseType.None;
        selectpanel.SetActive(true);
        testPanel.gameObject.SetActive(false);
        toolPanel.gameObject.SetActive(false);
        uIManager_qidong.clearButtonData();
    }

    #region setting
    private void onQuitAppClick(GameObject listener, object eventData, object[] args)
    {
        UIManager.Instance.OpenMessageBoxUI("提示", "确认退出！", 0, EnumMessageBoxType.OKCancel, OnQuit);
    }

    private void OnQuit(object args)
    {
        QuitSoftAndApp();
    }

    private void onFullscreenClick(GameObject listener, object eventData, object[] args)
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    private void onMinimumkClick(GameObject listener, object eventData, object[] args)
    {
        Minimze.Minmized();
    }

    private void OnSetBtnClick(GameObject listener, object eventData, object[] args)
    {
        OpenUI(EnumUIType.SettingPanel);
    }
    #endregion
    #region release 
    protected override void OnBtnRelease(GameObject listener, object eventData, params object[] args)
    {
        startTraining = false;
        SimulationLoop.Instance.DeleteAllValves();
        Debug.Log("TraniningPanel OnBtnRelease");
        //CloseUIToBeOpenUI(EnumUIType.CognitiveMenuPanel);
        if (TrainType == trainType.None)
        { CloseUIToBeOpenUI(EnumUIType.LoadingUI, true, EnumUIType.MainMenu, EnumSceneType.GameStart); }
        else
        {
          
            if (AppController.Instance.loginUser.Usertype == (int)UserType.学生 && TrainType == trainType.test)
            {
                Debug.Log("上传认知数据");
                SoftwareLearningData softwareLearningData = new SoftwareLearningData
                {
                    SoftID = (int)Defines.softInfo,
                    SoftName = Defines.softInfo.ToString(),
                    SchoolName = AppController.Instance.loginUser.School_name,
                    ClassInfo = AppController.Instance.loginUser.Classinfo,
                    Account = AppController.Instance.loginUser.Account,
                    UserName = AppController.Instance.loginUser.Username,
                    CourseType = (int)AppController.Instance.courseType,
                    CourseID = (int)AppController.Instance.pOTSType,
                    CourseName = AppController.Instance.pOTSType.ToString(),
                    LearnTime = (int)m_Timer,
                    Score = "100",
                    WorkOrder = "",
                    OperationContent = ""
                };
                _ = NetHelper.Instance.AddSoftLearningData(softwareLearningData, code =>
                {
                    if (code == -1)
                    {
                        UIManager.Instance.OpenMessageBoxUI("提示", "发送数据失败！", 0, EnumMessageBoxType.OK);
                    }
                    else
                    {
                        m_Timer = 0;
                    }
                });
            }
            currentTrainingCase = null;
            TrainType = trainType.None;
            AppController.Instance.courseType = CourseType.None;
            selectpanel.SetActive(true);
            testPanel.gameObject.SetActive(false);
            toolPanel.gameObject.SetActive(false);
            uIManager_qidong.clearButtonData();
            ItemManager.Instance.ClearAllItemData();
        }


    }
    #endregion
}
// 严格遵循原数据表命名，不作任何简化或修改
[System.Serializable]
public class PneumaticTrainingCase
{
    [JsonProperty("CaseNamber")]
    public int CaseNamber { get; set; }
    [JsonProperty("Casname")]
    public string Casname { get; set; }

    // 注意：已保留原 JSON 键名中的前导空格
    [JsonProperty(" CircuitSchematicNames")]
    public List<string> CircuitSchematicNames { get; set; }

    [JsonProperty("TaskDocument")]
    public TaskDocument TaskDocument { get; set; }

    // 格式为 [{"2":2}, {"2":2}]
    [JsonProperty("ComponentList")]
    public List<Dictionary<string, int>> ComponentList { get; set; }

    // 针对根目录中出现的 TaskRequirements 数组（如 [{"1":-10}]）
    [JsonProperty("TaskRequirements")]
    public List<Dictionary<string, int>> RootTaskRequirements { get; set; }

    // 兼容 Case 3 中在根目录替代出现的 TaskImplementation 字段
    [JsonProperty("TaskImplementation")]
    public string RootTaskImplementation { get; set; }
}

[System.Serializable]
public class TaskDocument
{
    [JsonProperty("TaskRequirements")]
    public string TaskRequirements { get; set; }

    [JsonProperty("TechnicalDocumentation")]
    public string TechnicalDocumentation { get; set; }

    [JsonProperty("TaskImplementation")]
    public string TaskImplementation { get; set; }
}