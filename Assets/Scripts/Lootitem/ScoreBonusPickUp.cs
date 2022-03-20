using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBonusPickUp : LootItem
{

    [SerializeField] int bonusScore;
    protected override void PickUp()
    {
        ScoreManager.Instance.AddScore(bonusScore);
        base.PickUp();
        
    }
}
