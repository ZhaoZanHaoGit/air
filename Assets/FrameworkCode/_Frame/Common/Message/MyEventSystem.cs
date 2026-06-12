
using System;

public class MyEventSystem
{
    // 定义委托
    public delegate void MyEventHandler(int value);
    public delegate void MyEventHandlerWithoutArgs();
    public delegate void MyEventHandlerWithUpdateDataTime();

    // 创建事件
    public static event MyEventHandler OnValueChange;
    public static event MyEventHandlerWithoutArgs OnArgsEvent;
    public static event MyEventHandlerWithUpdateDataTime OnUpdateDataTimeEvent;
    // 触发事件的方法
    public static void TriggerEvent(int value)
    {
        OnValueChange?.Invoke(value);
    }
    public static void TriggerArgsEvent()
    {
        OnArgsEvent?.Invoke();
    }
    public static void TriggerUpdateDataTimeEvent()
    {
        OnUpdateDataTimeEvent?.Invoke();
    }
    public static int GetOnValueChangeHandlerCount()
    {
        if (OnValueChange != null)
        {
            return OnValueChange.GetInvocationList().Length;
        }
        else
        {
            return 0;
        }
    }
    public static void ClearAll() {
        OnValueChange = null;
    }
    public static void ClearArgsEvent() {
        OnArgsEvent = null;
    }
    public static void ClearUpdateDataTimeEvent()
    {
        OnUpdateDataTimeEvent = null;
    }
    public static bool ContainsMethodInEvent(MyEventHandler method)
    {
        // Get the list of delegates (event handlers) associated with the event
        Delegate[] delegates = OnValueChange?.GetInvocationList();

        if (delegates != null)
        {
            // Iterate through the list and check if the target method matches
            foreach (Delegate del in delegates)
            {
                if (del.Target == method.Target && del.Method == method.Method)
                {
                    return true; // Method found in the event
                }
            }
        }

        return false; // Method not found in the event
    }
}
