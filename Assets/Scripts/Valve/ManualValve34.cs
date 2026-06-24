using UnityEngine;

/// <summary>
/// 三位四通手动换向阀 (3/4-Way Manual Rotary Valve - Closed Center)
/// 逻辑：
/// - 左位 (State = -1)：P->B, A->R
/// - 中位 (State =  0)：P, A, B, R 全截止 (不导通)
/// - 右位 (State =  1)：P->A, B->R
/// 手柄不复位，通过外部 UI 或点击事件改变 ValveState。
/// </summary>
public class ManualValve34 : BaseValve
{
   
    PneumaticPort coreHub;
    [Header("阀芯位置状态")]
    [Tooltip("通过外部按钮或手柄动画直接切换此枚举值")]
    public ValveState currentState = ValveState.Center;

    private void Start()
    {
        base.Start();
       
    }

    public override void ProcessLogic()
    {
        // 端口索引约定 (严格按照标准四通阀符号):
        // ports[0]: P (进气口) -> PortType.Input
        // ports[1]: A (工作口A) -> PortType.Output
        // ports[2]: B (工作口B) -> PortType.Output
        // ports[3]: R (总排气口) -> 仿真中压力恒为 0，用于清空对应腔体

        if (ports.Count < 3) return;

        switch (currentState)
        {
            case ValveState.Left:
                // --- 左位逻辑：P->B, A->R ---
                // P 与 B 导通
                ports[1].state = PortState.Conduct; // A 口物理截止
                ports[2].state = PortState.Conduct;
                ports[0].state = PortState.Conduct;
                ports[1].internalConnectTo = ports[3];
               ports[2].internalConnectTo = ports[0];


                break;

            case ValveState.Center:
                // --- 中位逻辑：全截止，各口保持封闭 ---
                // 此时 A 口和 B 口既不接进气，也不接排气，
                // 它们应该保持当前的物理密闭状态。但由于 SimulationLoop 每帧都会清空非 Output，
                // 为了防止气缸两侧瞬间泄压导致自由滑移，我们作为 Output 必须强行输出 0 (或者锁死压力)。
                // 标准 O 型中位阀的仿真处理：切断外界供给，输出端强制置0（若要保压，需结合气缸闭锁逻辑）
                ports[0].state = PortState.CutOff;
                ports[1].state = PortState.CutOff;
                ports[2].state = PortState.CutOff;
                ports[3].state = PortState.CutOff;
                break;

            case ValveState.Right:
                // --- 右位逻辑：P->A, B->R ---
                // P 与 A 导通
                ports[1].state = PortState.Conduct; 
                ports[2].state = PortState.Conduct;
                ports[0].state = PortState.Conduct;
                ports[1].internalConnectTo = ports[0];
                ports[2].internalConnectTo = ports[3];
                break;
        }
        ports[0].ReceiveInternalInfo();
        ports[1].ReceiveInternalInfo();
        ports[2].ReceiveInternalInfo();
    }

    /// <summary>
    /// 供外部手柄旋转、UI 按钮点击调用的公共方法
    /// </summary>
    public void SetValveState(int stateIndex)
    {
        // 映射：-1 -> 左, 0 -> 中, 1 -> 右
        currentState = (ValveState)Mathf.Clamp(stateIndex, -1, 1);
    }
}