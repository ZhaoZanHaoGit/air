
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.资源加载单例
 *
 *  Author:				
 *       
 *  Date:             
 * 
 ******************************************************************************/


using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetInfo<T> where T : UnityEngine.Object
{
    private T _loadObj;
    public string Path { get; set; }
    public int RefCount { get; set; }

    public bool IsLoaded => !ReferenceEquals(_loadObj, null);

    public T AssetObject
    {
        get
        {
            if (ReferenceEquals(_loadObj, null))
                resourcesLoad();
            return _loadObj;
        }
    }

    #region public function
    //协程加载
    public IEnumerator GetObjectByCoroutine(Action<T> loaded)
    {
        while (ReferenceEquals(_loadObj, null))
        {
            yield return null;
            resourcesLoad();
        }
        loaded?.Invoke(_loadObj);
    }

    //异步加载
    public IEnumerator GetObjectAsync(Action<T> loaded)
    {
        return GetObjectAsync(loaded, null);
    }

    public IEnumerator GetObjectAsync(Action<T> loaded, Action<float> progress)
    {
        if (!ReferenceEquals(_loadObj, null))
        {
            loaded?.Invoke(_loadObj);
            yield break;
        }
        var request = Resources.LoadAsync(Path);
        if (!ReferenceEquals(progress, null))
        {
            while (!request.isDone)
            {
                progress(request.progress);
            }
        }
        yield return request;
        if (ReferenceEquals(request.asset, null))
        {
            if (ReferenceEquals(_loadObj, null))
                Debug.LogErrorFormat($"Resources Load Failure! Path:{Path}");
            else
            {
                _loadObj = request.asset as T;
                loaded?.Invoke(_loadObj);
                yield return request;
            }
        }
        else
        {
            _loadObj = request.asset as T;
            loaded?.Invoke(_loadObj);
            yield return request;
        }
    }
    #endregion

    #region private function
    private void resourcesLoad()
    {
        try
        {
            _loadObj = Resources.Load<T>(Path);
            if (ReferenceEquals(_loadObj, null))
                Debug.LogErrorFormat($"Resources Load Failure! Path:{Path}");
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }
    #endregion
}


public class ResManager : Singleton<ResManager>
{
    //资源缓存集合
    private Hashtable _hashTable;

    //初始化
    public override void Init()
    {
        _hashTable = new Hashtable();
    }

    #region public function

    public bool HasCache<T>(string path) where T : UnityEngine.Object
    {
        var hashKey = string.Format($"{path}{typeof(T)}");
        return _hashTable.ContainsKey(hashKey);
    }

    //load
    public GameObject LoadPrefab(string path)
    {
        return Load<GameObject>(path);
    }

    public UnityEngine.UI.Image LoadImage(string path)
    {
        return Load<UnityEngine.UI.Image>(path);
    }

    public Font LoadFont(string path)
    {
        return Load<Font>(path);
    }

    public T Load<T>(string path) where T : UnityEngine.Object
    {
        var info = getAssetInfo<T>(path);
        return info?.AssetObject;
    }

    //Instance
    public T LoadInstance<T>(string path) where T : UnityEngine.Object
    {
        var obj = Load<T>(path);
        return Instantiate(obj);
    }

    //Instance & Coroutine
    public void LoadInstanceCoroutine(string path, Action<GameObject> loaded)
    {
        LoadInstanceCoroutine<GameObject>(path, loaded);
    }

    public void LoadInstanceCoroutine<T>(string path, Action<T> loaded) where T : UnityEngine.Object
    {
        LoadCoroutine<T>(path, obj => { Instantiate<T>(obj, loaded); });
    }

    public void LoadCoroutine<T>(string path, Action<T> loaded) where T : UnityEngine.Object
    {
        var info = getAssetInfo(path, loaded);
        if (!ReferenceEquals(info, null))
            CoroutineController.Instance.StartCoroutine(info.GetObjectByCoroutine(loaded));
    }

    //Instance & Async
    public void LoadInstanceAsync(string path, Action<GameObject> loaded)
    {
        LoadInstanceAsync<GameObject>(path, loaded);
    }

    public void LoadInstanceAsync<T>(string path, Action<T> loaded) where T : UnityEngine.Object
    {
        LoadAsync<T>(path, obj => { Instantiate<T>(obj, loaded); });
    }

    public void LoadInstanceAsync(string path, Action<GameObject> loaded, Action<float> progress)
    {
        LoadInstanceAsync<GameObject>(path, loaded, progress);
    }

    public void LoadInstanceAsync<T>(string path, Action<T> loaded, Action<float> progress) where T : UnityEngine.Object
    {
        LoadAsync<T>(path, obj => { Instantiate<T>(obj, loaded); }, progress);
    }

    public void LoadAsync<T>(string path, Action<T> loaded) where T : UnityEngine.Object
    {
        LoadAsync<T>(path, loaded, null);
    }

    public void LoadAsync<T>(string path, Action<T> loaded, Action<float> progress) where T : UnityEngine.Object
    {
        var info = getAssetInfo<T>(path, loaded);
        if (!ReferenceEquals(info, null))
            CoroutineController.Instance.StartCoroutine(info.GetObjectAsync(loaded, progress));
    }

    //释放资源
    public void UnloadUnusedAssets()
    {
        Resources.UnloadUnusedAssets();
    }
    #endregion

    #region private function

    private AssetInfo<T> getAssetInfo<T>(string path) where T : UnityEngine.Object
    {
        return getAssetInfo<T>(path, null);
    }
    private AssetInfo<T> getAssetInfo<T>(string path, Action<T> loaded) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Error: null path name.");
            loaded?.Invoke(null);
        }
        else
        {
            AssetInfo<T> info;
            var hashKey = string.Format($"{path}{typeof(T)}");
            if (!_hashTable.ContainsKey(hashKey))
            {
                info = new AssetInfo<T>
                {
                    Path = path
                };
                _hashTable.Add(hashKey, info);
            }
            else
                info = _hashTable[hashKey] as AssetInfo<T>;
            info.RefCount++;
            return info;
        }
        return null;
    }

    private T Instantiate<T>(T obj) where T : UnityEngine.Object
    {
        return Instantiate<T>(obj, null);
    }
    private T Instantiate<T>(T obj, Action<T> loaded) where T : UnityEngine.Object
    {
        var retObj = default(T);
        if (!ReferenceEquals(obj, null))
        {
            retObj = Object.Instantiate(obj);
            if (!ReferenceEquals(retObj, null))
                loaded?.Invoke(retObj);
            else
                Debug.LogError("Error: null Instantiate retObj.");
        }
        else
            Debug.LogError("Error: null Resources Load return obj.");
        return retObj;
    }
    #endregion
}
