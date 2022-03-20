using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [SerializeField] LootItemSetting[] lootItemSettings;

    public void Spawn(Vector2 position)
    {
        foreach (var item in lootItemSettings)
        {
            item.Spawn(position+Random.insideUnitCircle);
        }
    }
}
