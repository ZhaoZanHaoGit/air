
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.UI面板管理单例
 *
 *  Author:				
 *       
 *  Date:             
 * 
 ******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public struct UIInfoData
{
    public EnumUIType UIType { get; private set; }
    public string Path { get; private set; }
    public Type ScriptType { get; private set; }
    public object[] UIparams { get; private set; }
    public UIInfoData GetUIInfoData(EnumUIType uiType, string componentType, params object[] uiParams) {
        UIType = uiType;
        Path = UIPathDefines.GetPrefabPathByType(uiType, componentType);
        UIparams = uiParams;
        ScriptType = UIPathDefines.GetUIScriptByType(uiType, componentType);
        return this;
    }
}
public class UIManager : Singleton<UIManager>
{
    /// <summary>
    /// UI窗体信息
    /// </summary>


    private Dictionary<EnumUIType, GameObject> dicOpenUIs = null;
    private Stack<UIInfoData> stackOpenUIs = null;

    public override void Init() {
        dicOpenUIs = new Dictionary<EnumUIType, GameObject>();
        stackOpenUIs = new Stack<UIInfoData>();
    }

    public T GetUI<T>(EnumUIType uiType) where T : BaseUI {
        var retObj = GetUIObject(uiType);
        return null != retObj ? retObj.GetComponent<T>() : null;
    }

    public GameObject GetUIObject(EnumUIType uiType) {
        dicOpenUIs.TryGetValue(uiType, out var retObj);
        return retObj != null ? retObj : null;
        //if (dicOpenUIs.TryGetValue(uiType, out var retObj)) return retObj;
        //var msg = $"dicOpenUIs TryGetValue Failure! uiType :{uiType}";
        //throw new Exception(msg);
    }

    public void PreloadUI(EnumUIType[] uiTypes, string[] componentType) {
        var len = uiTypes.Length;
        if (len != componentType.Length) {
            Debug.LogErrorFormat($"uiTypes Length != componentType Length.uiTypes Length{len},componentType Length{componentType.Length}");
            return;
        }
        for (var i = 0; i < len; i++) {
            PreloadUI(uiTypes[i], componentType[i]);
        }
    }

    public void PreloadUI(EnumUIType[] uITypes) {
        var len = uITypes.Length;
        for (var i = 0; i < len; i++) {
            PreloadUI(uITypes[i]);
        }
    }

    /// <summary>
    /// 预加载
    /// </summary>
    /// <param name="uiType">UI类型</param>
    /// <param name="componentType">组件类型</param>
    public void PreloadUI(EnumUIType uiType, string componentType) {
        var path = UIPathDefines.GetPrefabPathByType(uiType, componentType);
        ResManager.Instance.LoadPrefab(path);
        //ResManager.Instance.LoadAsync<GameObject>(path, null);
    }

    /// <summary>
    /// 预加载
    /// </summary>
    /// <param name="uiType">UI类型</param>
    public void PreloadUI(EnumUIType uiType) {
        var path = UIPathDefines.GetPrefabPathByType(uiType, string.Empty);
        ResManager.Instance.LoadPrefab(path);
    }

    public void OpenMessageBoxUI(string content, int countTime = 10, EnumMessageBoxType type = EnumMessageBoxType.OK,
                                 MethodAction btnOK = null, object btnOKParam = null,
                                 MethodAction btnRelease = null, object btnReleaseParam = null,
                                 params object[] uiParams) {
        OpenMessageBoxUI(null, content, countTime, type, btnOK, btnOKParam, btnRelease, btnReleaseParam, uiParams);
    }

    //打开两个按钮的弹窗
    public void OpenMessageBoxUI(string title, string content, int countTime = 10, EnumMessageBoxType type = EnumMessageBoxType.OKCancel,
                                 MethodAction btnOK = null, object btnOKParam = null,
                                 MethodAction btnRelease = null, object btnReleaseParam = null,
                                 params object[] uiParams) {
        Debug.Log("弹出弹窗！");
        var module = ModuleManager.Instance.Get<MessageBoxModule>();
        module.Title = string.IsNullOrEmpty(title) ? "" : title;
        module.Content = content;
        module.CountTime = countTime;
        module.BtnOK = btnOK;
        module.BtnRelease = btnRelease;
        module.BtnOKParam = btnOKParam;
        module.BtnReleaseParam = btnReleaseParam;
        module.MessageType = type;
        OpenUI(EnumUIType.MessageBoxUI, uiParams);
    }

    /// <summary>
    /// 打开多个UI面板不关闭已打开的UI面板
    /// </summary>
    /// <param name="uITypes">打开面板的UI类型数组</param>
    public void OpenUI(EnumUIType[] uITypes) {
        OpenUI(false, uITypes, string.Empty, null);
    }

    /// <summary>
    /// 打开UI面板不关闭已打开的UI面板
    /// </summary>
    /// <param name="uiType">UI类型</param>
    /// <param name="uiParams">可变参数</param>
    public void OpenUI(EnumUIType uiType, params object[] uiParams) {
        var uiTypes = new EnumUIType[] { uiType };
        OpenUI(false, uiTypes, string.Empty, uiParams);
    }

    /// <summary>
    /// 打开UI面板不关闭已打开的UI面板
    /// </summary>
    /// <param name="uiType">UI类型</param>
    /// <param name="componentType">组件类型</param>
    /// <param name="uiParams">可变参数</param>
    public void OpenUI(EnumUIType uiType, string componentType, params object[] uiParams) {
        var uiTypes = new EnumUIType[] { uiType };
        OpenUI(false, uiTypes, componentType, uiParams);
    }

    /// <summary>
    /// 打开多个UI面板兵关闭其他面板
    /// </summary>
    /// <param name="uiTypes">打开面板的UI类型数组</param>
    public void OpenUICloseOthers(EnumUIType[] uiTypes) {
        OpenUI(true, uiTypes, string.Empty, null);
    }

    /// <summary>
    /// 打开多个UI面板兵关闭其他面板
    /// </summary>
    /// <param name="uiType">UI类型</param>
    /// <param name="uiParams">可变参数</param>
    public void OpenUICloseOthers(EnumUIType uiType, params object[] uiParams) {
        var uiTypes = new EnumUIType[] { uiType };
        OpenUI(true, uiTypes, string.Empty, uiParams);
    }

    /// <summary>
    /// 打开多个UI面板兵关闭其他面板
    /// </summary>
    /// <param name="uiType">UI类型</param>
    /// <param name="componentType">组件类型</param>
    /// <param name="uiParams">可变参数</param>
    public void OpenUICloseOthers(EnumUIType uiType, string componentType, params object[] uiParams) {
        var uiTypes = new EnumUIType[] { uiType };
        OpenUI(true, uiTypes, componentType, uiParams);
    }

    /// <summary>
    /// 打开UI面板
    /// </summary>
    /// <param name="isCloseOthers">是否关闭已打开的UI的面板</param>
    /// <param name="uiTypes">UI类型数组</param>
    /// <param name="componentType">组件类型</param>
    /// <param name="uiParams">可变参数</param>
    public void OpenUI(bool isCloseOthers, EnumUIType[] uiTypes, string componentType, params object[] uiParams) {
        if (isCloseOthers)
            CloseUIAll();
        foreach (var uiType in uiTypes) {
            if (!dicOpenUIs.ContainsKey(uiType))
                stackOpenUIs.Push(new UIInfoData().GetUIInfoData(uiType, componentType, uiParams));
        }
        if (stackOpenUIs.Count > 0) {
            CoroutineController.Instance.StartCoroutine(AsyncLoadData());
        }
    }

    private IEnumerator<int> AsyncLoadData() {
        if (!ReferenceEquals(stackOpenUIs, null) && stackOpenUIs.Count > 0) {
            do {
                var uiInfoData = stackOpenUIs.Pop();
                UnityEngine.Object prefabObj = ResManager.Instance.LoadPrefab(uiInfoData.Path);
                if (ReferenceEquals(prefabObj, null)) continue;
                var uiObj = UnityEngine.Object.Instantiate(prefabObj) as GameObject;
                if (uiObj != null) {
                    var baseUI = uiObj.GetComponent<BaseUI>() ?? uiObj.AddComponent(uiInfoData.ScriptType) as BaseUI;
                    //Debug.Log(baseUI.gameObject.name);
                    if (baseUI != null) baseUI.SetUIWhenOpening(uiInfoData.UIparams);
                }
                dicOpenUIs.Add(uiInfoData.UIType, uiObj);
            } while (stackOpenUIs.Count > 0);
        }
        yield return 0;
    }

    public void CloseUIAll() {
        var listKey = new List<EnumUIType>(dicOpenUIs.Keys);
        foreach (var type in listKey) {
            CloseUI(type);
        }
        dicOpenUIs.Clear();
    }

    public void CloseUI(EnumUIType[] uiTypes) {
        foreach (var type in uiTypes) {
            CloseUI(type);
        }
    }

    public void CloseUI(EnumUIType uiType) {
        var uiObj = GetUIObject(uiType);
        if (null == uiObj) {
            dicOpenUIs.Remove(uiType);
        }
        else {
            var baseUI = uiObj.GetComponent<BaseUI>();
            if (null == baseUI) {
                UnityEngine.Object.Destroy(uiObj);
                dicOpenUIs.Remove(uiType);
            }
            else {
                baseUI.StateChanged += CloseUIHandle;
                baseUI.Release();
            }
        }
    }

    public void CloseUIHandle(object sender, EnumObjectState newState, EnumObjectState oldState) {
        if (newState != EnumObjectState.Closing) return;
        var baseUI = sender as BaseUI;
        if (baseUI == null) return;
        dicOpenUIs.Remove(baseUI.GetUIType());
        baseUI.StateChanged -= CloseUIHandle;
    }

    //获得所有的打开的面板
    public Dictionary<EnumUIType, GameObject> GetDicOpenUIs() {
        return dicOpenUIs;
    }

    //获取最上层的UI面板
    public EnumUIType GetCurrentUI() {
        var curUIType = EnumUIType.None;
        if (dicOpenUIs.Count > 0)
            curUIType = dicOpenUIs.Last().Key;
        return curUIType;
    }

    //打开的UI面板中可有此类型的UI
    public bool FindUIByUIType(EnumUIType uiType) {
        return dicOpenUIs.ContainsKey(uiType);
    }
}
