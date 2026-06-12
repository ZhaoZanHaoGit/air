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
        if (OnItemAdded != null)
        { OnItemAdded.Invoke(item); }

    }

    public new void Remove(T item)
    {
        base.Remove(item);
        if (OnItemRemoved != null)
        { OnItemRemoved.Invoke(item); }

    }

}
