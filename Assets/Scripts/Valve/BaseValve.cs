using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseValve : MonoBehaviour
{
    public string valveName;
    public List<PneumaticPort> ports = new List<PneumaticPort>();

    protected virtual void Start()
    {
        // 自动注册到模拟器
        if (SimulationLoop.Instance != null)
            SimulationLoop.Instance.allValves.Add(this);
    }

    // 子类实现具体开关或节流逻辑
    public abstract void ProcessLogic();

    private void OnDestroy()
    {
        if (SimulationLoop.Instance != null)
            SimulationLoop.Instance.allValves.Remove(this);
    }
}
[System.Serializable]
public enum ValveName
{
    单电控二位三通电磁阀_常开,
    单电控二位三通电磁阀_常闭,
    单气控二位五通气控阀,
    单向节流阀,
    滚轮式二位三通控制阀_常闭,
    滚轮式二位三通控制阀_常开,
    滚轮式二位五通控制阀,
    快速排气节流阀,
    气动二联件,
    气动手滑阀,
    气动双压阀,
    气动梭阀,
    气动延时阀_常开,
    气动延时阀_常闭,
    手动三位四通换向阀_O型,
    双电控二位五通电磁阀,
    双电控三位五通电磁阀_O型,
    双电控三位五通电磁阀_Y型,
    双气控二位五通换向阀,
    双气控三位五通换向阀_O型,
    双气控三位五通换向阀_Y型,
    双气控三位五通换向阀_P型,
    双作用单杆气缸,
    单电控二位五通电磁阀,
    单向压力顺序阀,
    快速排气阀,
    手动二位四通换向阀,
    真空发生器,
    双杆双轴气缸,
    双向摆动回转气缸,
    双向气动马达,
    单作用气缸,
    按钮式二位三通换向阀_常闭,
    按钮式二位五通换向阀,
    机械式二位三通行程阀_常开,
    三通,
    四通,
    主气源


}



