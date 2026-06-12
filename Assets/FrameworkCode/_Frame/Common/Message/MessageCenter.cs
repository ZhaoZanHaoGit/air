
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.消息处理中心
 *
 *  Author:				
 *       
 *  Date:              
 * 
 ******************************************************************************/

using System.Collections.Generic;
public class MessageCenter : Singleton<MessageCenter>
{
    private Dictionary<string, List<MessageEvent>> _dicMsgEvents;

    public override void Init() => _dicMsgEvents = new Dictionary<string, List<MessageEvent>>();

    public void AddListener(string messageName, MessageEvent messageEvent)
    {
        List<MessageEvent> list;
        if (_dicMsgEvents.ContainsKey(messageName))
            list = _dicMsgEvents[messageName];
        else
        {
            list = new List<MessageEvent>();
            _dicMsgEvents.Add(messageName, list);
        }
        if (!list.Contains(messageEvent))
            list.Add(messageEvent);
    }

    public void RemoveListener(string messageName, MessageEvent messageEvent)
    {
        if (!_dicMsgEvents.ContainsKey(messageName)) return;
        var list = _dicMsgEvents[messageName];
        if (list.Contains(messageEvent))
        {
            list.Remove(messageEvent);
        }
        if (list.Count <= 0)
        {
            _dicMsgEvents.Remove(messageName);
        }
    }

    public void RemoveAllListener()
    {
        _dicMsgEvents.Clear();
    }

    public void SendMessage(Message message)
    {
        DoMessageDispatcher(message);
    }

    public void SendMessage(string name, object sender, object content = null, params object[] dicParams)
    {
        DoMessageDispatcher(new Message(name, sender, content, dicParams));
    }

    private void DoMessageDispatcher(Message message)
    {
        if (null == _dicMsgEvents || !_dicMsgEvents.ContainsKey(message.Name))
            return;
        var list = _dicMsgEvents[message.Name];
        foreach (var t in list)
            t?.Invoke(message);
    }
}
