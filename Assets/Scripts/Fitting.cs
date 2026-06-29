using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 必须引用 UI 命名空间

public class Fitting : MonoBehaviour
{
    // 将枚举改为 public 方便在 Inspector 或其他脚本中查看
    public FittingType fittingType;

    // 在 Inspector 中拖入对应的模型

    public GameObject plug1; // 对应 Blocked
    public GameObject plug3; // 对应 Opening
    public GameObject plug2; // 对应 Silencing


    void Start()
    {
        // 初始化时执行一次，确保初始状态正确
        UpdatePlugs();
    }

    /// <summary>
    /// 提供给外部（如 Toggle 事件）调用的方法
    /// 参数 index 对应你想要的输出 1, 2, 3
    /// </summary>
    public void SetFittingType(int index)
    {
        // 根据传入的整数切换枚举状态
        // 这里的 index - 1 是为了匹配枚举从 0 开始的索引
        fittingType = (FittingType)(index);

        // 执行显隐逻辑
        UpdatePlugs();
    }

    private void UpdatePlugs()
    {
        // 先全部隐藏（简洁的做法）
        if (plug1) plug1.SetActive(false);
        if (plug2) plug2.SetActive(false);
        if (plug3) plug3.SetActive(false);

        // 根据当前的枚举值显示对应的物体
        switch (fittingType)
        {
            case FittingType.Blocked:
                if (plug1) plug1.SetActive(true);
                break;
            case FittingType.Silencing:
                if (plug2) plug2.SetActive(true);
                break;
            case FittingType.Opening:
                if (plug3) plug3.SetActive(true);
                break;
        }

        Debug.Log($"当前切换至: {fittingType}，输出索引: {(int)fittingType + 1}");
    }
}

public enum FittingType
{
    Opening = 0,   // 对应输出 3
    Blocked = 1,   // 对应输出 1
    Silencing = 2, // 对应输出 2

}