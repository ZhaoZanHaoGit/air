using UnityEngine;

[RequireComponent(typeof(DynamicNode))]
public class PLCIOMapper_Y : MonoBehaviour
{
    [Header("PLC 输出绑定")]
    [Tooltip("对应的 PLC 寄存器名称，例如 Y0, Y1")]
    public string plcYRegisterName = "Y0";

    void Start()
    {
        plcYRegisterName = gameObject.name;
        DynamicNode myNode = GetComponent<DynamicNode>();
        ElecPLCControl plcController = FindFirstObjectByType<ElecPLCControl>();

        // 注册到 PLC 字典中，让控制器在 Update 里接管本节点的电源属性
        if (plcController != null && !plcController.plcYNodeMapping.ContainsKey(plcYRegisterName))
        {
            plcController.plcYNodeMapping.Add(plcYRegisterName, myNode);
        }
    }
}