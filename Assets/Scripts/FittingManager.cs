using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FittingManager : MonoBehaviour
{
   
    // 存储场景中所有 Fitting 实例的列表
    public List<Fitting> fittings=new List<Fitting>();

    /// <summary>
    /// 修改指定 Fitting 的状态
    /// </summary>
    /// <param name="fittingIndex">列表中的索引 (从0开始)</param>
    /// <param name="stateIndex">要切换的状态索引 (1, 2, 3)</param>
    public void ChangeFittingState(int fittingIndex, int stateIndex)
    {
        // 1. 安全检查：确保索引在列表范围内
        if (fittingIndex >= 0 && fittingIndex < fittings.Count)
        {
            Fitting targetFitting = fittings[fittingIndex];

            // 2. 确保目标脚本不为空
            if (targetFitting != null)
            {
                // 调用 Fitting 脚本中我们之前写好的方法
                targetFitting.SetFittingType(stateIndex);
                Debug.Log($"已修改索引为 {fittingIndex} 的配件状态为 {stateIndex}");
            }
            else
            {
                Debug.LogError($"索引 {fittingIndex} 处的 Fitting 脚本引用丢失！");
            }
        }
        else
        {
            Debug.LogWarning($"fittingIndex {fittingIndex} 超出有效范围。");
        }
    }
}
