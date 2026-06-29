using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItenMenuUI_5 : ItenMenuUIBase
{
    public List<ToggleGroup> toggleGroups;
    public Button confirmButton;
    [Header("UI 组件")]
    public TMP_Dropdown signalDropdown1;
    
    [Header("当前操作对象")]
    private RollerValve32 currentValve;
    private List<BaseSignalSource> availableSignals = new List<BaseSignalSource>();

    void Awake()
    {
        // 绑定 Dropdown 改变事件
        signalDropdown1.onValueChanged.AddListener(OnDropdownValueChanged);
       
    }
    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        // useButton.onClick.AddListener(OnUse);
    }
   
    /// <summary>
    /// 外部调用：打开 UI 并传入当前点击的行程阀
    /// </summary>
    public  override void InitItenMenu()
    {
        base.InitItenMenu();
        currentValve = itemIns.GetComponent<RollerValve32>();
        RefreshDropdown(signalDropdown1);
      
    }

    /// <summary>
    /// 核心逻辑：扫描场景并更新 Dropdown
    /// </summary>
    private void RefreshDropdown(TMP_Dropdown dpd)
    {
        // 1. 找到场景中所有信号源
        availableSignals = Object.FindObjectsOfType<BaseSignalSource>().ToList();

        // 2. 清空并重新构建 Dropdown 选项
        dpd.ClearOptions();

        List<string> options = new List<string>();
        int currentIndex = -1;

        for (int i = 0; i < availableSignals.Count; i++)
        {
            options.Add(availableSignals[i].signalName);

            // 检查这个信号源是否是当前阀门已经绑定的那个
            if (currentValve.boundSignal == availableSignals[i])
            {
                currentIndex = i;
            }
        }

        dpd.AddOptions(options);

        // 3. 同步显示当前已绑定的项
        if (currentIndex != -1)
        {
            dpd.value = currentIndex;
            dpd.RefreshShownValue();
        }
    }

    private void OnDropdownValueChanged(int index)
    {
        if (currentValve != null && index < availableSignals.Count)
        {
            // 执行绑定
            currentValve.boundSignal = availableSignals[index];
            Debug.Log($"阀门 {currentValve.name} 已绑定信号源: {availableSignals[index].signalName}");
        }
    }

    void OnConfirm()
    {
        base.OnConfirm();
        RefreshDropdown(signalDropdown1);
        signalDropdown1.onValueChanged.Invoke(signalDropdown1.value);
        for (int i = 0; i < toggleGroups.Count; i++)
        {
            int program2 = GetSelectedToggleIndex(i);
            if (itemIns != null)
            { itemIns.GetComponent<FittingManager>().ChangeFittingState(i, program2); }

        }

    }
    public int GetSelectedToggleIndex(int i)
    {
        // 获取 ToggleGroup 中唯一处于 On 状态的 Toggle
        Toggle activeToggle = toggleGroups[i].ActiveToggles().FirstOrDefault();
        int index = 0;
        if (activeToggle != null)
        {
            // 根据名字或者层级索引来判断
            // 假设你的 Toggle 名字分别是 "Toggle1", "Toggle2", "Toggle3"
            string name = activeToggle.name;

            if (name.Contains("0"))
            {
                Debug.Log("输出：0");
                index = 0;
            }
            else if (name.Contains("1"))
            {
                Debug.Log("输出：2");
                index = 1;
            }
            else if (name.Contains("2"))
            {
                Debug.Log("输出：3");
                index = 2;
            }

        }
        return index;
    }
}
