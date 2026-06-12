using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePilotValve52 : BaseValve
{
    [Header("端口约定说明")]
    [Tooltip("请按以下顺序拖入端口：\n" +
             "Index 0: P (进气口)\n" +
             "Index 1: A (伸出端 - 连气缸后腔)\n" +
             "Index 2: B (缩回端 - 连气缸前腔)\n" +
             "Index 3: Z (气控信号端 - 左侧控制)")]


    [Header("状态记忆")]
    public bool isSwitched = false; // false: P->B, true: P->A


    [Tooltip("弹起手柄时的物理缓冲延时(秒)，用于过滤瞬间断开带来的气压震荡")]
    public float debounceDuration = 0.05f;
    private float filterTimer = 0f;
    private bool internalPressedState = false;
    private void Start()
    {
        base.Start();
        ports[3].state = PortState.Conduct;
      
    }
    void Update()
    {

        isSwitched = (ports[3].pressure > 0.1f);

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


        // 端口约定：
        // ports[0]: P (进气)
        // ports[1]: A (伸出端)
        // ports[2]: B (缩回端)
        // ports[3]: Z (唯一的左侧气控信号口)

        // 2. 执行气路输出逻辑
        if (internalPressedState)
        {
            ports[0].state = PortState.Conduct;
            ports[1].state = PortState.Conduct;
            ports[2].state = PortState.CutOff;
            ports[2].internalConnectTo = null;
            ports[1].internalConnectTo = ports[0];
            ports[0].internalConnectTo = ports[1];

        }
        else
        {
            ports[0].state = PortState.Conduct;
            ports[1].state = PortState.CutOff;
            ports[2].state = PortState.Conduct;
            ports[1].internalConnectTo = null;

            ports[2].internalConnectTo = ports[0];
            ports[0].internalConnectTo = ports[2];
        }

        ports[1].ReceiveInternalInfo();
        ports[2].ReceiveInternalInfo();
        ports[0].ReceiveInternalInfo();
    }
}

