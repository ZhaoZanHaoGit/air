using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Globalization;
using dnlib.DotNet;

public class MainMenuButtonHoverHandler : UIHoverHandler
{
    public Transform jtTrans;
    public TMP_Text textColor;
    public Image line1,line2;
    public Color[] textcolors;
    public Color[] lineColors;
    private Transform myTrans;
    public Transform selectObj;
    public Transform hightObj;

    private void Awake()
    {
        myTrans = transform;
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        textColor.color = textcolors[1];
        line1.color = lineColors[1];
        line2.color = lineColors[1];
        jtTrans.DOScale(Vector3.one, 0.25f);
        myTrans.DOScale(new Vector3(1.02f, 1.02f, 1), 0.25f);
        selectObj.DOScaleX(0.8f, 0.25f);
        hightObj.GetComponent<Image>().DOFade(1f, 0.15f);
        hightObj.DOScaleX(1f, 0.25f);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        jtTrans.DOScale(Vector3.zero, 0.25f);
        textColor.color = textcolors[0];
        line1.color = lineColors[0];
        line2.color = lineColors[0];
        jtTrans.DOScale(Vector3.zero, 0.25f);
        myTrans.DOScale(new Vector3(1f, 1f, 1), 0.25f);
        selectObj.DOScaleX(0f, 0.25f);
        hightObj.GetComponent<Image>().DOFade(0f, 0.15f);
        hightObj.DOScaleX(0f, 0.25f);

    }
}
