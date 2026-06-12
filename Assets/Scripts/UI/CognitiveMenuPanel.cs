using SuperTreeView; // 确保引入了树形视图的命名空间
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CognitiveMenuPanel : BaseUI
{
    //功能逻辑相关变量定义

    [Header("UI Reference")]
    public TreeView mTreeView;
    [Header("Data")]
    public PneumaticComponentsData componentData;

    [Header("Display Image")]
    public Image componentImage, specificationImage;
    public RawImage checkImage;

    [Header("Describe Text")]
    public TMP_Text componentDescribe,itemName;

    private int mCurSelectedItemId = 0;     // 当前选中节点的唯一树ID

    private float m_Timer;

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

    protected override void OnStart()
    {

        base.OnStart();
        InitUserInfo();
        LoadItemData();
        // Start is called before the first frame update
        #region settingevent
        EventTriggerListener.Get(setBtn).SetEventHandle(EnumTouchEventType.OnClick, OnSetBtnClick, null); //打开设置界面
        EventTriggerListener.Get(smallBtn).SetEventHandle(EnumTouchEventType.OnClick, onMinimumkClick, null); //最小化
        EventTriggerListener.Get(windowBtn).SetEventHandle(EnumTouchEventType.OnClick, onFullscreenClick, null); //窗口化
        EventTriggerListener.Get(exitBTN).SetEventHandle(EnumTouchEventType.OnClick, onQuitAppClick, null); //退出
        #endregion

    }
    // Update is called once per frame
    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        m_Timer += deltaTime;
        //  m_TimerString = UnityHelper.SecondConvertToString((int)m_Timer);
        // timeText.text = m_TimerString;
    }

    public override EnumUIType GetUIType()
    {
        return EnumUIType.CognitiveMenuPanel;
    }

    public virtual void InitUserInfo()
    {
        if (AppController.Instance != null && AppController.Instance.loginUser != null && AppController.Instance.loginUser.Usertype != (int)UserType.游客)
        {
            UserName.text = "用户名：" + AppController.Instance.loginUser.Username;
        }
        else
        {
            UserName.text = "游客模式";
        }
        AppController.Instance.cLSType = CLSType.元器件认知;
        AppController.Instance.courseType = CourseType.认知;
        AppController.Instance.menuPanelType = MenuPanelType.认知;
        // ID.text = "ID:" + AppController.Instance.loginUser.Account;
        //loginTime.text = "TIME:" + DateTime.Now.ToString();
        // LayoutRebuilder./ForceRebuildLayoutImmediate(horizontalLayoutGroup.GetComponent<RectTransform>());
    }
    void LoadItemData()
    {// 拼接路径
        string filePath = Path.Combine(Application.streamingAssetsPath, "ItemData", "PneumaticComponents.json");

        if (File.Exists(filePath))
        {
            // 直接单行读取文本
            string jsonText = File.ReadAllText(filePath);
            componentData = JsonUtility.FromJson<PneumaticComponentsData>(jsonText);

            Debug.Log($"[PC端本地读取] 成功加载 {componentData.components.Count} 个元件。");
        }
        else
        {
            Debug.LogError($"未找到 JSON 文件: {filePath}");
        }

        // 2. 初始化树形视图并绑定核心事件
        if (mTreeView != null)
        {
            mTreeView.OnItemExpandBegin = OnItemExpandBegin;
            mTreeView.OnItemCollapseBegin = OnItemCollapseBegin;
            mTreeView.OnItemCustomEvent = OnItemCustomEvent;
            // mTreeView.OnTreeListRepositionFinish = OnTreeListRepositionFinish; // 核心：画线回调
            mTreeView.InitView();

            // 3. 根据加载的数据构建两级树菜单
            BuildTreeViewFromData();
        }
        else
        {
            Debug.LogError("未在 CognitiveMenuPanel 中绑定 mTreeView！");
        }
    }
    /// <summary>
    /// 核心逻辑：解析数据并分级构建树形视图
    /// </summary>
    void BuildTreeViewFromData()
    {
        if (componentData == null || componentData.components == null) return;

        // 用于去重和快速查找大类的字典 key: category名称, value: 树节点对象
        Dictionary<string, TreeViewItem> categoryNodes = new Dictionary<string, TreeViewItem>();

        foreach (var comp in componentData.components)
        {
            // 确保分类名称不为空，没有分类的归为"未分类"
            string categoryName = string.IsNullOrEmpty(comp.category) ? "其他大类" : comp.category;

            // 1. 如果字典里还没有这个大分类，先创建第一层父节点
            if (!categoryNodes.ContainsKey(categoryName))
            {
                // 使用在 DemoScene2Script 中配置的 "ItemPrefab1" 预制体
                TreeViewItem parentNode = mTreeView.AppendItem("ItemPrefab1");

                // 假设大分类统一用文件夹图标 "folder"
                parentNode.GetComponent<ItemScript1>().SetItemInfo(categoryName, categoryName, categoryName);

                // 默认有子节点的父节点，需要显示展开箭头
                parentNode.GetComponent<ItemScript1>().SetExpandBtnVisible(true);
                parentNode.GetComponent<ItemScript1>().SetExpandStatus(parentNode.IsExpand);

                categoryNodes.Add(categoryName, parentNode);
            }

            // 2. 获取大分类节点，并在其子树（ChildTree）下追加二级子节点（具体元件）
            TreeViewItem parentCategoryNode = categoryNodes[categoryName];
            TreeViewItem childNode = parentCategoryNode.ChildTree.AppendItem("ItemPrefab1");

            // 使用数据里的 iconName 作为图片名，comp.name 作为显示文本
            // 将整个组件的 name 存入 Data，方便点击时识别
            childNode.GetComponent<ItemScript1>().SetItemInfo(comp.category, comp.name, comp);

            // 叶子节点没有下一级子节点，隐藏展开按钮
            childNode.GetComponent<ItemScript1>().SetExpandBtnVisible(false);
            // ==================== 核心刷新代码 ====================
            // 1. 强制让 Layout Group 所在的容器立刻重新计算布局

        }
        mTreeView.gameObject.SetActive(false);
        mTreeView.gameObject.SetActive(true);

    }

    #region 树形视图事件回调 (仿 DemoScene2Script 核心逻辑)

    // 当节点开始展开时：旋转 UI 箭头为展开方向
    void OnItemExpandBegin(TreeViewItem item)
    {
        item.GetComponent<ItemScript1>().SetExpandStatus(true);
    }

    // 当节点开始折叠时：旋转 UI 箭头为折叠方向
    void OnItemCollapseBegin(TreeViewItem item)
    {
        item.GetComponent<ItemScript1>().SetExpandStatus(false);
    }

    /// <summary>
    /// 处理单选高亮逻辑
    /// </summary>
    void OnItemCustomEvent(TreeViewItem item, CustomEvent customEvent, System.Object param)
    {
        if (customEvent == CustomEvent.ItemClicked)
        {
            ItemScript1 itemScript = item.GetComponent<ItemScript1>();

            if (mCurSelectedItemId > 0)
            {
                if (item.ItemId == mCurSelectedItemId) return; // 重复点击不做处理

                // 取消上一个选择节点的高亮状态
                TreeViewItem curSelectedItem = mTreeView.GetTreeItemById(mCurSelectedItemId);
                if (curSelectedItem != null)
                {
                    curSelectedItem.GetComponent<ItemScript1>().IsSelected = false;
                }
                mCurSelectedItemId = 0;
            }

            // 高亮新选择的节点并记录ID
            itemScript.IsSelected = true;
            mCurSelectedItemId = item.ItemId;

            // 在这里触发你的业务逻辑，比如：展示该元件的详细文本、图片、视频
            OnComponentSelected(itemScript.Data);
        }
    }

    /// <summary>
    /// 当点击树节点时触发的业务响应
    /// </summary>
    /// <param name="componentName">元件名称</param>
    void OnComponentSelected(object componentName)
    {
        ComponentItem item = componentName as ComponentItem;

        if (item != null)
        {
            // 解包成功，可以安全地使用 item 的属性
            Debug.Log("用户选中了元件,元件名称是: " + item.name);

            componentDescribe.text = item.describeText;
            RectTransform Rect = componentDescribe.GetComponent<RectTransform>();
            if (Rect != null)
            {
                // 强行重建当前物体以及所有子物体的布局
                LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
            }
            UpdateComponentImage(item.iconName);
            showModdle(item);
        }
        else
        {
            string nameStr = componentName as string;
            if (nameStr != null)
            { Debug.Log("用户选中了分类: " + nameStr); }
            else
            {
                // 解包失败，说明这个 object 原本不是 ComponentItem 类型
                Debug.LogWarning("转换失败，类型不匹配！");

            }

        }
        // 可以在此处从 componentData.components 列表中匹配该名字，进而读取 describeText 等
    }

    // 辅助方法：快捷设置 RectTransform 的高度
    void SetRectTransformHeight(RectTransform rt, float height)
    {
        Vector2 size = rt.sizeDelta;
        size.y = height;
        rt.sizeDelta = size;
    }
    void showModdle(ComponentItem item)
    {
        itemName.text = item.name;
        string perfabname = item.prefabName;
        if (perfabname == null)
        {
            Debug.LogWarning("未绑定 perfabname 变量，无法更新模型。");
            return;
        }

        if (string.IsNullOrEmpty(perfabname))
        {
            checkImage.gameObject.SetActive(false);

            Debug.LogWarning("未读取到模型名，无法更新模型。");
            return;
        }
        checkImage.gameObject.SetActive(true);
        Vector3 showScale = Vector3.one * 20;
        // 拼接路径：Resources 目录下的相对路径 "Symbol/图片名"
        string resourcePath = "ShowModel/" + perfabname;
        GameObject targetModel = Resources.Load<GameObject>(resourcePath);
        if (targetModel == null)
        {
            checkImage.gameObject.SetActive(false);
            Debug.LogWarning("未读取到模型，无法更新模型。");
            return;

        }
        ModelShowManager.Instance.ModelShow(targetModel.transform, showScale, CameraShowType.Object3D);

    }


    // ==================== 4. 新增：动态加载 Resources/Symbol 图片的方法 ====================
    /// <summary>
    /// 根据图片名加载并替换 UI Image 组件的 Sprite
    /// </summary>
    void UpdateComponentImage(string iconName)
    {
        if (componentImage == null)
        {
            Debug.LogWarning("未绑定 componentImage 变量，无法更新图片。");
            return;
        }

        if (string.IsNullOrEmpty(iconName))
        {
            componentImage.sprite = null;
            componentImage.enabled = false;
            Debug.LogWarning("未读取到图片名，无法更新图片。");
            return;
        }

        // 拼接路径：Resources 目录下的相对路径 "Symbol/图片名"
        string resourcePath = "Symbol/" + iconName;
        Sprite targetSprite = Resources.Load<Sprite>(resourcePath);

        if (targetSprite != null)
        {
            // Debug.LogWarning($"在 Resources/Symbol/ 路径下找到名为 [{iconName}] 的图片。");
            componentImage.sprite = targetSprite;
            componentImage.enabled = true; // 确保可见
        }
        else
        {
            // 找不到图片则隐藏 Image 组件不显示
            Debug.LogWarning($"未能在 Resources/Symbol/ 路径下找到名为 [{iconName}] 的图片。");
            componentImage.sprite = null;
            componentImage.enabled = false;
        }
        // 拼接路径：Resources 目录下的相对路径 "Specification/图片名"
        string SpecificationPath = "Specification/技术参数";
        Sprite SpecificationSprite = Resources.Load<Sprite>(SpecificationPath);

        if (SpecificationSprite != null)
        {
            // Debug.LogWarning($"在 Resources/Symbol/ 路径下找到名为 [{iconName}] 的图片。");
            specificationImage.sprite = SpecificationSprite;
            specificationImage.enabled = true; // 确保可见
        }
        else
        {
            // 找不到图片则隐藏 Image 组件不显示
            Debug.LogWarning($"未能在 Resources/Symbol/ 路径下找到名为 [{iconName}] 的图片。");
            specificationImage.sprite = null;
            specificationImage.enabled = false;
        }
    }

    #endregion
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

        //CloseUIToBeOpenUI(EnumUIType.CognitiveMenuPanel);
        CloseUIToBeOpenUI(EnumUIType.MainMenu);
    }
    protected override void OnRelease()
    {
        base.OnRelease();
        State = EnumObjectState.None;
        if (true)
        {
            if (AppController.Instance.loginUser.Usertype == (int)UserType.学生)
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
                    CourseID = (int)AppController.Instance.cLSType,
                    CourseName = AppController.Instance.cLSType.ToString(),
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
                });
            }
        }
    }
    #endregion
}
