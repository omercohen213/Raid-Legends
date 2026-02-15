using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericObjectPool<T> where T : Component
{
    private readonly Queue<T> _objects = new();
    private readonly Func<T> _createFunc;
    private readonly Action<T> _onGet;
    private readonly Action<T> _onRelease;

    public GenericObjectPool(Func<T> createFunc, Action<T> onGet = null, Action<T> onRelease = null)
    {
        _createFunc = createFunc;
        _onGet = onGet;
        _onRelease = onRelease;
    }

    public T Get()
    {
        T obj;
        if (_objects.Count == 0)
        {
            obj = _createFunc();
        }
        else
        {
            obj = _objects.Dequeue();
        }

        _onGet?.Invoke(obj);
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Release(T obj)
    {
        _onRelease?.Invoke(obj);
        obj.gameObject.SetActive(false);
        _objects.Enqueue(obj);
    }
}
