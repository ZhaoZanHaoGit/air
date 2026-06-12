using TMPro;
using UnityEngine;

public class WorkOrderItem : MonoBehaviour
{
    private TMP_Text numberText, nameText, workconnectText, operationconnectText, standardText, judgeText;
    private void Awake()
    {
        numberText=transform.Find("number/Text (TMP)").GetComponent<TMP_Text>();
        nameText = transform.Find("name/Text (TMP)").GetComponent<TMP_Text>();
        workconnectText = transform.Find("workconnect/Text (TMP)").GetComponent<TMP_Text>();
        operationconnectText = transform.Find("operationconnect/Text (TMP)").GetComponent<TMP_Text>();
        standardText = transform.Find("standard/Text (TMP)").GetComponent<TMP_Text>();
        judgeText = transform.Find("judge/Text (TMP)").GetComponent<TMP_Text>();
    }
    public void Init(int _number, CheckItem data) {
        numberText.text=_number.ToString();
        nameText.text=data.itemName;
        switch (data.itemType)
        {
            case TicketValueType.checkbox:
                string connect = "";
                string s = "选项：";
                for (int i = 0; i < data.options.Count; i++) {
                    connect += data.options[i]+"  ";
                }
                workconnectText.text=s+connect;
                string selectAnswer = string.IsNullOrEmpty(data.selectedOption) ? "未选择" : data.selectedOption;
                standardText.text= data.options[int.Parse(data.normalAnswer)];
                if (selectAnswer == "未选择")
                {
                    judgeText.text = "错误";
                    operationconnectText.text = selectAnswer;
                    judgeText.color = Color.red;
                }
                else {
                    operationconnectText.text= data.options[int.Parse(selectAnswer)];
                }
                if (operationconnectText.text == standardText.text)
                {
                    judgeText.text = "正确";
                    judgeText.color=Color.black;
                }
                else {
                    judgeText.text = "错误";
                    judgeText.color = Color.red;
                }
                break;
            case TicketValueType.input:
                workconnectText.text = "输入数值";
                string answer = string.IsNullOrEmpty(data.inputValue) ? "未输入" : data.inputValue;
                operationconnectText.text = answer;
                switch (data.answerType)
                {
                    case AnswerType.标准值:
                        standardText.text = data.normalAnswer;
                        if (data.normalAnswer == "判定" && answer == "未输入")
                        {
                            judgeText.text = "错误";
                            judgeText.color = Color.red;
                        }
                        else if (data.normalAnswer == "判定" && answer != "未输入")
                        {
                            judgeText.text = "待判定";
                            judgeText.color = Color.black;
                        }
                        else
                        {
                            if (operationconnectText.text == standardText.text)
                            {
                                judgeText.text = "正确";
                                judgeText.color = Color.black;
                            }
                            else
                            {
                                judgeText.text = "错误";
                                judgeText.color = Color.red;
                            }
                        }

                        break;
                    case AnswerType.范围值:
                        standardText.text = "最小值:" + data.minRange + "  最大值:" + data.maxRange;
                        float min = 0;
                        float max = 0;
                        float select = 0;
                        bool isSuccessMin = float.TryParse(data.minRange, out min);
                        bool isSuccessMax = float.TryParse(data.maxRange, out max);
                        bool isSuccessSelect = float.TryParse(operationconnectText.text, out select);
                        if (isSuccessMin && isSuccessMax && isSuccessSelect)
                        {
                            if (select >= min && select <= max)
                            {
                                judgeText.text = "正确";
                                judgeText.color = Color.black;
                            }
                            else
                            {
                                judgeText.text = "错误";
                                judgeText.color = Color.red;
                            }
                        }
                        else {
                            judgeText.text = "错误";
                            judgeText.color = Color.red;
                        }
                        break;
                }
                break;
        }
    }
}
