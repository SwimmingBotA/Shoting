using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] Pool[] enemeyPools;

    [SerializeField] Pool[] playerProjectilePools;

    [SerializeField] Pool[] enmeyProjectilePools;

    [SerializeField] Pool[] vFXPools;

    [SerializeField] Pool[] LootItems;

    static Dictionary<GameObject, Pool> dictionary;


    private void Awake()
    {
        dictionary = new Dictionary<GameObject, Pool>();

        Initialize(playerProjectilePools);
        Initialize(enmeyProjectilePools);
        Initialize(enemeyPools);
        Initialize(vFXPools);
        Initialize(LootItems);
    }

    #if UNITY_EDITOR
    private void OnDestroy()
    {
        CheackPoolSize(enemeyPools);
        CheackPoolSize(playerProjectilePools);
        CheackPoolSize(enmeyProjectilePools);
        CheackPoolSize(vFXPools);
        CheackPoolSize(LootItems);
    }

    void CheackPoolSize(Pool[] pools)
    {
        foreach (var pool in pools)
        {
            if (pool.RuntimeSize > pool.Size)
            {
                Debug.LogWarning(string.Format("Pool : {0} has a runtime size is {1} bigger than its initial size{2}",
                    pool.Prefab.name,
                    pool.RuntimeSize,
                    pool.Size));
            }
        }
    }
    #endif 

    //初始化Pool池中所有种类子弹
    void Initialize(Pool[] pools)
    {
        foreach (var pool in pools)
        {
            //将预制体的池子加入字典中
            #if UNITY_EDITOR
            if (dictionary.ContainsKey(pool.Prefab))
            {
                Debug.LogError("Same prefab in multiple pools! Prefab:" + pool.Prefab.name);
                continue;
            }
            #endif

            dictionary.Add(pool.Prefab, pool);

            Transform poolParent = new GameObject("Pool:" +pool.Prefab.name).transform;
            poolParent.parent = transform;
            pool.Initialize(poolParent);
        }
    }


    //释放预制体
    public static GameObject Release(GameObject prefab)
    {
    #if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could Not Find Prefab" + prefab.name);
            return null;
        }
    #endif
        return dictionary[prefab].PreparedObject();
    }

    public static GameObject Release(GameObject prefab,Vector3 position)
    {
    #if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could Not Find Prefab" + prefab.name);
            return null;
        }
    #endif
        return dictionary[prefab].PreparedObject(position);
    }

    public static GameObject Release(GameObject prefab,Vector3 position,Quaternion rotation)
    {
    #if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could Not Find Prefab" + prefab.name);
            return null;
        }
    #endif
        return dictionary[prefab].PreparedObject(position, rotation);
    }

    public static GameObject Release(GameObject prefab,Vector3 position,Quaternion rotation,Vector3 localscale)
    {
    #if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could Not Find Prefab" + prefab.name);
            return null;
        }
    #endif
        return dictionary[prefab].PreparedObject(position,rotation,localscale);
    }
}
