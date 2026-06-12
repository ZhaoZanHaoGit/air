using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ItenMenuUI_1 : ItenMenuUIBase
{
    public Slider slider;
    public Button confirmButton;
    // Start is called before the first frame update
    void Start()
    {
        confirmButton.onClick.AddListener(OnConfirm);
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
        if (itemIns != null)
        {
            itemIns.GetComponent<ThrottleValve>().opening = slider.value;



        }

    }
}
