using System;
using System.Collections;
using System.Collections.Generic;
using Net.Common;
using UnityEngine;
using UnityEngine.Events;

public class CarState : SingletonMono<CarState>
{
    //整车电源状态
    private bool _carPower=true; //电源断开
    private bool _carUNLock; //锁车状态
    private bool _carOn; //踩下制动踏板on档
    private bool _carReady; //挂挡ready

    public UnityEvent<bool> OnCarPowerChanged = new UnityEvent<bool>();
    public UnityEvent<bool> OnCarLockChanged = new UnityEvent<bool>();
    public UnityEvent<bool> OnCarOnChanged = new UnityEvent<bool>();
    public UnityEvent<bool> OnCarReadyChanged = new UnityEvent<bool>();
    /// <summary>
    /// 车辆蓄电池通电
    /// </summary>
    public bool CarPower
    {
        get => _carPower;
        set
        {
            if (_carPower != value)
            {
                _carPower = value;
            }

            OnCarPowerChanged.Invoke(_carPower);
            if (!CarPower)
            {
                CarUNLock = false;
                CarOn = false;
                CarReady = false;
            }
        }
    }

    /// <summary>
    /// 车辆解锁
    /// </summary>
    public bool CarUNLock
    {
        get => _carUNLock;
        set
        {
            if (CarPower)
            {
                if (_carUNLock != value)
                {
                    _carUNLock = value;
                }

                OnCarLockChanged.Invoke(_carUNLock);
                CarOn = false;
                CarReady = false;
            }
        }
    }
    /// <summary>
    /// 车辆On档（踩下制动踏板）
    /// </summary>
    public bool CarOn
    {
        get => _carOn;
        set
        {
            if (CarPower && CarUNLock)
            {
                if (_carOn != value)
                {
                    _carOn = value;
                }

                OnCarOnChanged.Invoke(_carOn);
                CarReady = false;
            }
        }
    }
/// <summary>
/// 挂挡完成，可以行使 RD
/// </summary>
    public bool CarReady
    {
        get => _carReady;
        set
        {
            if (CarPower && CarUNLock && CarOn)
            {
                if (_carReady != value)
                {
                    _carReady = value;
                }


                OnCarReadyChanged.Invoke(_carReady);
            }
        }
    }
    /// <summary>
    /// 车辆 电瓶， 解锁 On档，Ready档
    /// </summary>
    /// <returns></returns>
    public bool CanControlReady()
    {
        if (CarPower && CarUNLock && CarOn&&CarReady)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 车辆电瓶通电
    /// </summary>
    /// <returns></returns>
    public bool CarPowerReady()
    {
        if (CarPower)
        {
            return true;
        }


        else
        {
            return false;
        }
    }
    /// <summary>
    /// 车辆电源，车辆解锁
    /// </summary>
    /// <returns></returns>
    public bool CarLockReady()
    {
        if (CarPower && CarUNLock)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 车辆电源 解锁，On档
    /// </summary>
    /// <returns></returns>
    public bool CarOnReady()
    {
        if (CarPower && CarUNLock&& CarOn)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.K))
        {
            CarPower = true;
            CarReady = true;
            CarOn = true;
            CarUNLock = true;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log($"车辆供电状态：电源：{_carPower}，解锁:{_carUNLock}，踩下踏板:{_carOn}，ready:{_carReady}");
        }
#endif
    }

}