using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiledPickUp : LootItem
{
    [SerializeField] int scoreBonus = 200;
    [SerializeField] float shieldBonus = 20f;
    [SerializeField] AudioData fullHealthSFX;

    protected override void PickUp()
    {
        if (player.isMaxHealth)
        {
            lootMessage.text = $"SCORE+{scoreBonus}";
            ScoreManager.Instance.AddScore(scoreBonus);
            pickUpSFX = fullHealthSFX;
        }
        else
        {
            pickUpSFX = defaultPickUpSFX;
            lootMessage.text = $"SHIELD+{shieldBonus}";
            player.RestoreHealth(shieldBonus);
        }
        base.PickUp();
    }
}
