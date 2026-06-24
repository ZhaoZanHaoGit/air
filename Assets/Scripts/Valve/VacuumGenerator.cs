using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 真空发生器
/// 端口排列（Inspector中按顺序配置 3 个 PneumaticPort）：
///   [0] P  进气口（供给压缩空气）
///   [1] A  真空口（产生真空，连接吸盘等）
///   [2] R  排气口（消声器口，压缩空气由此排出）
///
/// 工作逻辑：
///   当 P 口有进气压力时：
///     - P→R 气路导通（压缩空气从 R 口排出，产生文丘里效应）
///     - A 口产生真空（负压，由 PneumaticPort.isVacuumSource 驱动）
///     - 播放工作音效
///   当 P 口无进气压力时：
///     - 所有口截止，A 口真空消失（恢复大气压）
///     - 停止音效
///
/// 消噪滤波：
///   与 ManualValve32 / PilotValve52 逻辑一致，防止 SimulationLoop 重置时
///   瞬间断开导致误判进气消失。
/// </summary>
public class VacuumGenerator : BaseValve
{
    // ========== 端口索引常量 ==========
    private const int IDX_P = 0;   // 进气口
    private const int IDX_A = 1;   // 真空口
    private const int IDX_R = 2;   // 排气口
    private const int REQUIRED_PORT_COUNT = 3;

    // ========== Inspector 配置 ==========
    [Header("消噪滤波设置")]
    [Tooltip("进气口信号断开时的消噪延时(秒)，用于过滤 SimulationLoop 重置等瞬间断开带来的气压震荡")]
    public float debounceDuration = 0.05f;

    [Header("气控信号阈值")]
    [Tooltip("进气口压力超过此值(0~1)视为有进气")]
    public float supplyPressureThreshold = 0.1f;

    [Header("真空参数")]
    [Tooltip("产生的真空度（-1~0，负值表示真空，绝对值越大真空度越高）")]
    public float vacuumPressure = -0.8f;

    [Header("音效")]
    [Tooltip("真空发生器工作时的循环音效，拖入 AudioClip 即可")]
    public AudioClip workSoundClip;

    // ========== 消噪滤波器内部状态 ==========
    private float filterTimer = 0f;
    private bool internalAirSupplyState = false;  // 消噪后的稳定进气状态
    private bool isGeneratingVacuum = false;      // 当前是否正在产生真空

    // ========== 组件引用 ==========
    private AudioSource audioSource;

    // ========== Unity 生命周期 ==========
    protected override void Start()
    {
        base.Start();
        foreach (var p in ports)
            p.state = PortState.Conduct;

        // 初始化 AudioSource（若不存在则自动添加）
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && workSoundClip != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        if (audioSource != null)
        {
            audioSource.clip = workSoundClip;
            audioSource.loop = true;           // 持续进气时持续播放
            audioSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        // ============================================================
        // 消噪滤波器：与 ManualValve32 / PilotValve52 逻辑完全一致
        // 原理：
        //   信号出现（pressure > threshold）→ 立即生效，定时器满格重置
        //   信号消失（pressure < threshold）→ 不立即失效，定时器倒计时
        //   定时器到期后 → 才确认"真的消失了"
        // ============================================================
        if (ports.Count < REQUIRED_PORT_COUNT) return;

        if (ports[IDX_P].pressure > supplyPressureThreshold)
        {
            // 有进气：立即生效，重置定时器
            internalAirSupplyState = true;
            filterTimer = debounceDuration;
        }
        else
        {
            // 进气消失：启动消噪倒计时
            if (filterTimer > 0f)
            {
                filterTimer -= Time.deltaTime;
            }
            else
            {
                // 倒计时结束，确认进气真正消失
                internalAirSupplyState = false;
            }
        }

        // ============================================================
        // 真空产生状态机 + 音效控制
        // ============================================================
        if (internalAirSupplyState && !isGeneratingVacuum)
        {
            // 开始产生真空
            isGeneratingVacuum = true;
            if (audioSource != null && !audioSource.isPlaying)
                audioSource.Play();
        }
        else if (!internalAirSupplyState && isGeneratingVacuum)
        {
            // 停止产生真空
            isGeneratingVacuum = false;
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    // ========== 气路逻辑（每帧被 SimulationLoop 调用） ==========
    public override void ProcessLogic()
    {
        if (ports.Count < REQUIRED_PORT_COUNT)
        {
            Debug.LogWarning(
                $"VacuumGenerator [{name}]：需要 3 个端口（P,A,R），" +
                $"当前仅配置了 {ports.Count} 个。请在 Inspector 中检查 ports 列表！");
            return;
        }

        // 清空上一帧的所有内部连接
        for (int i = 0; i < ports.Count; i++)
            ports[i].internalConnectTo = null;

        if (isGeneratingVacuum)
        {
            // ============================================================
            // 有进气：P→R 导通；A 口设为真空源
            // ============================================================

            // P→R 气路导通（压缩空气流过发生器，从 R 排出）
            ports[IDX_P].internalConnectTo = ports[IDX_R];
            ports[IDX_R].internalConnectTo = ports[IDX_P];

            // A 口设为真空源，IntegrateAndOutput() 会直接输出 vacuumPressure
            ports[IDX_A].isVacuumSource = true;
            ports[IDX_A].vacuumPressure = vacuumPressure;
        }
        else
        {
            // ============================================================
            // 无进气：所有口截止；A 口取消真空源状态
            // ============================================================
            ports[IDX_A].isVacuumSource = false;
        }

        // 驱动所有端口刷新内部压力信息
        foreach (var port in ports)
            port.ReceiveInternalInfo();
    }
}
