using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpDownLoadMessage : MonoBehaviour
{
    private TMP_Text messageText;
    public static UpDownLoadMessage _instance;
    private TMP_Text numberText;
    private Image fillAmount;
    private GameObject maskObj;

    private void Awake()
    {
        _instance = this;
        numberText = transform.Find("bg/number").GetComponent<TMP_Text>();
        fillAmount = transform.Find("bg/fillamount").GetComponent<Image>();
        maskObj = transform.Find("bg").gameObject;
        messageText = transform.Find("bg/message").GetComponent<TMP_Text>();
    }
    public void Show(string str)
    {
        maskObj.SetActive(true);
        messageText.text = str;
    }
    public void ChanagedValue(float progress) {
        numberText.text = (progress*100).ToString("0") + "%";
        fillAmount.fillAmount = progress;
    }
    public void Hide()
    {
        messageText.text = "";
        numberText.text = "";
        fillAmount.fillAmount = 0;
        maskObj.SetActive(false);
    }
}
