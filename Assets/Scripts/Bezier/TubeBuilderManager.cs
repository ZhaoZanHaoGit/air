using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using HighlightPlus;

public class TubeBuilderManager : MonoBehaviour
{
    public static TubeBuilderManager Instance;

    [Header("配置")]
    public portChanel CurrentjointChanel; // 特定Tag
    public GameObject pneumaticPrefab, xixianPrefab, cuxianPrefab;    // 带有 BezierRendererInverse 脚本的气管预制体

    private bool isSelecting = false;
    private Transform firstJoint;
    private Transform secondJoint;

    private void Awake() => Instance = this;


    public void StartBuildPneumaticTube()
    { StartBuildTube("pneumatic"); }

    public void StartBuildtinckwireTube()
    { StartBuildTube("tinckElec"); }
    public void StartBuildwireTube()
    { StartBuildTube("simplleElec"); }

    // --- 1. UI 按钮调用的方法 ---
    public void StartBuildTube(string tubeType)
    {
        EndSelection();
        isSelecting = true;
        firstJoint = null;
        secondJoint = null;
        CurrentjointChanel= (portChanel)System.Enum.Parse(typeof(portChanel), tubeType);
        // 查找并高亮所有接头
        GameObject[] joints = GameObject.FindGameObjectsWithTag(tubeType);

        if (joints.Length < 2)
        {
            Debug.LogWarning("没有找到足够链接 Tag '" + tubeType + "' 的接头对象！");
            return;
        }
        foreach (var joint in joints)
        {
            PortBase port = joint.GetComponent<PortBase>();
            if (port != null && port.isOccupied && !port.isStackable)
            {
            //    Debug.LogWarning("该接头已经连接！");

            }
            else
            {
                // 这里调用你的高亮架构
                SetJointHighlight(joint, true);
            }

        }
   //     Debug.Log("进入放置模式，请选择第一个接头");
    }

    // --- 2. 交互逻辑 ---
    private void Update()
    {
        if (!isSelecting) return;

        if (GetInputDown(out Vector3 screenPos))
        {
            // 1. 排除真正的 UI（仅限 Canvas 上的元素）
            if (IsClickingRealUI(screenPos))
            {
       //         Debug.Log("点到了真正的 UI 按钮，跳过逻辑");
                return;
            }

            // 2. 执行你的接头检测逻辑
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                //Debug.Log("射线击中了 3D 物体: " + hit.transform.name);

                if (hit.transform.CompareTag(CurrentjointChanel.ToString()))
                {
                    HandleJointSelection(hit.transform);
                }
            }
        }
    }

    private void HandleJointSelection(Transform jointTransform)
    {
        PortBase port = jointTransform.GetComponent<PortBase>();
        if (port != null && !port.isStackable&& port.isOccupied )
        {
            //Debug.LogWarning("该接头已经连接了气管！");
            return;
        }

        if (firstJoint == null)
        {
            firstJoint = jointTransform;
          //  Debug.Log("已选择第一个接头: " + firstJoint.name);
        }
        else if (secondJoint == null && (jointTransform != firstJoint))
        {
            secondJoint = jointTransform;
          //  Debug.Log("已选择第二个接头: " + secondJoint.name);
            CreateTube();
        }
    }

    private void CreateTube()
    {
        GameObject newTube = null;

        // 1. 实例化正确的管线预制体
        switch (CurrentjointChanel)
        {
            case portChanel.pneumatic:
                newTube = Instantiate(pneumaticPrefab);
                break;
            case portChanel.tinckElec:
                newTube = Instantiate(xixianPrefab); // 确保已挂载 DynamicWire
                break;
            case portChanel.simplleElec:
                newTube = Instantiate(cuxianPrefab);  // 确保已挂载 DynamicWire
                break;
        }

        if (newTube == null) return;
        newTube.transform.position = Vector3.zero;

        // 2. 获取两端的物理接口组件
        PortBase portA = firstJoint.GetComponent<PortBase>();
        PortBase portB = secondJoint.GetComponent<PortBase>();

        // 3. 【核心兼容适配修改】：分流处理电气导线和气动管线
        if (CurrentjointChanel == portChanel.tinckElec || CurrentjointChanel == portChanel.simplleElec)
        {
            // 抓取全新的多态电线组件
            DynamicWire wireTopology = newTube.GetComponent<DynamicWire>();

            if (wireTopology != null && portA != null && portB != null)
            {
                Debug.Log($"<color=green>[兼容适配成功]</color> 成功抓取到新电线组件，开始初始化拓扑边...");
                // 传入两端 PortBase 上 Awake 时自动动态生成的电路节点 (circuitNode)
                wireTopology.SetupWire(portA.circuitNode, portB.circuitNode);
            }
            else
            {
                Debug.LogError($"<color=red>[组件丢失错误]</color> 电线预制体上未找到全新的 DynamicWire 脚本，或者接口缺失！");
            }
        }
        else
        {
            // 如果是纯气动管线（pneumatic），依然保留你原本的老版 bezier 赋值逻辑
            var oldBezier = newTube.GetComponent<linerendeler>();
            if (oldBezier != null)
            {
                oldBezier.p0 = firstJoint;
                oldBezier.p2 = secondJoint;
            }
        }

        // 4. 保持你原系统的物理状态绑定（让接头知道自己被占用了）
        if (portA != null && portB != null)
        {
            portA.OnConnect(portB);
            portB.OnConnect(portA);
            portA.isOccupied = true;
            portB.isOccupied = true;
        }

        // 5. 结束选择模式，关闭高亮
        EndSelection();
    }

    private void EndSelection()
    {
        //Debug.Log("清除选择信息");
        isSelecting = false;
        GameObject[] joints = GameObject.FindGameObjectsWithTag(CurrentjointChanel.ToString());

        foreach (var joint in joints)
        {
            SetJointHighlight(joint, false);
        }
    }

    // --- 工具方法 ---
    private bool GetInputDown(out Vector3 pos)
    {
        pos = Vector3.zero;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            pos = Input.GetTouch(0).position;
            return true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            pos = Input.mousePosition;
            return true;
        }
        return false;
    }

    // 高亮架构预留
    private void SetJointHighlight(GameObject obj, bool active)
    {
        obj.GetComponent<HighlightEffect>().highlighted = active;
        // 此处接入你的高亮插件或代码
        // 例如：obj.GetComponent<Outline>().enabled = active;
    }

    private bool IsPointerOverUI()
    {
        // 如果 EventSystem 为空或者根本没激活，直接返回 false
        if (EventSystem.current == null || !EventSystem.current.enabled) return false;

        int pointerId = -1;
        if (Input.touchCount > 0) pointerId = Input.GetTouch(0).fingerId;

        // 如果连 Canvas 都关了，理论上不应该有任何物体被射线击中
        return EventSystem.current.IsPointerOverGameObject(pointerId);
    }

    string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }

    // 专门用来过滤“真正的 UI”的方法
    private bool IsClickingRealUI(Vector2 pos)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = pos;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var res in results)
        {
            // 只有当物体属于 UI 层（Layer 5），才判定为点击了 UI
            if (res.gameObject.layer == 5)
            {
                return true;
            }
        }
        return false;
    }
}