using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    BossHealthBar healthBar;

    Canvas healthBarCanvas;

    protected override void Awake()
    {
        base.Awake();
        healthBar = FindObjectOfType<BossHealthBar>();
        healthBarCanvas = healthBar.GetComponentInChildren<Canvas>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        healthBar.Initialized(health, maxHealth);
        healthBarCanvas.enabled = true;
    }

    private void OnDisable()
    {
        healthBarCanvas.enabled = false;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.Die();
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        healthBar.UpdateStats(health, maxHealth);
    }

    public override void Die()
    {
        healthBarCanvas.enabled = false;
        base.Die();
    }

    protected override void SetHealth()
    {
        maxHealth += EnemyManager.Instance.WaveNumber * healthFactor;
    }
}
