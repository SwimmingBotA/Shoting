using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]public class LootItemSetting
{
    public GameObject prefab;
    [Header("µôÂÊ")]
    [Range(0f,100f)] public float dropPercentage;

    public void Spawn(Vector3 position)
    {
        if (Random.Range(0f, 100f) <= dropPercentage)
        {
            PoolManager.Release(prefab, position);
        }
        
    }
}
