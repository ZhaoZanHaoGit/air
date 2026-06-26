using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VacuumGenerator : BaseValve
{
    [Header("状态记忆")]
    public bool isSwitched = false;
    [Tooltip("弹起手柄时的物理缓冲延时(秒)，用于过滤瞬间断开带来的气压震荡")]
    public float debounceDuration = 0.05f;
    private float filterTimer = 0f;
    private bool internalPressedState = false;
    public AudioClip audioClip;
    public bool isPlaying = false;
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
        if (internalPressedState && !isPlaying)
        {
            AudioManager.Instance.PlaySoundEff2D(audioClip, true, ports[0].pressure);
            isPlaying = true;
        }
        else if (!internalPressedState && isPlaying)
        {
            AudioManager.Instance.StopSoundEff(audioClip);
            isPlaying = false;
        }
    }
    public override void ProcessLogic()
    {
        if (ports.Count < 1) return;

        if (ports[0].exPressure > 0.1f)
        {
            isSwitched = true;
        }
        // 只要 Z2 有压力，阀芯就推向左侧（P-B导通），并保持
        else
        {
            isSwitched = false;
        }
    }
}
