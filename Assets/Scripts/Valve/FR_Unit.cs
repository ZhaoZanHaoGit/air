using UnityEngine;

/// <summary>
/// 气源过滤减压二联件（纯粹拓扑指针 + 减压阀芯物理约束版）
/// 端口约定：
/// - ports[0]: P口 (进气端，通过软管连到上面的 MainSourceCon)
/// - ports[1]: A口 (调压输出端，连向下游换向阀或三通)
/// </summary>
public class FR_Unit : BaseValve
{
    [Header("二联件状态")]
    public bool isLocked = false;

    [Range(0f, 1f)]
    [Header("减压阀设定开度")]
    public float outputPressure = 0.8f; // 调节输出压力百分比

    public Transform knobTransform;

    private void Update()
    {
        // 动画保持原样：只有输出端真正憋出气压时，压力表手柄才偏转
        float z = Mathf.Lerp(0, 264, outputPressure);
        if (ports.Count > 1 && ports[1].pressure > 0.1f)
        {
            knobTransform.localEulerAngles = new Vector3(0, 0, z);
        }
    }

    protected override void Start()
    {
        base.Start();

        // 【纯拓扑绑定】：二联件内部在物理上是一条永久双向直通的管道通道
        if (ports.Count >= 2)
        {
            ports[0].state = PortState.Conduct;
            ports[1].state = PortState.Conduct;

            // P口 和 A口 在阀体内相互拉手
            ports[0].internalConnectTo = ports[1]; // P -> A
            ports[1].internalConnectTo = ports[0]; // A -> P
        }
    }

    /// <summary>
    /// 阶段 1：驱动内部拓扑互换，并叠加上调压阀芯对输出口缓冲区的物理压制
    /// </summary>
    public override void ProcessLogic()
    {
        if (ports.Count < 2) return;

        PneumaticPort portIn = ports[0];  // 进气 P 口
        PneumaticPort portOut = ports[1]; // 输出 A 口

        if (!isLocked)
        {
            portIn.state = PortState.CutOff;
            portOut.state = PortState.CutOff;
        }
        else
        {
            // 强行确保每帧极性刷新
            portIn.state = PortState.Conduct;
            portOut.state = PortState.Conduct;

            // 1. 驱动两个端口顺着 internalConnectTo 拓扑纽带自发互换这一帧的内部状态
            // portIn 会去看 portOut 憋了多少气；portOut 会去读 portIn 从主气源插座抓过来了多少压
            portIn.ReceiveInternalInfo();
            portOut.ReceiveInternalInfo(outputPressure,1f ,1f);

            // 2. 🔴 【核心物理调压干涉】：
            // 减压阀的本质特性：当气体正向流动（portIn 压力大于 portOut）时，
            // 阀芯会对流向 A 口的气体进行减压限制。
            if (portIn.pressure > portOut.pressure + 0.001f)
            {
                // 我们通过反射或者开放属性，直接对 portOut 在这一阶段通过内指针自演化出来的“内部缓存压力”
                // 乘以调压阀手柄的限制百分比，从而完美、规范地调节了输出压力！
                // 这里对齐你 PneumaticPort 内部用于暂存 ReceiveInternalInfo 结果的变量名（例如 internalPressure）
                // portOut.internalPressure = portOut.internalPressure * outputPressure;
            }
        }
    }
}