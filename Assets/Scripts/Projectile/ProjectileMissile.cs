using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMissile : PlayerProjectileOverdrive
{
    [SerializeField] AudioData targetAcquireVoice = null;

    [Header("----SPEED----")]
    [SerializeField] float lowSpeed = 8f;
    [SerializeField] float hightSpeed = 25f;
    [SerializeField] float variableSpeedDelay = 0.5f;

    WaitForSeconds waitVariableSpeedDelay;

    [Header("----EXPLOSION----")]
    [SerializeField] float explosionRadius = 3f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float explosionDamage = 50f;

    protected override void Awake()
    {
        base.Awake();
        waitVariableSpeedDelay = new WaitForSeconds(variableSpeedDelay);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(nameof(VariableSpeedCoroutine));
    }


    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);


        foreach (var other in Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer))
        {
            if (other.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(explosionDamage);
            }
        }

    }

    

    IEnumerator VariableSpeedCoroutine()
    {
        speed = lowSpeed;
        yield return waitVariableSpeedDelay;
        speed = hightSpeed;

        if (target != null)
        {
            AudioManager.Instance.PlayRandomSFX(targetAcquireVoice);
        }
    }
}
