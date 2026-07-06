using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomList<T> : List<T>
{
    public delegate void Action<D>(D item);
    public event Action<T> OnItemAdded;
    public event Action<T> OnItemRemoved;

    public new void Add(T item)
    {
        base.Add(item);
        OnItemAdded?.Invoke(item);
    }

    public new void Remove(T item)
    {
        base.Remove(item);
        OnItemRemoved?.Invoke(item);
    }

    /// <summary>
    /// 清空列表并触发 OnItemRemoved 事件
    /// </summary>
    public new void Clear()
    {
        // 复制当前元素列表，因为 base.Clear 会修改集合
        var snapshot = new List<T>(this);
        base.Clear();
        foreach (var item in snapshot)
        {
            OnItemRemoved?.Invoke(item);
        }
    }

    /// <summary>
    /// 批量重建列表：清空后填入新数据，只触发一次回调。
    /// 用于 SimulationLoop.RebuildGroups() 等需要整体刷新的场景。
    /// </summary>
    public void Rebuild(IEnumerable<T> newItems)
    {
        base.Clear();
        foreach (var item in newItems)
        {
            base.Add(item);
        }
        // 触发一次回调通知外部刷新
        if (Count > 0)
            OnItemAdded?.Invoke(this[0]);
        else
            OnItemRemoved?.Invoke(default);
    }
}
