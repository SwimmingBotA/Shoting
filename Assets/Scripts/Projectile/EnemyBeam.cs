using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeam : MonoBehaviour
{
    [SerializeField] float damage = 50f;
    [SerializeField] GameObject hitVFX;

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player character))
        {
            character.TakeDamage(damage);

            //var point = collision.GetContact(0);

            //PoolManager.Release(hitVFX,point.point,Quaternion.LookRotation(point.normal));

            PoolManager.Release(hitVFX, collision.GetContact(0).point, Quaternion.LookRotation(collision.GetContact(0).normal));
        }
    }
}
