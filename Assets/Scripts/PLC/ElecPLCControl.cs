using System;
using System.Collections.Generic;
using UnityEngine;
using ActUtlType64Lib;
using System.Threading;
using UnityEngine.UI;
using TMPro;

public class ElecPLCControl : MonoBehaviour
{
    Thread linkThread;
    Thread UpdateXThread;
    Thread UpdateYThread;
    public Sprite LightON, LightOff;
    ActUtlType64 plc;
    public int plcOpenstate = -1;
    public TextMeshProUGUI plcLinkText;

    [Header("========= 电路拓扑桥梁映射 =========")]
    [Tooltip("将 PLC 的 X 寄存器名称（如 X0）映射到场景中对应的虚拟节点组件")]
    public Dictionary<string, DynamicNode> plcXNodeMapping = new Dictionary<string, DynamicNode>();

    [Tooltip("将 PLC 的 Y 寄存器名称（如 Y0）映射到场景中对应的虚拟节点组件")]
    public Dictionary<string, DynamicNode> plcYNodeMapping = new Dictionary<string, DynamicNode>();
    public int plcOpenState
    {
        get { return plcOpenstate; }

        set
        {
            plcOpenstate = value;
        }
    }

    int plcCloseState = -1;
    public CustomList<int> InputX = new CustomList<int>();
    public CustomList<int> OutputY = new CustomList<int>();
    public Image LinkImage;
    public GameObject xpanel, ypanel;
    public GameObject ioPanel;
    public Image[] xImages;
    public Image[] yImages;

    public int resultCodeX, resultCodeY;
    void Start()
    {
        plcOpenState = -1;
        resultCodeX = -1;
        resultCodeY = -1;
        InputX.Add(0);
        OutputY.Add(0);
        xImages = xpanel.GetComponentsInChildren<Image>(true);
        yImages = ypanel.GetComponentsInChildren<Image>(true);
        foreach (var VARIABLE in xImages)
        {
            string ioname = VARIABLE.name.Replace("_", "");
            VARIABLE.GetComponentInChildren<TextMeshProUGUI>().text = ioname;
        }

        foreach (var VARIABLE in yImages)
        {
            string ioname = VARIABLE.name.Replace("_", "");
            VARIABLE.GetComponentInChildren<TextMeshProUGUI>().text = ioname;
        }
        
    }

    public void linkStart()
    {

        if (plcOpenState != 0)
        {
            linkThread = new Thread(new ThreadStart(RunDLLMethod));
            linkThread.Start();
            //RunDLLMethod();
        }




    }


    public void LinkStop()
    {
        if (plc != null)
        {
            linkThread = new Thread(new ThreadStart(stopplc));
            linkThread.Start();

        }
    }


    void RunDLLMethod()
    {
        if (plc == null)
        {
            plc = new ActUtlType64();
        }
        plc.ActLogicalStationNumber = 1;
        plcOpenState = plc.Open();
        plcCloseState = -1;
        if (plcOpenState == 0)
        {
            Thread UpdateXThread = new Thread(PlcReadAllX);
            UpdateXThread.Start();
            Thread UpdateYThread = new Thread(PlcReadAllY);
            UpdateYThread.Start();
        }
    }

    void stopplc()
    {
        plcCloseState = plc.Close();
        plcOpenState = -1;
        resultCodeX = -1;
        resultCodeY = -1;
        plc = null;
    }

    public void setinput(string name, int state)
    {
        if (plcOpenState == 0)
        {
            Debug.Log("plc写入");
            PLCInputData inputData = new PLCInputData(state, name);
            Thread setThread = new Thread(() => SetX(inputData.name, inputData.state));
            setThread.Start();
        }
    }

    void SetX(string name, int state)
    {
        lock (plc)
        {
            int x = -1;
            x = plc.SetDevice(name, state);
        }
    }

    /// <summary>
    /// 读取全部x节点的值
    /// </summary>
    void PlcReadAllX()
    {
        while (plcOpenState == 0)
        {
            int[] temps = new int[16];
            lock (InputX)
            {
                resultCodeX = plc.ReadDeviceBlock("X000", 16, out temps[0]);
                if (resultCodeX == 0&& temps.Length>0)
                {
                    InputX.Clear();
                    InputX.AddRange(temps);
                }

            }


            Thread.Sleep(10);
        }
    }

    void PlcReadAllY()
    {
        while (plcOpenState == 0)
        {
            int[] temps = new int[16];
            lock (OutputY)
            {
                resultCodeY = plc.ReadDeviceBlock("Y000", 16, out temps[0]);
                if (resultCodeY == 0)
                {
                    OutputY.Clear();
                    OutputY.AddRange(temps);
                }

            }
            Thread.Sleep(10);
        }
    }

    private void Update()
    {
        // ==========================================
        // 1. PLC 连接状态 UI 刷新 (保留您原有的逻辑)
        // ==========================================
        if (plcOpenState == 0 && ioPanel.transform.parent.gameObject.activeSelf)
        {
            LinkImage.color = Color.green;
            plcLinkText.text = "已连接到PLC";
        }
        else if (ioPanel.transform.parent.gameObject.activeSelf)
        {
            LinkImage.color = Color.red;
            string text = "未连接到PLC" + plcOpenState;
            plcLinkText.text = text;
        }

        // ==========================================
        // 2. 输入 X 状态灯显示 (保留您原有的逻辑)
        // ==========================================
        if (resultCodeX == 0 && ioPanel.activeSelf)
        {
            for (int i = 0; i < xImages.Length; i++)
            {
                bool isOn = (InputX[0] & (1 << i)) != 0;
                if (isOn)
                {
                    xImages[i].sprite = LightON;
                }
                else
                {
                    xImages[i].sprite = LightOff;
                }
            }
        }

        // ==========================================
        // 3. 输出 Y 状态灯显示 (保留您原有的逻辑)
        // ==========================================
        if (resultCodeY == 0 && ioPanel.activeSelf)
        {
            for (int i = 0; i < yImages.Length; i++)
            {
                bool isOn = (OutputY[0] & (1 << i)) != 0;
                if (isOn)
                {
                    yImages[i].sprite = LightON;
                }
                else
                {
                    yImages[i].sprite = LightOff;
                }
            }
        }

        // ==========================================
        // 4. 新增：PLC Y点输出 转换为 场景电路电源源头
        // ==========================================
        if (plcOpenState == 0 && resultCodeY == 0)
        {
            bool topologyNeedsRefresh = false;

            // 遍历所有注册到映射字典里的 Y 点节点
            foreach (var kvp in plcYNodeMapping)
            {
                string yRegisterName = kvp.Key;       // 例如 "Y0", "Y1"
                DynamicNode virtualNode = kvp.Value;  // 对应的场景接线柱节点

                if (virtualNode == null) continue;

                // 解析寄存器编号 (如从 "Y0" 中提取出整数 0)
                int yIndex = 0;
                if (int.TryParse(yRegisterName.Replace("Y", "").Replace("y", ""), out yIndex))
                {
                    // 从 OutputY[0] 的位图中判断当前 Y 寄存器是否输出高电平 (1)
                    bool isYOn = (OutputY[0] & (1 << yIndex)) != 0;

                    // 根据接线图（COM接+24V），Y 点导通时向外吐出 +24V 电压
                    // 导通时节点类型设为 Positive（正极源头），断开时还原为 Normal（普通节点）
                    NodeType targetType = isYOn ? NodeType.Positive : NodeType.Normal;

                    // 状态发生切换时才修改节点，避免频繁触发重构
                    if (virtualNode.type != targetType)
                    {
                        virtualNode.type = targetType;

                        // 动态更新源头属性
                        virtualNode.currentPowerStatus = targetType;
                        virtualNode.currentLevel = isYOn ? 0 : -1;

                        // 标记电路拓扑发生改变
                        topologyNeedsRefresh = true;
                    }
                }
            }

            // 如果本帧内有 Y 点输出状态切换，通知全局电路大脑在帧末重新计算并洗牌拓扑
            if (topologyNeedsRefresh && DynamicCircuitManager.Instance != null)
            {
                DynamicCircuitManager.Instance.MarkTopologyDirty();
            }
        }
    }


    void OnDestroy()
    {
        LinkStop();
    }
}

public class PLCInputData
{
    public int state;
    public string name;

    public PLCInputData(int number, string text)
    {
        state = number;
        name = text;
    }
}