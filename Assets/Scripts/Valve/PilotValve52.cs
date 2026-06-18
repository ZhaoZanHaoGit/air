using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotValve52 : BaseValve
{
    [Header("状态记忆")]
    public bool isSwitched = false; // false: P->B, true: P->A


    [Tooltip("弹起手柄时的物理缓冲延时(秒)，用于过滤瞬间断开带来的气压震荡")]
    public float debounceDuration = 0.05f;
    private float filterTimer = 0f;
    private bool internalPressedState = false;
    private void Start()
    {
        base.Start();
        ports[0].state = PortState.Conduct;
        ports[1].state = PortState.Conduct;
        ports[2].state = PortState.Conduct;
        ports[3].state = PortState.Conduct;
        ports[4].state = PortState.Conduct;
        ports[5].state = PortState.Conduct;
    }
    void Update()
    {
        // 在原生 Update 中处理物理状态的消噪滤波器
        if (isSwitched)
        {
            internalPressedState = true;
            filterTimer = debounceDuration; // 持续按下时，滤波器永远满格
        }
        else
        {
            // 当瞬时断开时，进入倒计时消噪保护
            if (filterTimer > 0)
            {
                filterTimer -= Time.deltaTime;
            }
            else
            {
                internalPressedState = false; // 只有当确定断开时间超过了阈值，才真正改变阀芯内部状态
            }
        }
    }
    public override void ProcessLogic()
    {
        // 端口索引约定：
        // ports[0]: P (进气) -> Input
        // ports[1]: A (输出1) -> Output
        // ports[2]: B (输出2) -> Output
        // ports[3]: Z1 (左控制口，使 P->A) -> Input
        // ports[4]: Z2 (右控制口，使 P->B) -> Input
        // ports[5]: R (右控制口，使 P->B) -> Input
        if (ports.Count < 5) return;

        // 1. 气控切换逻辑：检测控制口的压力脉冲
        // 只要 Z1 有压力，阀芯就推向右侧（P-A导通），并保持
        if (ports[3].pressure > 0.1f)
        {
            isSwitched = true;
        }
        // 只要 Z2 有压力，阀芯就推向左侧（P-B导通），并保持
        else if (ports[4].pressure > 0.1f)
        {
            isSwitched = false;
        }

        // 2. 执行气路输出逻辑
        if (internalPressedState)
        {
           
            ports[2].internalConnectTo = ports[5];
            ports[1].internalConnectTo = ports[0];  
        }
        else
        {
            ports[1].internalConnectTo = ports[5];
            ports[2].internalConnectTo = ports[0];
        }
       
        ports[1].ReceiveInternalInfo();
        ports[2].ReceiveInternalInfo();
        ports[0].ReceiveInternalInfo();
    }
}
