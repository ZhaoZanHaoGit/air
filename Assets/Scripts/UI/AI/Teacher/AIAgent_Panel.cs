using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIAgent_Panel : BasePanel
{
    public override PanelUIType GetUIType()
    {
        return PanelUIType.AIAgent_Panel;
    }

    private Button addAgentBtn;
    private Button deleteAgentBtn;

    public Toggle[] toggles;
    public Transform agentRoom;
    public GameObject agentRoomPrefab;

    public AgentType currentAgentType = AgentType.None;

    public List<AgentItem> allAgentItem = new List<AgentItem>();
    public List<AgentItem> selectAgentItem = new List<AgentItem>();


    public AddAgentPanel addAgentPanel;
    public EditorAgentPanel editorAgentPanel;

    public AgentDesPanel agentDesPanel;



    public override void Awake()
    {
        base.Awake();

        addAgentBtn = transform.Find("addAgent").GetComponent<Button>();
        deleteAgentBtn = transform.Find("deleteAgent").GetComponent<Button>();
        deleteAgentBtn.interactable = false;
    }

    private void Start()
    {
        addAgentBtn.onClick.AddListener(() =>
        {
            addAgentPanel.OnShow();
        });

        deleteAgentBtn.onClick.AddListener(OnDeleteAgentClicked);

        InitializeToggles(0);

        for (int i = 0; i < toggles.Length; i++)
        {
            int index = i;
            toggles[i].onValueChanged.RemoveAllListeners();
            toggles[i].onValueChanged.AddListener((bool isOn) =>
            {
                OnToggleChanged(index, isOn);
            });
        }
        if (AppController.Instance.loginUser.Usertype == (int)UserType.教师)
        {
            addAgentBtn.gameObject.SetActive(true);
            deleteAgentBtn.gameObject.SetActive(true);
        }
        else
        {
            addAgentBtn.gameObject.SetActive(false);
            deleteAgentBtn.gameObject.SetActive(false);
        }
    }

    #region 删除逻辑

    private void OnDeleteAgentClicked()
    {
        if (selectAgentItem.Count == 0) return;

        UIManager.Instance.OpenMessageBoxUI(
            "提示信息",
            "确认删除所选智能体？",
            0,
            EnumMessageBoxType.OKCancel,
            DeleteAgentDatas);
    }

    private void DeleteAgentDatas(object args)
    {
        deleteAgentBtn.interactable = false;

        List<int> ids = new List<int>();

        foreach (var item in selectAgentItem)
        {
            if (item?.Data == null) continue;

            if (!string.IsNullOrEmpty(item.Data.ThumbnailFolder))
            {
                PhpHelper.Instance.DeleteSoftWareTxtureFolder(
                    item.Data.ThumbnailFolder,
                    null);
            }

            ids.Add(item.Data.ID);
        }

        _ = NetHelper.Instance.DeleteAgentData(ids, code =>
        {
            if (code == 0)
            {
                selectAgentItem.Clear();   //修复：删除成功后清空选中
                OnRefresh();
            }
            else
            {
                UIManager.Instance.OpenMessageBoxUI(
                    "提示信息",
                    "删除智能体数据失败，请重试！",
                    0,
                    EnumMessageBoxType.OK);
            }
        });
    }

    #endregion

    #region 数据加载

    public override void Init()
    {
        if (agentRoom.childCount > 0) return;

        if (AppController.Instance.allAgentDatas != null &&
            AppController.Instance.allAgentDatas.Count > 0)
        {
            SpawnAgentItems(AppController.Instance.allAgentDatas);
        }
        else
        {
            OnRefresh();
        }
    }

    public override void OnRefresh()
    {

        if (agentRoom.childCount > 0)
        {
            DeleteChildren(agentRoom);
            AppController.Instance.allAgentDatas.Clear();
            allAgentItem.Clear();
            selectAgentItem.Clear();
        }

        if (AppController.Instance.loginUser.Usertype == (int)UserType.教师)
        {
            _ = NetHelper.Instance.GetAgentDatasByTeacher(AppController.Instance.loginUser, (datas) =>
            {
                AppController.Instance.allAgentDatas = datas;
                SpawnAgentItems(datas);
            });
        }
        else
        {
            _ = NetHelper.Instance.GetAgentDatasByStudent(AppController.Instance.loginUser, (datas) =>
            {
                AppController.Instance.allAgentDatas = datas;
                SpawnAgentItems(datas);
            });
        }
    }

    #endregion

    #region 生成与分页

    public void SpawnAgentItems(List<AgentData> datas)
    {
        if (datas.Count <= 0) return;
        foreach (var d in datas)
        {
            GameObject go = Instantiate(agentRoomPrefab, agentRoom);
            AgentItem item = go.GetComponent<AgentItem>();
            item.Init(d, this);
            allAgentItem.Add(item);
            if (currentAgentType == AgentType.None)
            {
                go.SetActive(true);
            }
            else
            {
                go.SetActive((AgentType)d.Type == currentAgentType);
            }
        }
    }
    #endregion



    #region Toggle 筛选

    private void InitializeToggles(int defaultIndex)
    {
        if (toggles == null || toggles.Length == 0) return;

        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].SetIsOnWithoutNotify(i == defaultIndex);
        }

        UpdateAgentTypeByIndex(defaultIndex);
        ShowHideAgent(currentAgentType);
    }

    private void OnToggleChanged(int index, bool isOn)
    {
        toggles[index]
            .GetComponent<ToggleTextColorHelper>()
            .SetToggleValue(isOn);

        if (!isOn) return;

        UpdateAgentTypeByIndex(index);
        ShowHideAgent(currentAgentType);
    }

    void ShowHideAgent(AgentType type)
    {
        if (allAgentItem.Count <= 0) return;
        if (type == AgentType.None)
        {
            foreach (var item in allAgentItem)
            {
                item.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (var item in allAgentItem)
            {
                item.gameObject.SetActive((AgentType)item.Data.Type == currentAgentType);
            }
        }
    }

    private void UpdateAgentTypeByIndex(int index)
    {
        switch (index)
        {
            case 0: currentAgentType = AgentType.None; break;
            case 1: currentAgentType = AgentType.技术支持; break;
            case 2: currentAgentType = AgentType.私域运营; break;
            case 3: currentAgentType = AgentType.客服销售; break;
            case 4: currentAgentType = AgentType.获客引流; break;
            case 5: currentAgentType = AgentType.品牌营销; break;
            case 6: currentAgentType = AgentType.项目设计; break;
            case 7: currentAgentType = AgentType.教研工具; break;
            case 8: currentAgentType = AgentType.课件内容生成; break;
            case 9: currentAgentType = AgentType.模型3D生成; break;
            case 10: currentAgentType = AgentType.辅助学习工具; break;
            default: currentAgentType = AgentType.None; break;
        }
    }

    #endregion

    #region 选择逻辑

    public void Add(AgentItem item)
    {
        if (!selectAgentItem.Contains(item))
        {
            selectAgentItem.Add(item);
            UpdateButtonState();
        }
    }

    public void Remove(AgentItem item)
    {
        selectAgentItem.Remove(item);
        UpdateButtonState();
    }

    void UpdateButtonState()
    {
        deleteAgentBtn.interactable = selectAgentItem.Count > 0;
    }

    #endregion

    #region 打开URL（跨平台安全）

    public void OpenUrlCustom(string url)
    {
        if (string.IsNullOrEmpty(url)) return;

        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
        {
            url = "https://" + url;
        }

        Application.OpenURL(url);  // 跨平台安全
    }

    #endregion

    public void OpenURLDesPanel(AgentData data)
    {
        agentDesPanel.Init(data);
        agentDesPanel.OnShow();
    }
    public override void OnHide()
    {
        agentDesPanel.OnHide();
        base.OnHide();
    }
}