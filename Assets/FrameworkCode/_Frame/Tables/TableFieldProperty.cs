
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.数据类
 *
 *  Author:				
 *       
 *  Date:              
 * 
 ******************************************************************************/

using System;
using System.Reflection;

public class TableFieldProperty
{
    public string ItemName { get; set; }
    public Type ItemType { get; set; }
    public MethodInfo Method { get; set; }
}

