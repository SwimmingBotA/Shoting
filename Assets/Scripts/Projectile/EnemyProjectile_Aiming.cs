using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile_Aiming : Projectile
{
    private void Awake()
    {
       SetTarget(GameObject.FindObjectOfType<Player>().gameObject);
    }

    protected override void OnEnable()
    {
        StartCoroutine(nameof(MoveDirectionCoroutine));
        base.OnEnable();
    }

    IEnumerator MoveDirectionCoroutine()
    {
        yield return null;

        if (target.activeSelf)
        {
            direction = (target.transform.position - transform.position).normalized;
        }
    }
}
