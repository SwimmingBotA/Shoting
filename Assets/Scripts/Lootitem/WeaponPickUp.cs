using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : LootItem
{
    [SerializeField] AudioData scoreSFX;
    [SerializeField] int scoreBonus = 100;
    protected override void PickUp()
    {
        if (player.isMaxweapon)
        {
            lootMessage.text = $"SCORE+{scoreBonus}";
            ScoreManager.Instance.AddScore(scoreBonus);
            pickUpSFX = scoreSFX;
        }
        else
        {
            pickUpSFX = defaultPickUpSFX;
            lootMessage.text = $"POWER UP!";
            player.WeaponPowerUp();
        }


        base.PickUp();
    }
}
