using UnityEngine;

/// <summary>
/// 场景单例基类：自动创建，且在切换场景时不销毁
/// </summary>
/// <typeparam name="T">继承了 MonoBehaviour 的类</typeparam>
public class SingletoMonon<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            // 防止在退出游戏时调用单例导致报错
            if (_applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] {typeof(T)} 实例在程序退出时被访问，返回 null。");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    // 1. 尝试在场景中寻找已有对象
                    _instance = (T)FindObjectOfType(typeof(T));

                    // 2. 如果场景中没有，则自动创建一个
                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = $"(Singleton) {typeof(T)}";

                        // 保证单例在场景切换时不被销毁
                        DontDestroyOnLoad(singleton);
                    }
                }

                return _instance;
            }
        }
    }

    private static bool _applicationIsQuitting = false;

    /// <summary>
    /// 当程序退出时，Unity 会按随机顺序销毁对象。
    /// 如果单例先被销毁，而其他脚本在 OnDestroy 中又访问了它，会创建出新的“孤儿”实例。
    /// 此变量用于拦截这种情况。
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }

    protected virtual void OnDestroy()
    {
        // 如果手动销毁了此对象，确保重置状态
        if (_instance == this)
        {
            _applicationIsQuitting = true;
        }
    }
}
