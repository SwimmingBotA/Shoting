using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] int scorePoint = 100;
    [SerializeField] int deathEnergyBonus = 3;
    [SerializeField] protected int healthFactor = 2;
    LootSpawner lootSpawner;

    protected virtual void Awake()
    {
        lootSpawner = GetComponent<LootSpawner>();
    }

    protected override void OnEnable()
    {
        SetHealth();
        base.OnEnable();
    }


    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.Die();
            Die();
        }
    }

    public override void Die()
    {
        PlayerEnergy.Instance.Obtain(deathEnergyBonus);
        ScoreManager.Instance.AddScore(scorePoint);
        EnemyManager.Instance.RemoveFromList(gameObject);
        lootSpawner.Spawn(transform.position);
        base.Die();
    }

    protected virtual void SetHealth()
    {
        maxHealth += EnemyManager.Instance.WaveNumber/ healthFactor;       
    }
}
