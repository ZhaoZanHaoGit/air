
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.消息类
 *
 *  Author:				
 *       
 *  Date:              
 * 
 ******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message : IEnumerable<KeyValuePair<string, object>>
{
    public Dictionary<string, object> DicDatas;

    public string Name { get; }
    public object Sender { get; }
    public object Content { get; set; }

    public object this[string key]
    {
        get => null == DicDatas || !DicDatas.ContainsKey(key) ? null : DicDatas[key];
        set
        {
            if (ReferenceEquals(DicDatas, null))
            {
                DicDatas = new Dictionary<string, object>();
            }

            if (DicDatas.ContainsKey(key))
                DicDatas[key] = value;
            else
                DicDatas.Add(key, value);
        }
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        if (null == DicDatas)
            yield break;
        foreach (var kvp in DicDatas)
        {
            yield return kvp;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return DicDatas.GetEnumerator();
    }

    public Message(string name, object sender, object content = null, params object[] dicParams)
    {
        Name = name;
        Sender = sender;
        Content = content;
        foreach (var dicParam in dicParams)
        {
            if (dicParam.GetType() == typeof(Dictionary<string, object>))
            {
                foreach (var kvp in (Dictionary<string, object>) dicParam)
                    this[kvp.Key] = kvp.Value;
            }
            else
                Debug.LogWarningFormat("Warning: Message dicParam Type Is Not Dictionary. Type: {0}", dicParam.GetType());
        }
    }

    public Message(Message message)
    {
        Name = message.Name;
        Sender = message.Sender;
        Content = message.Content;
        foreach (var kvp in message.DicDatas)
            this[kvp.Key] = kvp.Value;
    }

    public void Add(string key, object value)
    {
        this[key] = value;
    }

    public void Remove(string key)
    {
        if (null != DicDatas && DicDatas.ContainsKey(key))
        {
            DicDatas.Remove(key);
        }
    }

    public void Send()
    {
        MessageCenter.Instance.SendMessage(this);
    }
}
