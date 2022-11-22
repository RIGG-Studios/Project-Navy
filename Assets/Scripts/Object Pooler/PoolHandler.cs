using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolHandler : MonoBehaviour
{
    private static PoolManager _poolManager;

    public static void InitializePool(PoolManager manager)
    {
        _poolManager = manager;
        
        _poolManager.InitializePool();
    }

    public static void ReturnObjectToPool(PooledObject pooledObject)
    {
        if (_poolManager == null)
            return;
        
        _poolManager.ReturnToPool(pooledObject);
    }

    public static T GetPooledObject<T>(PooledObject pooledObject, Vector3 position, Quaternion rotation, bool active = true)
    {
        if (_poolManager == null)
            return default(T);

        return _poolManager.GetPooledObject<T>(pooledObject, position, rotation,active);
    }


    public static PooledObject FindPoolByName(string name)
    {
        foreach (PooledObject key in _poolManager.pools.Keys)
        {
            if (name == key.gameObject.name)
                return key;
        }

        return null;
    }
}
