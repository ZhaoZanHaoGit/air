using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItenMenuUIBase : MonoBehaviour
{
    protected Item3D item3D;
    public GameObject itemIns;
    public TMP_InputField tMP_InputField;

    virtual public void SetItem3D(Item3D item3D)
    {
        this.item3D = item3D;
    }
    virtual public Item3D GetItem3D()
    {
        return item3D;
    }
    virtual public void InitItenMenu()
    {
        tMP_InputField.text = itemIns.GetComponent<BaseValve>().valveName;
    }
    virtual public void OnConfirm()
    { itemIns.GetComponent<BaseValve>().valveName = tMP_InputField.text; }

    protected void LinkSliderToInputField(Slider slider, TMP_InputField inputField)
    {
        if (slider == null || inputField == null) return;

        inputField.text = slider.value.ToString("F2");

        slider.onValueChanged.AddListener((value) =>
        {
            inputField.text = value.ToString("F2");
        });

        inputField.onEndEdit.AddListener((text) =>
        {
            if (float.TryParse(text, out float result))
            {
                result = Mathf.Clamp(result, slider.minValue, slider.maxValue);
                slider.value = result;
                inputField.text = result.ToString("F2");
            }
            else
            {
                inputField.text = slider.value.ToString("F2");
            }
        });
    }

}
