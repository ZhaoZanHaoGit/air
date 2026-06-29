using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ItenMenuUI_3 : ItenMenuUIBase
{
    public List<ToggleGroup> toggleGroups;
    public Button confirmButton, useButton;


    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirm);
       // useButton.onClick.AddListener(OnUse);
    }

    void OnUse()
    {
        if (itemIns != null)
        {
            itemIns.GetComponent<ManualValve32>().isPressed = !itemIns.GetComponent<ManualValve32>().isPressed;

        }
    }

    void OnConfirm()
    {
        base.OnConfirm();
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
