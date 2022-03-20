using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MisslePickUp : LootItem
{
    protected override void PickUp()
    {
        player.PickUpMissle();
        base.PickUp();
    }
}
