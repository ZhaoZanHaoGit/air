using UnityEngine;

/// <summary>
/// 真正的主气源插座/空压机接口（绝对边界注入器）
/// 只有一个端口：ports[0] (P口)
/// </summary>
public class MainSourceCon : BaseValve
{
    [Header("气源开关")]
    public bool isGasValveOpen = true; // 模拟墙上总气阀是否打开

    protected override void Start()
    {
        base.Start();
        if (ports.Count > 0)
        {
            ports[0].state = PortState.Conduct;
        }
    }

    /// <summary>
    /// 阶段 1：作为全网络能量的始发站，注入绝对边界
    /// </summary>
    public override void ProcessLogic()
    {
       
    }
}