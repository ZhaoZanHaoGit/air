using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class caseBtn : MonoBehaviour
{
    public TextMeshProUGUI num;
    public TextMeshProUGUI text;
    public void UpdateBtnDData(string numText, string textText)
    {
        num.text = numText;
        text.text = textText;
    }
}
