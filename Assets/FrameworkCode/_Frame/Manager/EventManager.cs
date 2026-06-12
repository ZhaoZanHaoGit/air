using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
// **********************************************************
// 功能:事件管理器
// 创建:
// 时间:
// 修改内容:                 修改者姓名:
// **********************************************************
public interface IEventInfo { }
public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> action;
}
public class EventInfo : IEventInfo
{
    public UnityAction action;
}
public class EventManager :Singleton<EventManager>
{
    //private static EventManager instance;
    //public static EventManager Instance
    //{
    //    get
    //    {
    //        if (instance == null) instance = new EventManager();
    //        return instance;
    //    }
    //}
    public Dictionary<string, IEventInfo> actionDic = new Dictionary<string, IEventInfo>();
    #region 添加事件
    /// <summary>
    /// 添加事件无泛型
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void AddEventListener(string name,UnityAction action) {

        if (actionDic.ContainsKey(name))
        {
            (actionDic[name]as EventInfo).action += action;
        }
        else {
            actionDic.Add(name, new EventInfo(){ action = action });
        }
    }
    /// <summary>
    /// 添加事件泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void AddEventListener<T>(string name, UnityAction<T> action)
    {

        if (actionDic.ContainsKey(name))
        {
            (actionDic[name] as EventInfo<T>).action += action;
        }
        else
        {
            actionDic.Add(name, new EventInfo<T>() { action = action });
        }
    }
    #endregion
    #region 触发事件
    /// <summary>
    /// 触发事件无泛型
    /// </summary>
    /// <param name="name"></param>
    public void TriggerEventListener(string name)
    {
        if (actionDic.ContainsKey(name))
        {
            if ((actionDic[name] as EventInfo).action != null) (actionDic[name] as EventInfo).action.Invoke();         
        }
    }
    /// <summary>
    /// 触发事件泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="par"></param>
    public void TriggerEventListener<T>(string name,T par)
    {
        if (actionDic.ContainsKey(name))
        {
            if ((actionDic[name] as EventInfo<T>).action != null) (actionDic[name] as EventInfo<T>).action.Invoke(par);
        }
    }
    #endregion
    #region 移除事件
    /// <summary>
    /// 移除事件无泛型
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void RemoveEventListener(string name, UnityAction action)
    {
        if (actionDic.ContainsKey(name))
        {
            (actionDic[name]as EventInfo).action -= action;
        }       
    }
    /// <summary>
    /// 移除事件泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (actionDic.ContainsKey(name))
        {
            (actionDic[name] as EventInfo<T>).action -= action;
        }
    }
    #endregion
    #region 清空事件
    /// <summary>
    /// 清空事件
    /// </summary>
    public void ClearEventListener()
    {
        actionDic.Clear();
    }
    #endregion


    //列：
    //EventManager.Instance.AddEventListener("事件名字",Action);
    //EventManager.Instance.AddEventListener<string>("事件名字",Action);
    //public void Action() { }
    //public void Action(string s) { }
}
