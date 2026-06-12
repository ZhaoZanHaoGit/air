
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.mono的单例基类
 *
 *  Author:			
 *       
 *  Date:              
 * 
 ******************************************************************************/

using UnityEngine;

public class DDOLSingleton<T> : MonoBehaviour where T : DDOLSingleton<T>//Component
{
    public static GameObject Obj { get; set; }

    private static T _instance;

    public static T Instance
    {
        get
        {
            if (null != _instance) return _instance;
            if (null == Obj)
            {
                Obj = GameObject.Find("DDOLGameObject");
                if (null == Obj)
                {
                    Obj = new GameObject("DDOLGameObject");
                }
            }
            DontDestroyOnLoad(Obj);
            _instance = Obj.GetOrAddComponent<T>();
            return _instance;
        }
    }

    public void OnApplicationQuit()
    {
        _instance = null;
    }

    private void Awake()
    {
        Init();
    }

    public virtual void Init() { Debug.Log(typeof(T)); }
}
