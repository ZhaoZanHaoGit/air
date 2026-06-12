using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TipManager : MonoBehaviour
{
    public static TipManager instance;
    private TextMeshProUGUI tipText;
    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }
    public void ShowTip(string tipStr) {
        StartCoroutine(WaitHide(tipStr));
    }
    IEnumerator WaitHide(string tipStr) {
        gameObject.SetActive(true);
        if (!tipText) tipText = GetComponentInChildren<TextMeshProUGUI>();
        tipText.text= tipStr;
        yield return new WaitForSeconds(5);
        gameObject.SetActive(false);
    }
}
