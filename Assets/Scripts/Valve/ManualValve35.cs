using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 三位五通阀（O型中位，双气控）
/// 端口排列（Inspector中按顺序配置 7 个 PneumaticPort）：
///   [0] P  进气口
///   [1] A  输出口A
///   [2] B  输出口B
///   [3] R  排气口R
///   [4] S  排气口S
///   [5] K1 左气控口（Z1）
///   [6] K2 右气控口（Z2）
///
/// 工作逻辑：
///   K1有信号：P→A，B→S（排气），R截止  （左位）
///   K2有信号：P→B，A→R（排气），S截止  （右位）
///   K1/K2均无信号：所有口截止            （O型中位）
///
/// 消噪滤波：
///   每个控制口有独立的消噪滤波器，防止 SimulationLoop 重置时
///   瞬间断开导致的气压震荡误触发阀位切换。
/// </summary>
public class ManualValve35 : BaseValve
{
    // ========== 端口索引常量 ==========
    private const int IDX_P  = 0;
    private const int IDX_A  = 1;
    private const int IDX_B  = 2;
    private const int IDX_R  = 3;
    private const int IDX_S  = 4;
    private const int IDX_K1 = 5;
    private const int IDX_K2 = 6;
    private const int REQUIRED_PORT_COUNT = 7;

    // ========== Inspector 配置 ==========
    [Header("消噪滤波设置")]
    [Tooltip("控制口信号断开时的消噪延时(秒)，用于过滤 SimulationLoop 重置等瞬间断开带来的气压震荡")]
    public float debounceDuration = 0.05f;

    [Header("气控信号阈值")]
    [Tooltip("控制口压力超过此值(0~1)视为有控制信号")]
    public float controlPressureThreshold = 0.1f;

    [Header("阀芯状态（只读）")]
    [Tooltip("-1 = 左位（K1有效），0 = 中位（O型），1 = 右位（K2有效）")]
    public int valvePosition = 0;

    // ========== 消噪滤波器内部状态 ==========
    // 每个控制口独立的消噪定时器
    private float filterTimerK1 = 0f;
    private float filterTimerK2 = 0f;

    // 消噪后的稳定信号状态（经过消噪滤波器确认的有效信号）
    private bool k1Effective = false;
    private bool k2Effective = false;

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
        // 消噪滤波器：在 Update 中处理，与 ManualValve32 / PilotValve52 逻辑一致
        // 原理：
        //   信号出现（pressure > threshold）→ 立即生效，定时器满格重置
        //   信号消失（pressure < threshold）→ 不立即失效，定时器开始倒计时
        //   定时器到期后 → 才确认信号真正消失
        // 这样即使 SimulationLoop 重置导致瞬间断开，只要断开时间短于
        // debounceDuration，阀门就不会误切换。
        // ============================================================

        if (ports.Count < REQUIRED_PORT_COUNT) return;

        // --- K1 消噪滤波器 ---
        if (ports[IDX_K1].pressure > controlPressureThreshold)
        {
            // 信号出现：立即生效，重置定时器
            k1Effective = true;
            filterTimerK1 = debounceDuration;
        }
        else
        {
            // 信号消失：启动消噪倒计时
            if (filterTimerK1 > 0f)
            {
                filterTimerK1 -= Time.deltaTime;
            }
            else
            {
                // 倒计时结束，确认信号真正消失
                k1Effective = false;
            }
        }

        // --- K2 消噪滤波器 ---
        if (ports[IDX_K2].pressure > controlPressureThreshold)
        {
            k2Effective = true;
            filterTimerK2 = debounceDuration;
        }
        else
        {
            if (filterTimerK2 > 0f)
            {
                filterTimerK2 -= Time.deltaTime;
            }
            else
            {
                k2Effective = false;
            }
        }

        // --- 根据消噪后的稳定信号决定阀位 ---
        if (k1Effective && !k2Effective)
            valvePosition = -1;   // 左位
        else if (k2Effective && !k1Effective)
            valvePosition = 1;    // 右位
        else
            valvePosition = 0;    // 中位（O型）
                                   // 注：K1/K2 同时有效时也回到中位（安全设计）
    }

    // ========== 气路逻辑 ==========
    public override void ProcessLogic()
    {
        if (ports.Count < REQUIRED_PORT_COUNT)
        {
            Debug.LogWarning(
                $"ManualValve35 [{name}]：需要 7 个端口（P,A,B,R,S,K1,K2），" +
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
            // 左位（K1 控制）：P→A 导通，B→S 排气，R 截止
            // ============================================================
            ports[IDX_A].internalConnectTo = ports[IDX_P];  // A 从 P 取压
            ports[IDX_B].internalConnectTo = ports[IDX_S];  // B 排气到 S（S 通大气，压力≈0）
            // R(ports[3]) 不连接 → 截止
        }
        else if (valvePosition == 1)
        {
            // ============================================================
            // 右位（K2 控制）：P→B 导通，A→R 排气，S 截止
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
