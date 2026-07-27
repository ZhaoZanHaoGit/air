using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 双电控三位五通换向阀（O型中位）
/// 端口排列（Inspector中按顺序配置 5 个 PneumaticPort）：
///   [0] P  进气口
///   [1] A  输出口A
///   [2] B  输出口B
///   [3] R  排气口R
///   [4] S  排气口S
///
/// 工作逻辑：
///   左侧电磁铁得电：P→A 导通，B→S 排气，R 截止  （左位）
///   右侧电磁铁得电：P→B 导通，A→R 排气，S 截止  （右位）
///   两侧均无信号：所有口截止                    （O型中位）
///
/// 消噪滤波：
///   每个电磁线圈有独立的消噪滤波器，防止 SimulationLoop 重置时
///   瞬间断电导致的阀位抖动。与 SolenoidValve52 逻辑一致。
/// </summary>
public class DoubleSolenoidValve35O : BaseValve
{
    // ========== 端口索引常量 ==========
    private const int IDX_P = 0;
    private const int IDX_A = 1;
    private const int IDX_B = 2;
    private const int IDX_R = 3;
    private const int IDX_S = 4;
    private const int REQUIRED_PORT_COUNT = 5;

    // ========== Inspector 配置 ==========
    [Header("防扰动消噪设置")]
    [Tooltip("电磁线圈失电后的机械/磁滞恢复延时(秒)，防止临界点抖动")]
    public float debounceDuration = 0.05f;

    [Header("阀芯状态（只读）")]
    [Tooltip("-1 = 左位（左侧电磁铁得电），0 = 中位（O型），1 = 右位（右侧电磁铁得电）")]
    public int valvePosition = 0;

    [Header("左侧电磁线圈双接线柱配置")]
    public DynamicNode leftCoilPortA;  // 左侧线圈接线柱 A (例如接24V)
    public DynamicNode leftCoilPortB;  // 左侧线圈接线柱 B (例如接0V)

    [Header("右侧电磁线圈双接线柱配置")]
    public DynamicNode rightCoilPortA; // 右侧线圈接线柱 A (例如接24V)
    public DynamicNode rightCoilPortB; // 右侧线圈接线柱 B (例如接0V)

    // ========== 消噪滤波器内部状态 ==========
    private float filterTimerLeft = 0f;
    private float filterTimerRight = 0f;
    private bool leftCoilEnergized = false;  // 左侧电磁线圈是否【有效通电】
    private bool rightCoilEnergized = false; // 右侧电磁线圈是否【有效通电】

    // ========== Unity 生命周期 ==========
    protected override void Start()
    {
        base.Start();
        foreach (var p in ports)
            p.state = PortState.Conduct;
    }

    private void Update()
    {
        // ============================================================
        // 左侧电磁线圈消噪滤波器（与 SolenoidValve52 逻辑一致）
        //   通电 → 立即生效，定时器满格重置
        //   断电 → 不立即失效，定时器倒计时结束后才确认失电
        // ============================================================
        bool leftHasPower = CheckCoilPower(leftCoilPortA, leftCoilPortB);

        if (leftHasPower)
        {
            leftCoilEnergized = true;
            filterTimerLeft = debounceDuration;
        }
        else
        {
            if (filterTimerLeft > 0f)
                filterTimerLeft -= Time.deltaTime;
            else
                leftCoilEnergized = false;
        }

        // ============================================================
        // 右侧电磁线圈消噪滤波器
        // ============================================================
        bool rightHasPower = CheckCoilPower(rightCoilPortA, rightCoilPortB);

        if (rightHasPower)
        {
            rightCoilEnergized = true;
            filterTimerRight = debounceDuration;
        }
        else
        {
            if (filterTimerRight > 0f)
                filterTimerRight -= Time.deltaTime;
            else
                rightCoilEnergized = false;
        }

        // --- 根据消噪后的稳定信号决定阀位 ---
        if (leftCoilEnergized && !rightCoilEnergized)
            valvePosition = -1;   // 左位
        else if (rightCoilEnergized && !leftCoilEnergized)
            valvePosition = 1;    // 右位
        else
            valvePosition = 0;    // 中位（O型）
                                   // 注：两侧同时得电时也回到中位（安全设计）
    }

    /// <summary>
    /// 检查电磁线圈是否有有效电流通过。
    /// 只有当两端子同时通电，且一端为正极、一端为负极时，才视为有电流。
    /// </summary>
    private bool CheckCoilPower(DynamicNode portA, DynamicNode portB)
    {
        if (portA == null || portB == null) return false;

        NodeType statusA = portA.currentPowerStatus;
        NodeType statusB = portB.currentPowerStatus;

        return (statusA == NodeType.Positive && statusB == NodeType.Negative) ||
               (statusA == NodeType.Negative && statusB == NodeType.Positive);
    }

    // ========== 气路逻辑 ==========
    public override void ProcessLogic()
    {
        if (ports.Count < REQUIRED_PORT_COUNT)
        {
            Debug.LogWarning(
                $"DoubleSolenoidValve35O [{name}]：需要 5 个端口（P,A,B,R,S），" +
                $"当前仅配置了 {ports.Count} 个。请在 Inspector 中检查 ports 列表！");
            return;
        }

        // 清空上一帧的所有内部连接
        // 中位（valvePosition == 0）时所有口互不连通，自然就是截止状态
        for (int i = 0; i < ports.Count; i++)
            ports[i].internalConnectTo = null;

        if (valvePosition == -1)
        {
            // ============================================================
            // 左位（左侧电磁铁得电）：P→A 导通，B→S 排气，R 截止
            // ============================================================
            ports[IDX_A].internalConnectTo = ports[IDX_P];  // A 从 P 取压
            ports[IDX_B].internalConnectTo = ports[IDX_S];  // B 排气到 S（S 通大气，压力≈0）
            // R(ports[3]) 不连接 → 截止
        }
        else if (valvePosition == 1)
        {
            // ============================================================
            // 右位（右侧电磁铁得电）：P→B 导通，A→R 排气，S 截止
            // ============================================================
            ports[IDX_B].internalConnectTo = ports[IDX_P];  // B 从 P 取压
            ports[IDX_A].internalConnectTo = ports[IDX_R];  // A 排气到 R（R 通大气，压力≈0）
            // S(ports[4]) 不连接 → 截止
        }
        // valvePosition == 0：中位 O 型，所有 internalConnectTo 保持 null，所有口截止

        // 驱动所有端口刷新内部压力信息
        foreach (var port in ports)
            port.ReceiveInternalInfo();
    }
}
