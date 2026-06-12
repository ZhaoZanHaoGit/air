
/******************************************************************************
 * 
 *  Title:				
 *
 *  Version:			
 *
 *  Description:
 *  1.通用弹窗
 *
 *  Author:			
 *       
 *  Date:	           
 * 
 ******************************************************************************/
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxUI : BaseUI
{
    protected override int UIOrder => 130;
    // public override bool IsOnlyClose => true; 

    private MessageBoxModule _module;
    public MessageBoxModule Module => _module ??= ModuleManager.Instance.Get<MessageBoxModule>();

    #region UI组件
    private TMP_Text _txtTitle;
    private TMP_Text TxtTitle => _txtTitle ??= UnityHelper.GetTheChildComponent<TMP_Text>(CacheGameObject, "txtTitle");
    private TMP_Text _txtContent;
    public TMP_Text TxtContent => _txtContent ??= UnityHelper.GetTheChildComponent<TMP_Text>(CacheGameObject, "txtContent");
    private TMP_Text _txtCountdown;
    public TMP_Text TxtCountdown => _txtCountdown ??= UnityHelper.GetTheChildComponent<TMP_Text>(CacheGameObject, "txtCountdown");
    private Button _btnOK;
    public Button BtnOK => _btnOK ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btnOK");

    private Button _btnRelease;
    public Button btnRelease => _btnRelease ??= UnityHelper.GetTheChildComponent<Button>(gameObject, "btnRelease");

    //private Button _btnOnlyOK;
    //public Button BtnOnlyOK => _btnOnlyOK ??= UnityHelper.GetTheChildComponent<Button>(CacheGameObject, "btnOnlyOK");
    #endregion

    private bool _isConfirm;

    public override EnumUIType GetUIType()
    {
        return EnumUIType.MessageBoxUI;
    }

    protected override void OnStart()
    {
        initMessageBox();
    }

    private void initMessageBox()
    {
        StopAllCoroutines();
        TxtTitle.text = Module.Title;
        TxtContent.text = Module.Content;
        RectTransform Rect = TxtContent.transform.parent.GetComponent<RectTransform>();
        if (Rect != null)
        {
            // 强行重建当前物体以及所有子物体的布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
        }
        switch (Module.MessageType)
        {
            case EnumMessageBoxType.OK:
                BtnOK.gameObject.SetActive(true);
                EventTriggerListener.Get(BtnOK.transform).SetEventHandle(EnumTouchEventType.OnClick, onBtnOK);
                break;
            case EnumMessageBoxType.OKCancel:
                BtnOK.gameObject.SetActive(true);
                btnRelease.gameObject.SetActive(true);
                EventTriggerListener.Get(BtnOK.transform).SetEventHandle(EnumTouchEventType.OnClick, onBtnOK);
                EventTriggerListener.Get(btnRelease.transform).SetEventHandle(EnumTouchEventType.OnClick, onBtnRelease);
                break;
        }
        if (Module.CountTime > 0)
            StartCoroutine(countTime());
    }


    private IEnumerator countTime()
    {
        TxtCountdown.gameObject.SetActive(true);
        while (Module.CountTime > 0)
        {
            TxtCountdown.text = string.Format($"({Module.CountTime}秒过后自动关闭)");
            yield return new WaitForSeconds(1.0f);
            Module.CountTime--;
        }
        CloseUI();
    }

    protected override void OnRelease()
    {
        Module.Send(_isConfirm);
    }

    private void onBtnOK(GameObject gameObject, object eventData, params object[] args)
    {
        _isConfirm = true;
        CloseUI();
    }


    private void onBtnRelease(GameObject gameObject, object eventData, params object[] args)
    {
        _isConfirm = false;
        CloseUI();
    }
}
