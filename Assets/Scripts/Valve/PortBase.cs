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
    public NodeType nodetype;
    // ---> [新增电路节点持有] <---
    public DynamicNode circuitNode;
    // Start is called before the first frame update
    protected void Awake()
    {
        // 如果是电气接口，在初始化时自动挂载并配置电路拓扑节点
        if (chanel == portChanel.tinckElec || chanel == portChanel.simplleElec || circuitNode != null)
        {
           // circuitNode.nodeName = gameObject.name;

            // 这里可以根据实际情况初始化节点类型：
            // 如果这个接头在场景里是固定死代表电源正极的，可以写逻辑改成 Positive
            // 默认都是普通节点 Normal
            circuitNode.type = nodetype;
        }
    }

    public virtual void Propagate()
    {
        // 由子类实现：将本端状态传播给连线对端
    }
    public virtual void OnConnect(PortBase port)
    {


    }

    public virtual void AlwaysON()
    {
        // 由子类实现：每帧持续执行的逻辑（如压力平衡）
    }
    public virtual void Disconnect()
    {
        isOccupied = false;
        // ---> [新增断开逻辑] <---
        // 当断开连接时，如果自身有电路节点状态，重置它
        if (circuitNode != null)
        {
            circuitNode.currentLevel = -1;
            circuitNode.currentPowerStatus = NodeType.Normal;
        }
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
    X, Y, A, B, P, R, Z1, Z2, L, M, v0, v24, L1, L2, L3, N

}
[System.Serializable]
public class portandvalve
{
    public PneumaticPortName port;
    public ValveName valve;

    public string GetPortAndValveInfo()
    {
        return $" {valve}__{port}";
    }
}
