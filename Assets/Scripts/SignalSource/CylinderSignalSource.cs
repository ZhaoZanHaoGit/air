using Unity.Mathematics;
using UnityEngine;

public class CylinderSignalSource : BaseSignalSource
{
    public DoubleActingCylinder targetCylinder; // 引用对应的气缸

    public enum DetectionMode { StartPoint, EndPoint }
    [Header("检测配置")]
    public DetectionMode detectionMode;
    public float threshold = 0.01f; // 触发阈值
    public float percent =1;

    public override bool IsTriggered
    {
        get
        {
            if (targetCylinder == null) return false;

          float temp=  math.abs(targetCylinder.maxStroke * percent - targetCylinder.currentPos);
            /*
            // 根据当前位置判定是否触发
            if (detectionMode == DetectionMode.StartPoint)
            {
                // 检测是否在起点 (位置接近 0)
                return targetCylinder.currentPos <= threshold;
            }
            else
            {
                // 检测是否在终点 (位置接近最大行程)
                return targetCylinder.currentPos >= (targetCylinder.maxStroke - threshold);
            }*/
            return temp < threshold;
        }
    }
}