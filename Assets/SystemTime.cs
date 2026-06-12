using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SystemTime : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI timeText;
    void Start()
    {
        InvokeRepeating( "UpdateTime", 0, 1);
    }

    // Update is called once per frame
    void UpdateTime()
    {
        string dateTimeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        timeText.text = dateTimeStr;
    }
}
