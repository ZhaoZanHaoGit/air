using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortBase : MonoBehaviour
{
  
    public portandvalve portandvalve;
    public portChanel chanel;   // 端口类型
    public bool isOccupied = false;   // 是否已连线
    public bool isStackable = false;
    public bool isMain;
    public bool debugOn = false;
    // Start is called before the first frame update
    protected void Awake()
    {

    }

    public virtual void Propagate()
    {
        // 由子类实现：将本端状态传播给连线对端
    }
    public virtual void OnConnect(PortBase port)
    {
        // 由子类实现：接收对端状态并更新自身
    }

    public virtual void AlwaysON()
    {
        // 由子类实现：每帧持续执行的逻辑（如压力平衡）
    }
    public virtual void Disconnect()
    {
        // 由子类实现：断开连接时的逻辑
    }

}
public enum portChanel
{
    none = 0,
    pneumatic,
    tinckElec,
    simplleElec
}
[System.Serializable]
public enum PneumaticPortName
{
    X,Y,A, B, P,R, Z1, Z2,L,M

}
[System.Serializable]
public class portandvalve
{
    public PneumaticPortName port;
    public ValveName valve;

    public string GetPortAndValveInfo()
    {
        return $" Valve: {valve},Port: {port}";
    }
}
