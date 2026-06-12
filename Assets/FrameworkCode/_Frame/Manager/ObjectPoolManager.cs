
/******************************************************************************
 * 
 *  Title:				Frame
 *
 *  Version:			1.0
 *
 *  Description:
 *  1.对象池单例
 *
 *  Author:			
 *       
 *  Date:              
 * 
 ******************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ObjectPoolManager : DDOLSingleton<ObjectPoolManager>
{
    private readonly List<Pool> _poolList = new List<Pool>();

    public Transform Spawn(Transform trans, Transform parent = null)
    {
        return Spawn(trans, Vector3.zero, Quaternion.identity, parent);
    }

    public Transform Spawn(Transform trans, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        return Spawn(trans.gameObject, pos, rot, parent).transform;
    }

    public GameObject Spawn(GameObject obj, Transform parent = null)
    {
        return Spawn(obj, Vector3.zero, Quaternion.identity, parent);
    }

    public GameObject Spawn(GameObject obj, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        return _Spawn(obj, pos, rot, parent);
    }

    private GameObject _Spawn(GameObject obj, Vector3 pos, Quaternion rot, Transform parent)
    {
        if (obj == null)
        {
            Debug.Log("NullReferenceException: obj unspecified");
            return null;
        }

        var ID = GetPoolID(obj);
        if (ID == -1)
            ID = _New(obj);

        return _poolList[ID].Spawn(pos, rot, parent);
    }

    public void UnSpawnChildren(Transform parent)
    {
        if (parent == null)
        {
            Debug.LogError("UnSpawnChildren parent == null");
            return;
        }

        for (var i = parent.childCount - 1; i >= 0; i--)
        {
            UnSpawn(parent.GetChild(i));
        }
    }

    public void UnSpawn(Transform trans)
    {
        _UnSpawn(trans.gameObject);
    }

    public void UnSpawn(GameObject obj)
    {
        _UnSpawn(obj);
    }

    private void _UnSpawn(GameObject obj)
    {
        for (int i = 0, imax = _poolList.Count; i < imax; i++)
        {
            if (_poolList[i].UnSpawn(obj))
                return;
        }

        Destroy(obj);
    }

    public int New(Transform trans, int count = 1)
    {
        return _New(trans.gameObject, count);
    }

    public int New(GameObject obj, int count = 1)
    {
        return _New(obj, count);
    }

    private int _New(GameObject obj, int count = 1)
    {
        var id = GetPoolID(obj);
        if (id != -1)
        {
            _poolList[id].MachObjectCount(count);
        }
        else
        {
            var pool = new Pool
            {
                Prefab = obj
            };
            pool.MachObjectCount(count);
            _poolList.Add(pool);
            id = _poolList.Count - 1;
        }

        return id;
    }

    public int GetPoolID(GameObject obj)
    {
        for (int i = 0, max = _poolList.Count; i < max; i++)
        {
            if (_poolList[i].Prefab == obj)
                return i;
        }

        return -1;
    }

    public void ClearAll()
    {
        for (int i = 0, max = _poolList.Count; i < max; i++)
            _poolList[i].Clear();
        _poolList.Clear();
    }

    public Transform GetOPMTransform()
    {
        return transform;
    }
}

[System.Serializable]
public class Pool
{
    public GameObject Prefab;
    public List<GameObject> InactiveList = new List<GameObject>();
    public List<GameObject> ActiveList = new List<GameObject>();
    public int Max = 1000;
    public GameObject Spawn(Vector3 pos, Quaternion rot, Transform parent = null)
    {
        GameObject obj;
        if (InactiveList.Count == 0)
        {
            obj = (GameObject)MonoBehaviour.Instantiate(Prefab, pos, rot);
        }
        else
        {
            obj = InactiveList[0];
            InactiveList.RemoveAt(0);
        }
        if (obj == null)
        {
            Debug.LogError("ObjectPoolManager : " + Prefab + "失败");
            return null;
        }
        obj.transform.SetParent(parent, false);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = pos;
        obj.transform.localRotation = rot;
        obj.SetActive(true);
        ActiveList.Add(obj);
        return obj;
    }

    public bool UnSpawn(GameObject obj)
    {
        if (!ActiveList.Contains(obj)) return false;
        obj.SetActive(false);
        obj.transform.SetParent(ObjectPoolManager.Instance.GetOPMTransform());
        InactiveList.Add(obj);
        ActiveList.Remove(obj);
        return true;
    }

    public void MachObjectCount(int count)
    {
        if (count > Max)
            return;

        var currentCount = ActiveList.Count + InactiveList.Count;
        for (var i = currentCount; i < count; i++)
        {
            var obj = Object.Instantiate(Prefab);
            obj.transform.SetParent(ObjectPoolManager.Instance.GetOPMTransform());
            obj.SetActive(false);
            InactiveList.Add(obj);
        }
    }

    public void Clear()
    {
        for (int i = 0, max = InactiveList.Count; i < max; i++)
        {
            if (InactiveList[i] != null)
                Object.Destroy(InactiveList[i]);
        }

        for (int i = 0, max = ActiveList.Count; i < max; i++)
        {
            if (ActiveList[i] != null)
                Object.Destroy(ActiveList[i]);
        }

        InactiveList.Clear();
        ActiveList.Clear();
    }
}
