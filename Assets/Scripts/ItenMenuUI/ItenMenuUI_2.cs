using Palmmedia.ReportGenerator.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItenMenuUI_2 : ItenMenuUIBase
{
    public List<ToggleGroup> toggleGroups;
    public Button confirmButton;

    public Slider singal1SliderUI, singal2SliderUI;
    public Slider singal1Sliderthreshold, singal2Sliderthreshold;
    public TextMeshProUGUI UIsignal1percent, signal2percentUI;
    public TextMeshProUGUI UIsignal1threshold, signal2threshold;
    CylinderSignalSource signal1Source, signal2Source;

    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        // useButton.onClick.AddListener(OnUse);
        singal1SliderUI.onValueChanged.AddListener(
            (float a) =>
            {
                UIsignal1percent.text = ((int)(a * 100)).ToString() + "%";
            });
        singal2SliderUI.onValueChanged.AddListener(
            (float a) =>
            {
                signal2percentUI.text = ((int)(a * 100)).ToString() + "%";
            });
        singal1Sliderthreshold.onValueChanged.AddListener(
            (float a) =>
            {
                UIsignal1threshold.text ="±" +a.ToString("F2") ;
            });
        singal2Sliderthreshold.onValueChanged.AddListener(
            (float a) =>
            {
                signal2threshold.text = "±"+a.ToString("F2");
            });
    }
    public override void InitItenMenu()
    {
        base.InitItenMenu();
        if (itemIns != null)
        {
            signal1Source = itemIns.GetComponent<DoubleActingCylinder>().cySignal1;
            signal2Source = itemIns.GetComponent<DoubleActingCylinder>().cySignal2;

        }
    }



    public override void OnConfirm()
    {
        base.OnConfirm();

        for (int i = 0; i < toggleGroups.Count; i++)
        {
            int program2 = GetSelectedToggleIndex(i);
            if (itemIns != null)
            {
                itemIns.GetComponent<FittingManager>().ChangeFittingState(i, program2);


            }

        }
        signal1Source.signalName = tMP_InputField.text + "标尺1";
        signal1Source.percent =singal1SliderUI.value;
        signal1Source.threshold = singal1Sliderthreshold.value;
        signal2Source.signalName = tMP_InputField.text + "标尺2";
        signal2Source.percent =singal2SliderUI.value;
        signal2Source.threshold= singal2Sliderthreshold.value;

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
