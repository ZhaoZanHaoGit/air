using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowCHargeLightEffect : MonoBehaviour
{
    public Material[] lights;
    public Material buleMaterial;
    public Color blueColor, greenColor;

    // 灯光序列协程
    public void StartLight(bool isConnect)
    {
        StartCoroutine(LightSequence(isConnect));
    }
    private IEnumerator LightSequence(bool isConnect)
    {
        buleMaterial.color = isConnect ? blueColor : Color.white;
        while (true) // 无限循环
        {
            for (int k = 1; k <= lights.Length; k++)
            {
                // 亮起前 k 个球体，后面的关闭
                for (int i = 0; i < lights.Length; i++)
                {
                    if (i < k)
                    {
                        TurnOn(i);
                    }
                    else
                    {
                        TurnOff(i);
                    }
                }
                yield return new WaitForSeconds(1f); // 间隔 1 秒
            }
            // 全亮后，下一个循环的 k=1 会自动重置（关闭多余的）
        }
    }
    // 亮起单个球体（使用 Emission）
    private void TurnOn(int index)
    {
        var mat = lights[index];
        mat.SetColor("_BaseColor", greenColor);
        mat.SetColor("_EmissionColor", Color.green); // 自发光白色
        mat.EnableKeyword("_EMISSION"); // 启用 Emission
    }

    // 关闭单个球体
    private void TurnOff(int index)
    {
        var mat = lights[index];
        mat.SetColor("_BaseColor", Color.white);
        mat.SetColor("_EmissionColor", Color.white); // 自发光黑色（相当于关闭）
    }

    private void OnDisable()
    {
        foreach (var light in lights)
        {
            light.SetColor("_BaseColor", Color.white);
            light.SetColor("_EmissionColor", Color.white); // 自发光黑色（相当于关闭）
            light.DisableKeyword("_EMISSION"); // 禁用 Emission
        }
    }
    private void OnEnable()
    {
        foreach (var light in lights)
        {
            light.SetColor("_BaseColor", Color.white);
            light.SetColor("_EmissionColor", Color.white); // 自发光黑色（相当于关闭）
            light.DisableKeyword("_EMISSION"); // 禁用 Emission
        }
    }
}
