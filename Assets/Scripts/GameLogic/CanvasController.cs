/******************************************************************************
 *
 *  Title:			    Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.Canvas控制脚本
 *
 *  Author:
 *
 *  Date:
 *
 ******************************************************************************/

using Net.Component;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class CanvasController : DDOLSingleton<CanvasController>
{
    private RectTransform _mainUIParent;

    public RectTransform MainUIParent =>
        _mainUIParent ??= UnityHelper.GetTheChildComponent<RectTransform>(gameObject, "MainUIParent");

    public Stack<UIInfoData> _stackOpenUIOrder;

    /// <summary>
    /// 记录需要按顺序关闭的UI面板
    /// </summary>
    public Stack<UIInfoData> StackOpenUIOrder => _stackOpenUIOrder ??= new Stack<UIInfoData>();


    


    public override void Init()
    {
#if Local_Debug
        Debug.unityLogger.logEnabled = false;
#else
        Debug.unityLogger.logEnabled = true;
#endif
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void Start()
    {
        ModuleManager.Instance.RegisterAllModules();
        TableManager.InitData();
        AppController.Instance.Init();
        NetHelper.Instance.Init();
        LoadHelper.Instance.Init();
        PhpHelper.Instance.Init();
        AudioManager.Instance.Init();
        UIManager.Instance.OpenUI(EnumUIType.LoginUI);
        GetPaths();
        GetModelsData();
        ClientManager.Instance.ip = AppController.Instance.pathDatas.Find(a => a.ID == 0).Path;
        ClientManager.Instance.startConnect = true;
    }

    void GetPaths()
    {
        DataSet dataSet = OpExcel.GetDataSet(Application.streamingAssetsPath + "/PathTable.xlsx");
        if (dataSet != null && dataSet.Tables[0].Rows.Count > 1)
        {
            AppController.Instance.pathDatas.Clear();
            for (int i = 1; i < dataSet.Tables[0].Rows.Count; i++)
            {
                PathData data = new PathData
                {
                    ID = int.Parse(dataSet.Tables[0].Rows[i][0].ToString()),
                    Des = dataSet.Tables[0].Rows[i][1].ToString(),
                    Path = dataSet.Tables[0].Rows[i][2].ToString()
                };
                AppController.Instance.pathDatas.Add(data);
            }
        }
    }

    void GetModelsData()
    {
        AppController.Instance.ModelsDatas.Clear();
        //if (AppController.Instance.ModelsDatas.Count > 0) return;
        DataSet dataSet = OpExcel.GetDataSet(Application.streamingAssetsPath + "/Models.xlsx");
        if (dataSet != null && dataSet.Tables[0].Rows.Count > 1)
        {
            for (int i = 1; i < dataSet.Tables[0].Rows.Count; i++)
            {
                ModelsInfoData data = new ModelsInfoData
                {
                    ID = int.Parse(dataSet.Tables[0].Rows[i][0].ToString()),
                    Type = int.Parse(dataSet.Tables[0].Rows[i][1].ToString()),
                    Name = dataSet.Tables[0].Rows[i][2].ToString(),
                    ModelName = dataSet.Tables[0].Rows[i][3].ToString(),
                    IconName = dataSet.Tables[0].Rows[i][4].ToString(),
                    TextureName = dataSet.Tables[0].Rows[i][5].ToString(),
                    Intro = dataSet.Tables[0].Rows[i][6].ToString(),
                    Principle = dataSet.Tables[0].Rows[i][7].ToString(),
                    IsAnimation = int.Parse(dataSet.Tables[0].Rows[i][8].ToString())
                };
                AppController.Instance.ModelsDatas.Add(data);
            }
        }
    }

    //按下Escape时并无要关闭的界面所要做的操作
    private void doEscapeDown()
    {
        UIManager.Instance.CloseUIAll();
    }


    #region private function

    public void DoQuitGame(object args)
    {
        Application.Quit();
    }

    #endregion
}