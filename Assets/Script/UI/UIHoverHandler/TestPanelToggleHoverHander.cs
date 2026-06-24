using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestPanelToggleHoverHander : UIHoverHandler
{
    public GameObject[] stateObjs;
    public TMP_Text text;
    private Toggle toggle;
    public Color[] colors;
    private void Awake()
    {
        toggle= GetComponent<Toggle>();
    }
    private void Start()
    {
        toggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                stateObjs[1].SetActive(true);
                text.color = colors[1];
            }
            else {
                stateObjs[1].SetActive(false);
                text.color = colors[0];
            }
            stateObjs[0].SetActive(false);
        });
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (toggle.isOn) return;
        stateObjs[0].SetActive(true);
        text.color=colors[1];
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (toggle.isOn) return;
        stateObjs[0].SetActive(false);
        text.color = colors[0];
    }
}
