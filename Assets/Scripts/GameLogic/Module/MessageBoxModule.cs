
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.消息提示面板的数据
 *
 *  Author:				
 *       
 *  Date:               
 * 
 ******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBoxModule : BaseModule
{

    public string Title { get; set; }
    public string Content { get; set; }
    public int CountTime { get; set; }
    public EnumMessageBoxType MessageType { get; set; }

    public MethodAction BtnOK;
    public MethodAction BtnRelease;

    public object BtnOKParam;
    public object BtnReleaseParam;

    public MessageBoxModule()
    {
        AutoRegister = true;
    }

    /// <summary>
    /// 点击事件
    /// </summary>
    /// <param name="btnNum">按钮类型</param>
    /// <param name="isConfirm"></param>
    public void Send(bool isConfirm)
    {
        switch (MessageType)
        {
            case EnumMessageBoxType.OK:
                BtnOK?.Invoke(BtnOKParam);
                break;
            case EnumMessageBoxType.OKCancel:
                if (isConfirm)
                    BtnOK?.Invoke(BtnOKParam);
                else
                    BtnRelease?.Invoke(BtnReleaseParam);
                break;
        }
        BtnOK = null;
        BtnRelease = null;
        BtnOKParam = null;
        BtnRelease = null;
    }
}
