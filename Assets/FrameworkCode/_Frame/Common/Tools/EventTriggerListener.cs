
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.点击事件重写
 *
 *  Author:			
 *       
 *  Date:              
 * 
 ******************************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchHandle
{
    private event OnTouchEventHandle EventHandle;
    private object[] _handleargs;
    private Hashtable _hashtable;
    private float _waitTime = 0.5f;

    public TouchHandle() { }
    public TouchHandle(OnTouchEventHandle handle, Hashtable hashtable, params object[] args)
    {
        SetHandle(handle, hashtable, args);
    }

    public void SetHandle(OnTouchEventHandle handle, Hashtable hs, params object[] args)
    {
        DestroyHandle();
        _hashtable = hs;
        EventHandle += handle;
        _handleargs = args;
    }

    public void CallEventHandle(GameObject listener, object eventData)
    {
        var eventTrigger = listener.GetComponent<EventTriggerListener>();
        var audioName = string.Empty;
        if (!ReferenceEquals(_hashtable, null))
        {
            foreach (EnumHashtableParamsType type in _hashtable.Keys)
            {
                switch (type)
                {
                    case EnumHashtableParamsType.Audio:
                        audioName = _hashtable[type].ToString();
                        break;
                    case EnumHashtableParamsType.LockAllClick:
                        var strAllTime = _hashtable[EnumHashtableParamsType.LockAllClick].ToString();
                        _waitTime = string.IsNullOrEmpty(strAllTime) ? _waitTime : float.Parse(strAllTime);
                        CoroutineController.Instance.StartCoroutine(lockAllClick());
                        break;
                    case EnumHashtableParamsType.LockSelfClick:
                        var strSelfTime = _hashtable[EnumHashtableParamsType.LockSelfClick].ToString();
                        _waitTime = string.IsNullOrEmpty(strSelfTime) ? _waitTime : float.Parse(strSelfTime);
                        CoroutineController.Instance.StartCoroutine(lockSelfClick(eventTrigger));
                        break;
                }
            }
        }
        audioName = string.IsNullOrEmpty(audioName) ? "Buttonsound01" : audioName;
        AudioManager.Instance.PlaySoundEff2D($"Audio/ButtonAudio/{audioName}");
        EventHandle?.Invoke(listener, eventData, _handleargs);
    }

    private IEnumerator lockAllClick()
    {
        EventTriggerListener.IsLockAllClick = true;
        yield return new WaitForSeconds(_waitTime);
        EventTriggerListener.IsLockAllClick = false;
    }

    private IEnumerator lockSelfClick(EventTriggerListener eventTrigger)
    {
        eventTrigger.IsLockSelfClick = true;
        yield return new WaitForSeconds(_waitTime);
        eventTrigger.IsLockSelfClick = false;
    }

    public void DestroyHandle()
    {
        EventHandle = null;
    }

}

public class EventTriggerListener : EventTrigger
{
    private TouchHandle _onBeginDrag;
    private TouchHandle _onCancel;
    private TouchHandle _onDeselect;
    private TouchHandle _onDrag;
    private TouchHandle _onDrop;
    private TouchHandle _onEndDrag;
    private TouchHandle _onInitializePotentialDrag;
    private TouchHandle _onMove;
    private TouchHandle _onClick;
    private TouchHandle _onDoubleClick;
    private TouchHandle _onDown;
    private TouchHandle _onEnter;
    private TouchHandle _onExit;
    private TouchHandle _onUp;
    private TouchHandle _onScroll;
    private TouchHandle _onSelect;
    private TouchHandle _onSubmit;
    private TouchHandle _onUpdateSelected;

    public bool IngnoreClick = false;                                           //拖拽不响应Click


    public static EventTriggerListener Get(UnityEngine.UI.Selectable btn)
    {
        return Get(btn.gameObject);
    }

    public static EventTriggerListener Get(Transform transform)
    {
        return Get(transform.gameObject);
    }

    public static EventTriggerListener Get(GameObject go)
    {
        return go.GetOrAddComponent<EventTriggerListener>();
    }

    /// <summary>
    /// 设置添加的事件
    /// </summary>
    /// <param name="type">事件类型</param>
    /// <param name="handle">事件的委托</param>
    /// <param name="hashtable">事件的hashtable</param>
    /// <param name="args">可变参数</param>
    public void SetEventHandle(EnumTouchEventType type, OnTouchEventHandle handle, Hashtable hashtable = null, params object[] args)
    {
        switch (type)
        {
            case EnumTouchEventType.OnBeginDrag:
                if (null == _onBeginDrag)
                    _onBeginDrag = new TouchHandle();
                _onBeginDrag.SetHandle(handle, hashtable, args);
                break;
            case EnumTouchEventType.OnCancel:
                if (null == _onCancel)
                    _onCancel = new TouchHandle();
                _onCancel.SetHandle(handle, hashtable, args);
                break;
            case EnumTouchEventType.OnDeselect:
                if (null == _onDeselect)
                    _onDeselect = new TouchHandle();
                _onDeselect.SetHandle(handle, hashtable, args);
                break;
            case EnumTouchEventType.OnDrag:
                if (null == _onDrag)
                    _onDrag = new TouchHandle();
                _onDrag.SetHandle(handle, hashtable, args);
                break;
            case EnumTouchEventType.OnDrop:
                if (null == _onDrop)
                    _onDrop = new TouchHandle();
                _onDrop.SetHandle(handle, hashtable, args);
                break;
            case EnumTouchEventType.OnEndDrag:
                if (null == _onEndDrag)
                    _onEndDrag = new TouchHandle();
                _onEndDrag.SetHandle(handle, hashtable, args);
                break;
            case EnumTouchEventType.OnInitializePotentialDrag:
                if (null == _onInitializePotentialDrag)
                    _onInitializePotentialDrag = new TouchHandle();
                _onInitializePotentialDrag.SetHandle(handle, hashtable, args);
                break;
            case EnumTouchEventType.OnMove:
                if (null == _onMove)
                    _onMove = new TouchHandle();
                _onMove.SetHandle(handle, hashtable, args);
                break;
            case EnumTouchEventType.OnClick:
                if (null == _onClick)
                    _onClick = new TouchHandle();
                _onClick.SetHandle(handle, hashtable, args);
                break;
            case EnumTouchEventType.OnDoubleClick:
                if (null == _onDoubleClick)
                    _onDoubleClick = new TouchHandle();
                _onDoubleClick.SetHandle(handle, hashtable, args);
                break;
            case EnumTouchEventType.OnDown:
                if (null == _onDown)
                    _onDown = new TouchHandle();
                _onDown.SetHandle(handle, hashtable, args);
                break;
            case EnumTouchEventType.OnEnter:
                if (null == _onEnter)
                    _onEnter = new TouchHandle();
                _onEnter.SetHandle(handle, hashtable, args);
                break;
            case EnumTouchEventType.OnExit:
                if (null == _onExit)
                    _onExit = new TouchHandle();
                _onExit.SetHandle(handle, hashtable, args);
                break;
            case EnumTouchEventType.OnUp:
                if (null == _onUp)
                    _onUp = new TouchHandle();
                _onUp.SetHandle(handle, hashtable, args);
                break;
            case EnumTouchEventType.OnScroll:
                if (null == _onScroll)
                    _onScroll = new TouchHandle();
                _onScroll.SetHandle(handle, hashtable, args);
                break;
            case EnumTouchEventType.OnSelect:
                if (null == _onSelect)
                    _onSelect = new TouchHandle();
                _onSelect.SetHandle(handle, hashtable, args);
                break;
            case EnumTouchEventType.OnSubmit:
                if (null == _onSubmit)
                    _onSubmit = new TouchHandle();
                _onSubmit.SetHandle(handle, hashtable, args);
                break;
            case EnumTouchEventType.OnUpdateSelected:
                if (null == _onUpdateSelected)
                    _onUpdateSelected = new TouchHandle();
                _onUpdateSelected.SetHandle(handle, hashtable, args);
                break;
        }
    }

    private void RemoveAllHandle()
    {

        RemoveHandle(_onBeginDrag);
        RemoveHandle(_onCancel);
        RemoveHandle(_onDeselect);
        RemoveHandle(_onDrag);
        RemoveHandle(_onDrop);
        RemoveHandle(_onEndDrag);
        RemoveHandle(_onInitializePotentialDrag);
        RemoveHandle(_onMove);
        RemoveHandle(_onClick);
        RemoveHandle(_onDoubleClick);
        RemoveHandle(_onDown);
        RemoveHandle(_onEnter);
        RemoveHandle(_onExit);
        RemoveHandle(_onUp);
        RemoveHandle(_onScroll);
        RemoveHandle(_onSelect);
        RemoveHandle(_onSubmit);
        RemoveHandle(_onUpdateSelected);
    }

    private void RemoveHandle(TouchHandle handle)
    {
        if (null == handle) return;
        handle.DestroyHandle();
    }

    private void OnDestroy()
    {
        RemoveAllHandle();
    }

    public static bool IsLockAllClick;
    public static bool IsLoading;
    public bool IsLockSelfClick;

    /// <summary>
    /// 是否可点击
    /// </summary>
    /// <returns></returns>
    private bool canCallClick()
    {
        return enabled && !IsLockAllClick && !IsLockSelfClick && !IsLoading;
    }

    #region 重写事件系统
    public override void OnBeginDrag(PointerEventData eventData)
    {
        _onBeginDrag?.CallEventHandle(gameObject, eventData);
    }

    public override void OnCancel(BaseEventData eventData)
    {
        _onCancel?.CallEventHandle(gameObject, eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        _onDeselect?.CallEventHandle(gameObject, eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        _onDrag?.CallEventHandle(gameObject, eventData);
    }

    public override void OnDrop(PointerEventData eventData)
    {
        _onDrop?.CallEventHandle(gameObject, eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        _onEndDrag?.CallEventHandle(gameObject, eventData);
    }

    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        _onInitializePotentialDrag?.CallEventHandle(gameObject, eventData);
    }

    public override void OnMove(AxisEventData eventData)
    {
        _onMove?.CallEventHandle(gameObject, eventData);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!canCallClick()) return;
        if (null != _onClick && !IngnoreClick)
            _onClick.CallEventHandle(gameObject, eventData);
        if (null != _onDoubleClick && 2 == eventData.clickCount)
            _onDoubleClick.CallEventHandle(gameObject, eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        _onDown?.CallEventHandle(gameObject, eventData);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        _onEnter?.CallEventHandle(gameObject, eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        _onExit?.CallEventHandle(gameObject, eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        _onUp?.CallEventHandle(gameObject, eventData);
    }

    public override void OnScroll(PointerEventData eventData)
    {
        _onScroll?.CallEventHandle(gameObject, eventData);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        _onSelect?.CallEventHandle(gameObject, eventData);
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        _onSubmit?.CallEventHandle(gameObject, eventData);
    }

    public override void OnUpdateSelected(BaseEventData eventData)
    {
        _onUpdateSelected?.CallEventHandle(gameObject, eventData);
    }
    #endregion
}
