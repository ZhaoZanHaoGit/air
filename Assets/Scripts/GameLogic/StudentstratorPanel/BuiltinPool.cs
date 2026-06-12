using System.Collections.Generic;
using UnityEngine;

public class BuiltinPool : MonoBehaviour
{
    public GameObject prefab;
    private UnityEngine.Pool.ObjectPool<GameObject> _pool; // 显式写全名更稳
    private readonly List<GameObject> objectpools = new List<GameObject>(); // 在用对象列表

    // 为了让每一批生成的顺序可预测：同一个父节点下从 0 开始排
    private int _nextIndex = 0;
    private Transform _lastParent = null;

    void Awake()
    {
        _pool = new UnityEngine.Pool.ObjectPool<GameObject>(
            () => {
                var go = Instantiate(prefab);
                go.SetActive(false);   // 先禁用，取出时再启用
                return go;
            },
            go => go.SetActive(true),   // actionOnGet
            go => go.SetActive(false),  // actionOnRelease（回收仅禁用，不销毁）
            go => Destroy(go),          // actionOnDestroy
            collectionCheck: true,      // 开发期开启可发现重复 Release
            defaultCapacity: 50,
            maxSize: 500
        );
    }

    public GameObject Spawn(Transform parent)
    {
        var go = _pool.Get();
        var t = go.transform;

        // 如果换了父节点，重新从 0 编号，保证每个父节点下都是确定顺序
        if (_lastParent != parent)
        {
            _lastParent = parent;
            _nextIndex = 0;
        }

        // 作为 parent 的子物体，本地清零
        t.SetParent(parent, worldPositionStays: false);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;

        // 稳定兄弟顺序：把它插到 _nextIndex；再自增
        // Clamp 防止索引越界（childCount-1 因为已经成为子物体）
        t.SetSiblingIndex(Mathf.Clamp(_nextIndex, 0, parent.childCount - 1));
        _nextIndex = Mathf.Min(_nextIndex + 1, parent.childCount); // 下一个位置

        // 记录在用对象，便于批量回收
        objectpools.Add(go);
        return go;
    }

    public void Despawn(GameObject go)
    {
        if (go == null) return;
        objectpools.Remove(go); // 从在用列表移除
        _pool.Release(go);      // 归还到池（会被禁用）
    }

    // 批量回收当前所有在用对象，并重置顺序计数
    public void Despawn()
    {
        // 拷贝一份，避免遍历时修改集合
        var tmp = new List<GameObject>(objectpools);
        foreach (var item in tmp)
        {
            if (item != null)
                _pool.Release(item);
        }
        objectpools.Clear();

        // 新一批生成时从 0 重新排；父级保持上一次的
        _nextIndex = 0;
    }

    // 如需彻底清空（销毁池里“闲置”的对象），可加这个辅助方法：
    public void ClearInactiveInPool()
    {
        _pool.Clear(); // 仅销毁未借出的对象；在用的请先 Despawn()
    }

    public int GetCounts()
    {
        return objectpools.Count; // 当前“在用”对象数量
    }
}
