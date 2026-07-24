using UnityEngine;

[RequireComponent(typeof(DynamicNode))]
public class PLCIOMapper_X : MonoBehaviour
{
    [Header("PLC 输入绑定")]
    [Tooltip("对应的 PLC 寄存器名称，例如 X0, X1")]
    public string plcXRegisterName = "X0";

    private DynamicNode myNode;
    private int lastState = -1;
    private ElecPLCControl plcController;

    void Start()
    {
        plcXRegisterName=gameObject.name;
        myNode = GetComponent<DynamicNode>();
        plcController = FindFirstObjectByType<ElecPLCControl>();

        // 注册到 PLC 控制器的桥梁字典中中
        if (plcController != null && !plcController.plcXNodeMapping.ContainsKey(plcXRegisterName))
        {
            plcController.plcXNodeMapping.Add(plcXRegisterName, myNode);
        }
    }

    void Update()
    {
        if (plcController == null || myNode == null) return;

        // 根据接线图物理特性：X点采用漏型接法，外部线路闭合将 Xn 钳位到 0V（Negative）
        // 也就是说：只要全网重构后，我这个节点成功获取到了 Negative（0V）染色，就代表按钮按下了！
        bool isPressed = (myNode.currentPowerStatus == NodeType.Negative && myNode.currentLevel >= 0);
        int currentState = isPressed ? 1 : 0;

        // 状态发生切换时，利用多线程安全接口写入 PLC
        if (currentState != lastState)
        {
            plcController.setinput(plcXRegisterName, currentState);
            lastState = currentState;
            Debug.Log($"[PLC物理映射] 场景节点 {myNode.nodeName} 电位改变 -> 写入 PLC {plcXRegisterName} = {currentState}");
        }
    }
}