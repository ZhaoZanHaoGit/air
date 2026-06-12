
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.数据的配置文件
 *
 *  Author:				
 *       
 *  Date:             
 * 
 ******************************************************************************/

using System;
using System.Collections.Generic;

public class TableLoadConfig
{
    public string LocalJsonDirectory { get; set; }

    public List<Type> TableHelperTypeList { get; set; }

    public TableLoadConfig()
    {
        LocalJsonDirectory = "Table/";

        TableHelperTypeList = new List<Type>()
        {
            //todo 添加Tbale文件
        };
    }
}

