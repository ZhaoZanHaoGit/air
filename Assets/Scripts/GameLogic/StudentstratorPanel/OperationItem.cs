using TMPro;
using UnityEngine;

public class OperationItem : MonoBehaviour
{
    private TMP_Text numberText, stepnameText, timeText;
    private void Awake()
    {
        numberText=transform.Find("number/Text (TMP)").GetComponent<TMP_Text>();
        stepnameText = transform.Find("stepName/Text (TMP)").GetComponent<TMP_Text>();
        timeText = transform.Find("time/Text (TMP)").GetComponent<TMP_Text>();
    }
    public void Init(int _number, StepInfo data) {
        numberText.text = _number.ToString();
        string info = data.StepName;
        string errorstr = "Error|";
        if (info.Contains(errorstr))
        {
            string result = info.Replace(errorstr, "");
            stepnameText.text = result;
            stepnameText.color = Color.red;
        }
        else {
            stepnameText.text = data.StepName;
            stepnameText.color = Color.black;
        }
        //stepnameText.text = data.StepName;
        timeText.text=data.CreatTime.ToString();
    }
}
