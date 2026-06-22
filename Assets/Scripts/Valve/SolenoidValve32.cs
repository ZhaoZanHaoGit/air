using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 阀门的物理初始特性：常闭型（断电不通） / 常开型（断电即通）
public enum ValveInitialType
{
    NormallyClosed, // 常闭型 (NC)
    NormallyOpen    // 常开型 (NO)
}

public class SolenoidValve32 : BaseValve
{
    [Header("电磁线圈配置")]
   
    [Header("物理特性配置")]
    [Tooltip("常开(NO)或常闭(NC)物理特性切换")]
    public ValveInitialType valveType = ValveInitialType.NormallyClosed;

    [Header("防扰动消噪设置")]
    [Tooltip("电磁线圈失电后的机械/磁滞恢复延时(秒)，防止临界点抖动")]
    public float debounceDuration = 0.05f;

    private float filterTimer = 0f;
    private bool isCoilEnergized = false; // 电磁线圈是否【有效通电】

    [Header("电磁线圈双接线柱配置")]
    public DynamicNode electricityPortA; // 接线柱 A (例如接24V)
    public DynamicNode electricityPortB; // 接线柱 B (例如接0V)

    void Update()
    {
        if (electricityPortA == null || electricityPortB == null) return;

        // 获取 A、B 两个端子当前在全网并查集中的通电极性
        NodeType statusA = electricityPortA.currentPowerStatus;
        NodeType statusB = electricityPortB.currentPowerStatus;

        // 核心物理判定：只有当两端子【同时通电】，且【一端是正极、一端是负极】时，才视为有电流通过线圈
        bool hasPower = (statusA == NodeType.Positive && statusB == NodeType.Negative) ||
                        (statusA == NodeType.Negative && statusB == NodeType.Positive);

        // 带有消噪的线圈状态机逻辑 (保持不变)
        if (hasPower)
        {
            isCoilEnergized = true;
            filterTimer = debounceDuration;
        }
        else
        {
            if (filterTimer > 0) filterTimer -= Time.deltaTime;
            else isCoilEnergized = false;
        }
    }

    public override void ProcessLogic()
    {
        if (ports.Count < 2) return;

        PneumaticPort portP = ports[0]; // 进气口
        PneumaticPort portA = ports[1]; // 工作出气口

        // 3. 核心多态映射：将【线圈状态】与【常开/常闭属性】转化为统一的【阀芯位置】
        // 判定阀芯当前是否应该处于“激活导通态”
        bool isValveActivated = false;

        if (valveType == ValveInitialType.NormallyClosed)
        {
            // 常闭阀：通电则激活导通，断电则关闭
            isValveActivated = isCoilEnergized;
        }
        else
        {
            // 常开阀：断电时默认导通（激活），通电时反而切断（关闭）
            isValveActivated = !isCoilEnergized;
        }

        // 4. 根据最终的阀芯位置（isValveActivated）执行对应的通道流体拓扑交换
        if (isValveActivated)
        {
            // 【通路状态】：P口与A口在内部手拉手
            portP.state = PortState.Conduct;
            portA.state = PortState.Conduct;

            portP.internalConnectTo = portA;
            portA.internalConnectTo = portP;
        }
        else
        {
            // 【断路状态】：P口与A口物理孤立截止
            portP.state = PortState.CutOff;
            portA.state = PortState.Conduct; // 保持工作口活跃，以便可以向排气口释放压力

            portP.internalConnectTo = null;
            portA.internalConnectTo = ports[2];
        }

        // 5. 驱动步骤：让各个活跃端口顺着内部纽带去获取彼此的内部压力/状态数据
        foreach (var p in ports)
        {
            p.ReceiveInternalInfo();
        }
    }
}