using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItenMenuUI_0 : ItenMenuUIBase
{
    public Toggle toggle;
    public TextMeshProUGUI percent;
    public Slider slider;
    public Button confirmButton;
    // Start is called before the first frame update
    void Start()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        slider.onValueChanged.AddListener((value) =>
        {
            percent.text = (value ).ToString("F2") ;
        });
    }
    public override void InitItenMenu()
    {
        base.InitItenMenu();
        if (itemIns != null)
        {
            toggle.isOn = itemIns.GetComponent<FR_Unit>().isLocked;
            slider.value = itemIns.GetComponent<FR_Unit>().outputPressure;
        }
    }
   public override void OnConfirm()
    {
        base.OnConfirm();
        if (itemIns != null)
        {
            itemIns.GetComponent<FR_Unit>().isLocked = toggle.isOn;
            itemIns.GetComponent<FR_Unit>().outputPressure = slider.value;


        }

    }
}
