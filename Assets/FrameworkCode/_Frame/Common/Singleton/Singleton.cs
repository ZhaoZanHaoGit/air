
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.单列基类
 *
 *  Author:				
 *       
 *  Date:             
 * 
 ******************************************************************************/

using System;

public class Singleton<T> where T : class, new()
{
    private static T _instance;
    public static T Instance => _instance ?? (_instance = Activator.CreateInstance<T>());

    protected Singleton()
    {
        if (null != _instance)
        {
            var msg = $"This {typeof(T)} Singleton Instance is not null !!!";
            throw new Exception(msg);
        }
        Init();
    }

    public virtual void Init() { }
}
