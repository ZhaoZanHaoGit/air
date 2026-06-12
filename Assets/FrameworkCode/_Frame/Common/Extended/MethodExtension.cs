
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.方法扩展类
 *
 *  Author:				
 *       
 *  Date:              
 * 
 ******************************************************************************/

using UnityEngine;

public static class MethodExtension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        var result = go.GetComponent<T>();
        if (null == result)
            result = go.AddComponent<T>();
        return result;
    }

    public static T GetOrAddComponent<T>(this Transform transform) where T : Component
    {
        return GetOrAddComponent<T>(transform.gameObject);
    }

    public static T GetOrAddComponent<T>(this Component component) where T : Component
    {
        return GetOrAddComponent<T>(component.gameObject);
    }
}
