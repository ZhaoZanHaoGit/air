using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单电控二位五通电磁阀
/// 端口排列（Inspector中按顺序配置 5 个 PneumaticPort）：
///   [0] P  进气口
///   [1] A  输出口A
///   [2] B  输出口B
///   [3] R  排气口R
///   [4] S  排气口S
///
/// 工作逻辑（常闭型 NC）：
///   断电（弹簧复位）：P→A 导通，B→S 排气，R 截止  （第一位）
///   通电（线圈吸合）：P→B 导通，A→R 排气，S 截止  （第二位）
///
/// 常开型 NO 逻辑相反：
///   断电：P→B 导通，A→R 排气，S 截止
///   通电：P→A 导通，B→S 排气，R 截止
///
/// 消噪滤波：
///   电磁线圈失电后有机械/磁滞恢复延时，防止 SimulationLoop 重置时
///   瞬间断电导致的阀位抖动。与 SolenoidValve32 / ManualValve32 逻辑一致。
/// </summary>
public class SolenoidValve52 : BaseValve
{
    // ========== 端口索引常量 ==========
    private const int IDX_P = 0;
    private const int IDX_A = 1;
    private const int IDX_B = 2;
    private const int IDX_R = 3;
    private const int IDX_S = 4;
    private const int REQUIRED_PORT_COUNT = 5;

    // ========== Inspector 配置 ==========
    [Header("物理特性配置")]
    [Tooltip("常闭(NC)：断电弹簧复位到P→A位；常开(NO)：断电默认在P→B位")]
    public ValveInitialType valveType = ValveInitialType.NormallyClosed;

    [Header("防扰动消噪设置")]
    [Tooltip("电磁线圈失电后的机械/磁滞恢复延时(秒)，防止临界点抖动")]
    public float debounceDuration = 0.05f;

    [Header("阀芯状态（只读）")]
    [Tooltip("false = 弹簧复位位（第一位），true = 线圈吸合位（第二位）")]
    public bool valvePosition = false;

    [Header("电磁线圈双接线柱配置")]
    public DynamicNode electricityPortA; // 接线柱 A (例如接24V)
    public DynamicNode electricityPortB; // 接线柱 B (例如接0V)

    // ========== 消噪滤波器内部状态 ==========
    private float filterTimer = 0f;
    private bool isCoilEnergized = false; // 电磁线圈是否【有效通电】

    // ========== Unity 生命周期 ==========
    protected override void Start()
    {
        base.Start();
        foreach (var p in ports)
            p.state = PortState.Conduct;
    }

    private void Update()
    {
        if (electricityPortA == null || electricityPortB == null) return;

        // 获取 A、B 两个端子当前在全网并查集中的通电极性
        NodeType statusA = electricityPortA.currentPowerStatus;
        NodeType statusB = electricityPortB.currentPowerStatus;

        // 核心物理判定：只有当两端子【同时通电】，且【一端是正极、一端是负极】时，才视为有电流通过线圈
        bool hasPower = (statusA == NodeType.Positive && statusB == NodeType.Negative) ||
                        (statusA == NodeType.Negative && statusB == NodeType.Positive);

        // ============================================================
        // 消噪滤波器（与 SolenoidValve32 逻辑一致）
        //   通电 → 立即生效，定时器满格重置
        //   断电 → 不立即失效，定时器倒计时结束后才确认失电
        // 这样即使 SimulationLoop 重置导致瞬间断电，只要断开时间短于
        // debounceDuration，阀门就不会误切换。
        // ============================================================
        if (hasPower)
        {
            isCoilEnergized = true;
            filterTimer = debounceDuration;
        }
        else
        {
            if (filterTimer > 0f)
                filterTimer -= Time.deltaTime;
            else
                isCoilEnergized = false;
        }
    }

    // ========== 气路逻辑 ==========
    public override void ProcessLogic()
    {
        if (ports.Count < REQUIRED_PORT_COUNT)
        {
            Debug.LogWarning(
                $"SolenoidValve52 [{name}]：需要 5 个端口（P,A,B,R,S），" +
                $"当前仅配置了 {ports.Count} 个。请在 Inspector 中检查 ports 列表！");
            return;
        }

        // 清空上一帧的所有内部连接
        for (int i = 0; i < ports.Count; i++)
            ports[i].internalConnectTo = null;

        // 将【线圈状态】与【常开/常闭属性】转化为统一的【阀芯位置】
        bool isValveActivated;
        if (valveType == ValveInitialType.NormallyClosed)
        {
            // 常闭阀：通电→切换到第二位（P→B），断电→弹簧复位到第一位（P→A）
            isValveActivated = isCoilEnergized;
        }
        else
        {
            // 常开阀：断电→默认在第二位（P→B），通电→切换到第一位（P→A）
            isValveActivated = !isCoilEnergized;
        }

        valvePosition = isValveActivated;

        if (!isValveActivated)
        {
            // ============================================================
            // 第一位（弹簧复位位）：P→A 导通，B→S 排气，R 截止
            // ============================================================
            ports[IDX_A].internalConnectTo = ports[IDX_P];  // A 从 P 取压
            ports[IDX_B].internalConnectTo = ports[IDX_S];  // B 排气到 S（S 通大气，压力≈0）
            // R(ports[3]) 不连接 → 截止
        }
        else
        {
            // ============================================================
            // 第二位（线圈吸合位）：P→B 导通，A→R 排气，S 截止
            // ============================================================
            ports[IDX_B].internalConnectTo = ports[IDX_P];  // B 从 P 取压
            ports[IDX_A].internalConnectTo = ports[IDX_R];  // A 排气到 R（R 通大气，压力≈0）
            // S(ports[4]) 不连接 → 截止
        }

        // 驱动所有端口刷新内部压力信息
        foreach (var port in ports)
            port.ReceiveInternalInfo();
    }
}
