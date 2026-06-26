using UnityEngine;

public class RollerValve32_B : BaseValve
{

    [Header("信号绑定")]
    public BaseSignalSource boundSignal;

    [Header("防扰动消噪设置")]
    [Tooltip("触碰信号断开后的机械恢复延时(秒)，防止临界点像素级抖动导致压力崩塌")]
    public float debounceDuration = 0.1f;

    private float filterTimer = 0f;
    private bool internalTriggerState = false;

    void Update()
    {
        bool rawTrigger = (boundSignal != null && boundSignal.IsTriggered);

        if (rawTrigger)
        {
            internalTriggerState = true;
            filterTimer = debounceDuration;
        }
        else
        {
            if (filterTimer > 0)
            {
                filterTimer -= Time.deltaTime;
            }
            else
            {
                internalTriggerState = false;
            }
        }
    }

    public override void ProcessLogic()
    {
        if (ports.Count < 2) return;

        PneumaticPort portP = ports[0];
        PneumaticPort portA = ports[1];

        // 注意：由于我们之前在第二步将行程阀在逻辑上改为了“常开型切换”，
        // 所以当检测到有效触发信号（internalTriggerState == true）时，阀芯需要【锁死切断（CutOff）】来实现慢速工进。
        if (!internalTriggerState)
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
