using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject hitVFX;
    [SerializeField] float damage;
    [SerializeField] AudioData[] hitSFX;

    [SerializeField] protected float speed;
    [SerializeField] protected Vector2 direction;

    protected GameObject target;

    protected virtual void OnEnable()
    {
        StartCoroutine(MoveDirectly());
    }

    IEnumerator MoveDirectly()
    {
        while (gameObject.activeSelf)
        {
            Move();
            yield return null;
        }
    }

    protected void SetTarget(GameObject target) => this.target = target;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Character>(out Character character))
        {
            character.TakeDamage(damage);

            //var point = collision.GetContact(0);

            //PoolManager.Release(hitVFX,point.point,Quaternion.LookRotation(point.normal));

            PoolManager.Release(hitVFX, collision.GetContact(0).point, Quaternion.LookRotation(collision.GetContact(0).normal));

            AudioManager.Instance.PlayRandomSFX(hitSFX);

            gameObject.SetActive(false);
        }
    }

    public void Move() => transform.Translate(direction * speed * Time.deltaTime);
}
