using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Unity.Mathematics;

public class PoolManager : MonoBehaviourPun
{
    [SerializeField] private PoolObjectInfo[] scenePools;
    [SerializeField] private int defaultPoolSize;

    public Dictionary<PooledObject, Pool> pools = new Dictionary<PooledObject, Pool>();

    private bool _intiailized;

    public struct Pool
    {
        public string poolID;
        public PooledObject pooledObject;

        public Transform pooledTransform;
        public Transform activeTransform;

        public int pooledCount => pooledTransform.childCount;
        public int activeCount => activeTransform.childCount;
        public int totalCount => pooledCount + activeCount;

        public void GrowPool(int amt)
        {
            for (int i = totalCount; i < amt; i++)
            {
                PooledObject obj = Instantiate(pooledObject, pooledTransform, true);
                obj.transform.position = Vector3.zero;
                obj.gameObject.SetActive(false);
            }
        }

        public Pool(PooledObject pooledObject, string poolID, int count, Transform pooledTransform,
            Transform activeTransform)
        {
            this.poolID = poolID;
            this.pooledObject = pooledObject;
            this.pooledTransform = pooledTransform;
            this.activeTransform = activeTransform;

            for (int i = totalCount; i < count; i++)
            {
                PooledObject obj = Instantiate(pooledObject, pooledTransform, true);
                pooledObject.poolTransform = pooledTransform;
                obj.transform.position = Vector3.zero;
                obj.gameObject.SetActive(false);
            }
        }

        public T GetObject<T>(bool active)
        {
            if (pooledTransform == null || activeTransform == null)
                return default(T);

            if (pooledCount > 0)
            {
                Transform t = pooledTransform.GetChild(pooledTransform.childCount - 1);
                T result = t.GetComponent<T>();

                if (result != null)
                {
                    t.SetParent(activeTransform);
                }

                return result;
            }

            if (activeTransform.childCount > 0)
            {
                Transform t = activeTransform.GetChild(0);
                T result = t.GetComponent<T>();

                if (result != null)
                {
                    t.gameObject.SetActive(false);
                    t.SetAsLastSibling();
                }

                return result;
            }

            return default(T);
        }
    }

    private void Awake()
    {
        PoolHandler.InitializePool(this);
    }

    public void InitializePool()
    {
        if (_intiailized)
            return;

        for (int i = 0; i < scenePools.Length; i++)
        {
            CreatePool(scenePools[i]);
        }

        _intiailized = true;
    }

    public void CreatePool(PoolObjectInfo info)
    {
        if (info.baseCount < 1)
            info.baseCount = 1;

        if (pools.ContainsKey(info.PooledObject))
        {
            pools[info.PooledObject].GrowPool(info.baseCount);
        }
        else
        {
            Transform root = new GameObject(info.poolID).transform;
            root.SetParent(transform);

            Transform poolTransform = new GameObject("Pool").transform;
            poolTransform.SetParent(root);

            Transform activeTransform = new GameObject("Active").transform;
            activeTransform.SetParent(root);
            
            
            pools.Add(info.PooledObject,
                new Pool(info.PooledObject, info.poolID, info.baseCount, poolTransform, activeTransform));
        }
    }


    public void ReturnToPool(PooledObject pooledObject)
    {
        Pool pool;
        if (pools.TryGetValue(pooledObject, out pool))
        {
            pooledObject.transform.SetParent(pool.pooledTransform, false);
            pooledObject.transform.position = Vector3.zero;
            pooledObject.transform.rotation = quaternion.identity;
            pooledObject.gameObject.SetActive(false);
        }
    }

    public T GetPooledObject<T>(PooledObject pooledObject, Vector3 position, Quaternion rotation, bool active = true)
    {
        Pool pool;

        if (pools.TryGetValue(pooledObject, out pool))
        {
            T result = pool.GetObject<T>(active);
            Component comp = result as Component;

            if (comp != null)
            {
                Transform t = comp.transform;
                t.position = position;
                t.rotation = rotation;
            }

            if (active)
                comp.gameObject.SetActive(true);

            return result;
        }

        PoolObjectInfo info = GetInfo(pooledObject);
        CreatePool(info);

        return GetPooledObject<T>(info.PooledObject, position, rotation);
    }


    private PoolObjectInfo GetInfo(PooledObject pooledObject)
    {
        for (int i = 0; i < scenePools.Length; i++)
        {
            if (pooledObject == scenePools[i].PooledObject)
                return scenePools[i];
        }

        return null;
    }

}

[System.Serializable]
public class PoolObjectInfo
{
    public string poolID;
    public GameObject pooledObject;
    public int baseCount;

    public PooledObject PooledObject => pooledObject.GetComponent<PooledObject>();

    public PoolObjectInfo(string poolID, GameObject pooledObject, int baseCount)
    {
        this.poolID = poolID;
        this.pooledObject = pooledObject;
        this.baseCount = baseCount;
    }
}
