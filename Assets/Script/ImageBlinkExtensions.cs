using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public static class ImageBlinkExtensions
{
    /// <summary>
    /// 让 UI Image 以 HSV 的 V 通道在 [minVPercent, maxVPercent] 之间闪烁 blinkCount 次
    /// （一次=上升+下降）。仅改变 V，不动 H/S 与 alpha。
    /// </summary>
    /// <param name="img">目标 Image</param>
    /// <param name="blinkCount">闪烁次数（一次=上+下），默认 2 次</param>
    /// <param name="minVPercent">V 下限(%)，默认 60</param>
    /// <param name="maxVPercent">V 上限(%)，默认 100</param>
    /// <param name="halfDuration">每个半程的时长(秒)，默认 0.15</param>
    /// <param name="ease">缓动，默认 InOutSine</param>
    /// <param name="restoreOriginalAtEnd">是否在结束时恢复最初颜色，默认 false</param>
    public static Sequence BlinkValue(
        this Image img,
        int blinkCount = 2,
        float minVPercent = 60f,
        float maxVPercent = 100f,
        float halfDuration = 0.15f,
        Ease ease = Ease.InOutSine,
        bool restoreOriginalAtEnd = false
    ){
        if (img == null) return null;

        minVPercent = Mathf.Clamp(minVPercent, 0f, 100f);
        maxVPercent = Mathf.Clamp(maxVPercent, 0f, 100f);
        if (minVPercent > maxVPercent) (minVPercent, maxVPercent) = (maxVPercent, minVPercent);

        // 记录初始颜色与 HSV
        var original = img.color;
        Color.RGBToHSV(original, out float h, out float s, out float v);
        float a = original.a;

        float minV = minVPercent / 100f;
        float maxV = maxVPercent / 100f;

        // 清除绑定在该 Image 上的旧 tween，避免叠加
        DOTween.Kill(img);

        // getter/setter：只改 V，再写回颜色
        Tween Up(float target) => DOTween.To(
            () => v,
            x => {
                v = x;
                var c = Color.HSVToRGB(h, s, v);
                c.a = a;
                img.color = c;
            },
            target,
            halfDuration
        ).SetEase(ease);

        var seq = DOTween.Sequence()
            .SetId(img) // 之后可用 DOTween.Kill(img) 停止
            .Append(Up(minV))  // 上升到 100%
            .Append(Up(maxV))  // 下降回 60%
            .SetLoops(blinkCount) // 每个 loop = (上+下) = 1 次闪烁
            .SetLink(img.gameObject, LinkBehaviour.KillOnDestroy); // 物体销毁自动清理

        if (restoreOriginalAtEnd)
            seq.OnComplete(() => img.color = original);

        return seq;
    }
}
