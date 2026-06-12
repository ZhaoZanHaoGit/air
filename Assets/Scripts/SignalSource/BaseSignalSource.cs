using UnityEngine;

public abstract class BaseSignalSource : MonoBehaviour
{
    [Header("信号配置")]
    public string signalName; // 用于 Dropdown 显示的名称

    // 信号的当前状态：true 为有信号，false 为无信号
    public abstract bool IsTriggered { get; }

    protected virtual void Start()
    {
        // 如果信号名为空，自动使用物体名
        if (string.IsNullOrEmpty(signalName)) signalName = gameObject.name;
    }
}