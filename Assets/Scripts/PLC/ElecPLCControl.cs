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

        if (plcOpenState == 0 && ioPanel.transform.parent.gameObject.activeSelf)
        {
            LinkImage.color = Color.green;
            plcLinkText.text = "已连接到plc";
        }
        else if (ioPanel.transform.parent.gameObject.activeSelf)
        {
            LinkImage.color = Color.red;
            string text = "";

            text = "未连接到PLC" + plcOpenState;


            plcLinkText.text = text;
        }

        //        Debug.Log(plcOpenState);
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
                    //Debug.Log(i+"x为"+(InputX[0] & (1 << i)));
                    xImages[i].sprite = LightOff;
                }
            }


        }

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