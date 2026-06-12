using UnityEngine;
using DG.Tweening;

/// <summary>
/// 单作用气缸 (Single-Acting Cylinder - Spring Return)
/// 端口约定：
/// - ports[0]: A (唯一个进气/控制口) -> PortType.Input
/// 逻辑：
/// - A口有压 (pA > 0.1)：克服弹簧力伸出，速度受进气流量控制。
/// - A口无压 (pA <= 0.1)：依靠内部弹簧释放自动缩回。
/// </summary>
public class SingleActingCylinder : BaseValve
{
    [Header("参数设置")]
    public Transform pistonRod;
    public float maxStroke = 2.6f; // 保持和你双作用气缸一致的 2.6

    [Tooltip("气压推动时，满流速(Flow=1.0)走完满行程的最快时间")]
    public float minDuration = 0.5f;

    [Tooltip("气压推动且流速极低时，走完行程的最长时间（节流明显）")]
    public float maxDuration = 10.0f;

    [Tooltip("弹簧复位（无气压）时，走完满行程缩回的时间（固定常数）")]
    public float springReturnDuration = 0.8f;

    public float currentPos = 0f;
    private Tweener moveTweener;
    private float lastDirection = 0;
    private float lastFlowRate = -1f;

    [Header("音效设置")]
    public AudioSource audioSource;
    public AudioClip extendSound;  // 气压推动伸出音效
    public AudioClip retractSound; // 弹簧释放缩回音效
    public CylinderSignalSource cySignal1, cySignal2;
    public override void ProcessLogic() { }

    void Update()
    {
        // 单作用气缸通常只需要 1 个物理端口
        if (ports.Count < 1) return;

        float pA = ports[0].pressure;
        float currentFlow = ports[0].inFlowRate;

        float currentDirection = 0;
        if (pA > 0.1f)
        {
            currentDirection = 1; // 气压推力 -> 伸出
        }
        else
        {
            currentDirection = -1; // 弹簧回弹 -> 缩回
        }

        // 仅在完全缩回（Pos=0）或完全伸出（Pos=maxStroke）且无指令时停止
        if (currentDirection == -1 && currentPos <= 0.001f) currentDirection = 0;
        if (currentDirection == 1 && currentPos >= maxStroke - 0.001f)
        {
            // 如果已经到顶了，且流量没变，就不需要重复刷新动画
            if (Mathf.Abs(currentFlow - lastFlowRate) <= 0.01f) currentDirection = 0;
        }

        // 当运动方向改变，或在伸出过程中流量发生变化时刷新动画
        if (currentDirection != lastDirection || (currentDirection == 1 && Mathf.Abs(currentFlow - lastFlowRate) > 0.01f))
        {
            if (currentDirection != lastDirection && currentDirection != 0)
            {
                PlayActionSound(currentDirection);
            }
            UpdateAnimation(currentDirection, currentFlow);
            lastDirection = currentDirection;
            lastFlowRate = currentFlow;
        }
    }

    private void PlayActionSound(float direction)
    {
        if (audioSource == null) return;
        audioSource.Stop();

        if (direction == 1 && extendSound != null)
        {
            audioSource.clip = extendSound;
            audioSource.Play();
        }
        else if (direction == -1 && retractSound != null)
        {
            audioSource.clip = retractSound;
            audioSource.Play();
        }
    }

    private void UpdateAnimation(float direction, float flow)
    {
        moveTweener?.Kill();

        if (direction == 0)
        {
            if (audioSource != null && audioSource.isPlaying) audioSource.Stop();
            return;
        }

        float targetPos = (direction == 1) ? maxStroke : 0f;
        float remainingDistance = Mathf.Abs(targetPos - currentPos);
        float progressNeeded = remainingDistance / maxStroke;

        float realDuration = 0f;

        if (direction == 1)
        {
            // --- 伸出状态：受进气节流阀控制 ---
            if (flow <= 0.001f) return; // 节流阀完全关死则不移动
            float calcDuration = Mathf.Lerp(maxDuration, minDuration, flow);
            realDuration = progressNeeded * calcDuration;

            if (audioSource != null) audioSource.pitch = Mathf.Lerp(0.8f, 1.2f, flow);
        }
        else
        {
            // --- 缩回状态：纯靠内部弹簧复位 ---
            // 弹簧释放速度是恒定的，不受进气口流量系数影响
            realDuration = progressNeeded * springReturnDuration;

            if (audioSource != null) audioSource.pitch = 1.0f; // 弹簧声音保持正常音调
        }

        moveTweener = DOTween.To(() => currentPos, x =>
        {
            currentPos = x;
            // 沿用你模型的相对坐标映射逻辑
            pistonRod.localPosition = new Vector3(currentPos, 0, 0);
        }, targetPos, realDuration)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            if (audioSource != null) audioSource.Stop();
        });
    }
}