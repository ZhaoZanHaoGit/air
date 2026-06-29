using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class ItenMenuUI_1 : ItenMenuUIBase
{
    public Slider slider;
    public Button confirmButton;
    public TextMeshProUGUI percentText;
    // Start is called before the first frame update
    void Start()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        slider.onValueChanged.AddListener((value) =>
        {
            percentText.text = $"{(int)(value * 100)}%";
        });
    }
    public override void InitItenMenu()
    {
        base.InitItenMenu();
        if (itemIns != null)
        {
            slider.value = itemIns.GetComponent<ThrottleValve>().opening;
        }
    }
    void OnConfirm()
    {
        base.OnConfirm();
        if (itemIns != null)
        {
            itemIns.GetComponent<ThrottleValve>().opening = slider.value;



        }

    }
}
