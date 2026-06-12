using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AgentItem : MonoBehaviour
{
    private RawImage agentIcon;
    private TMP_Text agentName;
    private Button agentBtn;
    private Button editorBtn;
    private Toggle selectToggle;

    private Texture2D runtimeTexture;   // 只管理运行时创建的贴图
    private Texture2D defaultTexture;   // Inspector默认贴图（不要销毁）

    private AIAgent_Panel aiAgent_Panel;
    public AgentData Data { get; private set; }

    private void Awake()
    {
        agentBtn = GetComponent<Button>();
        editorBtn = transform.Find("editorButton").GetComponent<Button>();
        agentIcon = transform.Find("Image").GetComponent<RawImage>();
        agentName = transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        selectToggle = transform.Find("Toggle").GetComponent<Toggle>();

        // 记录默认贴图（Asset）
        defaultTexture = agentIcon.texture as Texture2D;
    }

    private void Start()
    {
        agentBtn.onClick.AddListener(OnClickAgent);
        selectToggle.onValueChanged.AddListener(OnToggleChanged);
        editorBtn.onClick.AddListener(OnClickEditor);
        editorBtn.gameObject.SetActive(AppController.Instance.loginUser.Usertype == (int)UserType.教师);
    }

    #region 初始化

    public void Init(AgentData data, AIAgent_Panel panel)
    {
        aiAgent_Panel = panel;
        Data = data;
        if (Data == null) return;
        agentName.text = Data.AgentName;
        if (AppController.Instance.loginUser.Usertype == (int)UserType.教师)
        {
            bool isOwner = Data.Account == AppController.Instance.loginUser.Account;
            editorBtn.interactable = isOwner;
        }
        // 权限控制
        selectToggle.interactable =
            Data.Account == AppController.Instance.loginUser.Account;

        // 重置贴图
        ResetTexture();

        // 加载缩略图
        if (!string.IsNullOrEmpty(Data.ThumBnailPath))
        {
            PhpHelper.Instance.LoadTexture(Data.ThumBnailPath, OnTextureLoaded);
        }
    }

    #endregion

    #region 事件

    private void OnClickAgent()
    {
        if (aiAgent_Panel == null || Data == null) return;

        aiAgent_Panel.OpenURLDesPanel(Data);
    }

    private void OnToggleChanged(bool isOn)
    {
        if (aiAgent_Panel == null) return;

        if (isOn)
            aiAgent_Panel.Add(this);
        else
            aiAgent_Panel.Remove(this);
    }

    private void OnClickEditor()
    {
        if (aiAgent_Panel == null || Data == null) return;

        aiAgent_Panel.editorAgentPanel.OnShow();
        aiAgent_Panel.editorAgentPanel.Init(agentIcon.texture as Texture2D, Data);
    }

    #endregion

    #region 贴图管理（核心安全部分）

    private void OnTextureLoaded(Texture2D tex)
    {
        if (tex == null) return;

        // 销毁旧的运行时贴图
        if (runtimeTexture != null)
        {
            Destroy(runtimeTexture);
        }

        runtimeTexture = tex;
        agentIcon.texture = runtimeTexture;
    }

    private void ResetTexture()
    {
        // 销毁运行时贴图
        if (runtimeTexture != null)
        {
            Destroy(runtimeTexture);
            runtimeTexture = null;
        }

        // 恢复默认贴图
        agentIcon.texture = defaultTexture;
    }

    private void OnDestroy()
    {
        // 对象销毁时清理运行时贴图
        if (runtimeTexture != null)
        {
            Destroy(runtimeTexture);
        }
    }

    #endregion
}