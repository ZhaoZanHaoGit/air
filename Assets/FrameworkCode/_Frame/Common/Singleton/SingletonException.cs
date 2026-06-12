
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.“Ï≥£
 *
 *  Author:			
 *       
 *  Date:				
 * 
 ******************************************************************************/
using System;

public class SingletonException : Exception
{
    public SingletonException(string msg) : base(msg)
    {
        UIManager.Instance.OpenMessageBoxUI(msg);
    }
}
