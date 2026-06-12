
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.UI界面通过类型获取
 *
 *  Author:				
 *       
 *  Date:             
 * 
 ******************************************************************************/

using System;
using UnityEngine;

public class UIPathDefines
{
    /// <summary>
    /// 通过UI类型/给定的字符串获取预制体地址
    /// </summary>
    /// <param name="uiType">UI类型</param>
    /// <param name="componentType">给定的字符串</param>
    /// <returns></returns>
    public static string GetPrefabPathByType(EnumUIType uiType, string componentType)
    {
        var prefabName = string.IsNullOrEmpty(componentType) ? uiType.ToString() : componentType;
        var path = $"{Defines.UIPREFAB}{prefabName}";
        var msg = string.Empty;
        if (uiType == EnumUIType.None)
            msg = string.Format($"没有该类型的预制:{uiType}");
        if (!string.IsNullOrEmpty(msg))
            Debug.LogWarning(msg);
        return path;
    }

    /// <summary>
    /// 通过UI类型/给定的字符串获取脚本类型
    /// </summary>
    /// <param name="uiType">UI类型</param>
    /// <param name="componentType">给定的字符串</param>
    /// <returns></returns>
    public static Type GetUIScriptByType(EnumUIType uiType, string componentType)
    {
        var msg = string.Empty;
        var scriptType = Type.GetType(string.IsNullOrEmpty(componentType) ? uiType.ToString() : componentType);
        if (uiType == EnumUIType.None)
            msg = $"没有该类型对应的脚本:{uiType}";
        if (!string.IsNullOrEmpty(msg))
            Debug.LogWarning(msg);
        return scriptType;
    }

}
