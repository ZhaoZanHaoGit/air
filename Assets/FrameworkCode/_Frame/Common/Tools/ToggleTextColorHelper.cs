using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleTextColorHelper : MonoBehaviour
{
    private Toggle _toggle;
    public Color normalcolor,selectcolor;
    private TMP_Text text;
    public TMP_Text text1;
    public Image iconimage;
    public GameObject imageObj;
    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(onToggleValueChanged);
        text = transform.Find("name").GetComponent<TMP_Text>();
        text.color = _toggle.isOn == true ? selectcolor : normalcolor;
    }

    private void onToggleValueChanged(bool isOn)
    {
        text.color = isOn == true ? selectcolor : normalcolor;
        if(text1!=null)
            text1.color = isOn == true ? selectcolor : normalcolor;
        if (iconimage!=null)
            iconimage.color= isOn == true ? selectcolor : normalcolor;
        if (imageObj != null)
            imageObj.SetActive(isOn);
    }
    public void SetToggleValue(bool value)
    {
        text.color = value == true ? selectcolor : normalcolor;
    }
}
