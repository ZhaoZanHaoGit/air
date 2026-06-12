
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.常用静态方法
 *
 *  Author:				
 *       
 *  Date:             
 * 
 ******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnityHelper : MonoBehaviour
{
    /// <summary>
    /// 是否点击在UI控件上，用于判断UI点穿
    /// </summary>
    /// <returns>返回是否点穿</returns>
    public static bool IsPointerOverUIObject()
    {
        if (EventSystem.current == null)
            return false;
        var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        var count = results.Count - 1;
        for (var i = count; i >= 0; i--)
        {
            if (results[i].gameObject.layer != 5)
                results.RemoveAt(i);
        }
        return results.Count > 0;
    }


    /// <summary>
    /// 查找子对象
    /// </summary>
    /// <param name="goParent">父对象</param>
    /// <param name="childName">子对象名称</param>
    /// <returns></returns>
    public static Transform FindTheChild(GameObject goParent, string childName)
    {
        var searchTrans = goParent.transform.Find(childName);
        if (!ReferenceEquals(searchTrans, null)) return searchTrans;
        foreach (Transform trans in goParent.transform)
        {
            searchTrans = FindTheChild(trans.gameObject, childName);
            if (!ReferenceEquals(searchTrans, null))
            {
                return searchTrans;
            }
        }
        return null;
    }

    /// <summary>
    /// 查找子对象
    /// </summary>
    /// <param name="goParent">父对象</param>
    /// <param name="childName">子对象名称</param>
    /// <returns></returns>
    public static Transform FindTheChild(Transform goParent, string childName)
    {
        return FindTheChild(goParent.gameObject, childName);
    }


    /// <summary>
    /// 获取子物体的脚本
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    /// <param name="goParent">父对象</param>
    /// <param name="childName">子对象名称</param>
    /// <returns></returns>
    public static T GetTheChildComponent<T>(GameObject goParent, string childName) where T : Component
    {
        var searchTrans = FindTheChild(goParent, childName);
        return searchTrans?.GetOrAddComponent<T>();
    }/// <summary>
     /// 获取子物体的脚本
     /// </summary>
     /// <typeparam name="T">泛型</typeparam>
     /// <param name="goParent">父对象</param>
     /// <param name="childName">子对象名称</param>
     /// <returns></returns>
    public static T GetTheChildComponent<T>(Transform goParent, string childName) where T : Component
    {
        return GetTheChildComponent<T>(goParent.gameObject, childName);
    }

    /// <summary>
    /// 清除父节点下所有子节点
    /// </summary>
    /// <param name="trans">父节点</param>
    public static void ClearChildren(Transform trans)
    {
        if (ReferenceEquals(trans, null)) return;
        for (var i = 0; i < trans.childCount; i++)
        {
            Destroy(trans.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 秒数转换成时间
    /// </summary>
    /// <param name="time">秒数</param>
    /// <param name="format">时间格式</param>
    /// <returns></returns>
    public static string SecondConvertToString(int time, string format = "HH:mm:ss")
    {
        var ts = new TimeSpan(0, 0, time);
        return Convert.ToDateTime(ts.ToString()).ToString(format);
    }

    /// <summary>
    /// 获取字符串中第一个字母
    /// </summary>
    /// <param name="validString">该字符串</param>
    /// <returns></returns>
    public static int GetFirstLetter(string validString)
    {
        var bytes = System.Text.Encoding.Default.GetBytes(validString);
        var pos = 0;
        for (var i = 0; i < validString.Length; i++)
        {

            var by = bytes[i];
            if (((by >= 65) && (by <= 90)) || ((by >= 97) && (by <= 122)))
            {
                break;
            }
            pos++;
        }
        return pos;
    }

    /// <summary>
    /// 根据传进来的参数创建一个hashtable
    /// </summary>
    /// <param name="args">成对可变参数</param>
    /// <returns></returns>
    public static Hashtable CreateHashtable(params object[] args)
    {
        Hashtable hashtable = null;
        if (args.Length % 2 == 0)
        {
            hashtable = new Hashtable(args.Length / 2);
            var i = 0;
            while (i < args.Length - 1)
            {
                hashtable.Add(args[i], args[i + 1]);
                i += 2;
            }
        }
        else
            Debug.LogError("Hashtable Error: Hash requires an even number of arguments!");
        return hashtable;
    }
}
