using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ManualValve32 : BaseValve
{
    [Header("按钮物理状态")]
    public bool isPressed = false;

    [Header("防扰动消噪设置")]
    [Tooltip("弹起手柄时的物理缓冲延时(秒)，用于过滤瞬间断开带来的气压震荡")]
    public float debounceDuration = 0.05f;

    private float filterTimer = 0f;
    private bool internalPressedState = false;

    void Update()
    {
        // 在原生 Update 中处理物理状态的消噪滤波器
        if (isPressed)
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

    /// <summary>
    /// 阶段 1：根据消噪后的内部状态驱动阀芯極性
    /// </summary>
    public override void ProcessLogic()
    {
        if (ports.Count < 2) return;

        PneumaticPort portP = ports[0];
        PneumaticPort portA = ports[1];

        //if (portP.state == PortState.CutOff || portA.state == PortState.CutOff) return;

        // 2. 核心控制：利用消噪后的内部状态，重新编排“内部小手”的绑定
        if (internalPressedState)
        {
            // 【有效按下态】：P口与A口在内部手拉手，R口彻底孤立截止
            portP.state = PortState.Conduct;
            portA.state = PortState.Conduct;
           // portR.state = PortState.CutOff;

            portP.internalConnectTo = portA;
            portA.internalConnectTo = portP;
            //portR.internalConnectTo = null;
        }
        else
        {
            // 【有效弹起态】：A口与R口在内部手拉手实现排气，P口物理孤立截止
            portP.state = PortState.CutOff;
            portA.state = PortState.Conduct;
           // portR.state = PortState.Conduct;

            portP.internalConnectTo = null;
            portA.internalConnectTo = null;
            //portR.internalConnectTo = portA;
        }

        // 3. 驱动步骤1：让各个活跃端口顺着全新的内部纽带，无因果污染地去获取彼此内部数据
        foreach (var p in ports)
        {
            p.ReceiveInternalInfo();
        }
    }
}
