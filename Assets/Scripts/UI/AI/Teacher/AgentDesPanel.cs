using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AgentDesPanel : MonoBehaviour
{
    private RectTransform thisT;
    private float animationSpeed = 0.5f;
    private TMP_Text agentNameText, creatTimeText;
    private TMP_InputField agentDesText;
    private Button useBtn, backBtn;
    private RawImage textureImage;
    public AgentData Data { get; set; }
    private Texture2D oldTexture { get; set; }
    private Texture2D Texture { get; set; }
    public AIAgent_Panel agent_Panel;
    private void Awake()
    {
        thisT = GetComponent<RectTransform>();
        agentNameText=transform.Find("AgentName").GetComponent<TMP_Text>();
        creatTimeText = transform.Find("creattime").GetComponent<TMP_Text>();
        agentDesText = transform.Find("agentDes").GetComponent<TMP_InputField>();
        useBtn = transform.Find("useButton").GetComponent<Button>();
        backBtn = transform.Find("cancelButton").GetComponent<Button>();
        textureImage = transform.Find("RawImage").GetComponent<RawImage>();
        oldTexture = textureImage.texture as Texture2D;
    }
    public void Init(AgentData _data)
    {
        Data = _data;
        agentNameText.text = Data.AgentName;
        creatTimeText.text = Data.CreateTime.ToString();
        agentDesText.text = Data.Des;
        if (!string.IsNullOrEmpty(Data.ThumBnailPath))
        {
            PhpHelper.Instance.LoadTexture(Data.ThumBnailPath, OnTextureLoaded);
        }
    }
    private void Start()
    {
        useBtn.onClick.AddListener(() =>
        {
            agent_Panel.OpenUrlCustom(Data.AgentURL);
        });
        backBtn.onClick.AddListener(() =>
        {
            OnHide();
        });
    }

    private void OnTextureLoaded(Texture2D tex)
    {
        if (tex == null) return;
        textureImage.texture = tex;
    }
    public void OnShow()
    {
        thisT.DOLocalMoveX(0, animationSpeed);
    }
    public void OnHide()
    {
        thisT.DOLocalMoveX(1700, animationSpeed).OnComplete(() =>
        {
            OnReset();
        });
    }
    void OnReset()
    {
        textureImage.texture = oldTexture;
        agentNameText.text = "";
        creatTimeText.text = "";
        agentDesText.text = "";
        Data = null;
    }
}
