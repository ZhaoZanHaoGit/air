using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 车辆信息
/// </summary>
public class CarStatus : MonoBehaviour
{
    public CarInfo carInfo;

    void Start()
    {
        
    }

   
}
[Serializable]
public class CarInfo {
    /// <summary>
    /// 是否是P档
    /// </summary>
    public bool isPGear;
    public CarGearType gearType;

}