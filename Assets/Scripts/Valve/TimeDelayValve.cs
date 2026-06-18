using UnityEngine;

public class TimeDelayValve : BaseValve
{
    // Index 0: Input P (进气)
    // Index 1: Control Z (控制信号)
    // Index 2: Output A (输出)

    [Header("延迟设置")]
    public float delayTime = 2.0f;          // 目标延迟时间
    public float faultTolerance = 0.5f;     // 抖动容错时间 (断气多久才重置)

    [Header("实时状态")]
    [SerializeField] private float currentTimer = 0f;    // 充气计时
    [SerializeField] private float dischargeTimer = 0f; // 断气计时
    private bool hasSignal = false;

    void Update()
    {
        if (hasSignal)
        {
            // 1. 正常计时：信号存在时，充气计时增加，断气计时重置
            currentTimer += Time.deltaTime;
            dischargeTimer = 0f;
        }
        else
        {
            // 2. 容错逻辑：信号消失时，开始计算断气时长
            dischargeTimer += Time.deltaTime;

            // 只有当断气时间超过了设定的容错值，才判定为彻底断气
            if (dischargeTimer >= faultTolerance)
            {
                currentTimer = 0f;
            }
        }
    }

    public override void ProcessLogic()
    {
        if (ports.Count < 3) return;

        // 获取控制口 Z 的压力状态
        // 这里依然保持 0.1f 判定，确保 SimulationLoop 的重置能被捕捉到
        hasSignal = (ports[1].pressure > 0.1f);

        // 输出逻辑判定：只有充气计时达标且“当前”有压力时才输出
        // 这样可以保证一旦彻底断气，输出 A 会立刻消失，而不是等 0.5s 容错结束
        if (currentTimer >= delayTime && hasSignal)
        {
            ports[0].state = PortState.Conduct;
            ports[2].state = PortState.Conduct;
            ports[2].internalConnectTo = ports[0];
            ports[0].ReceiveInternalInfo();
            ports[2].ReceiveInternalInfo();
        }
        else
        {
            ports[2].internalConnectTo = ports[3];
            ports[0].state = PortState.CutOff;
        }
    }
}