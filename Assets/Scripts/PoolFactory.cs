using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolFactory : MonoBehaviour
{
    // Singleton instance for easy access
    public static PoolFactory Instance;

    // Dictionary to store pools and their actions
    private Dictionary<string, (object pool, Action<Component> onGet, Action<Component> onRelease)> _pools;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        _pools = new Dictionary<string, (object, Action<Component>, Action<Component>)>();
    }

    // Generic method to get objects from the pool
    public T GetObject<T>(T prefab) where T : Component
    {
        string key = prefab.name;

        // Create a pool if it doesn't exist
        if (!_pools.ContainsKey(key))
        {
            var newPool = new ObjectPool<T>(
                () => Instantiate(prefab),
                (obj) => _pools[key].onGet?.Invoke(obj),      // Invoke custom OnGet action
                (obj) => _pools[key].onRelease?.Invoke(obj),  // Invoke custom OnRelease action
                (obj) => Destroy(obj.gameObject)
            );
            _pools[key] = (newPool, null, null); // Initially no actions registered
        }

        var poolEntry = _pools[key];
        var pool = (ObjectPool<T>)poolEntry.pool;
        return pool.Get();
    }

    // Method to set custom actions for a pool
    public void SetPoolActions<T>(T prefab, Action<T> onGet = null, Action<T> onRelease = null) where T : Component
    {
        string key = prefab.name;

        // Create the pool if it doesn't exist
        if (!_pools.ContainsKey(key))   
        {
            var newPool = new ObjectPool<T>(
                () => Instantiate(prefab),
                (obj) => onGet?.Invoke(obj),       // Invoke custom OnGet action
                (obj) => onRelease?.Invoke(obj),   // Invoke custom OnRelease action
                (obj) => Destroy(obj.gameObject)
            );
            _pools[key] = (newPool, (c) => onGet?.Invoke((T)c), (c) => onRelease?.Invoke((T)c));
        }
        else
        {
            // Update the actions for an existing pool
            _pools[key] = (_pools[key].pool, (c) => onGet?.Invoke((T)c), (c) => onRelease?.Invoke((T)c));
        }
    }

    // Method to release objects back to the pool
    public void ReleaseObject<T>(T obj) where T : Component
    {
        string key = obj.name.Replace("(Clone)", "").Trim();
        if (_pools.ContainsKey(key))
        {
            var pool = (ObjectPool<T>)_pools[key].pool;
            pool.Release(obj);
        }
    }
}
