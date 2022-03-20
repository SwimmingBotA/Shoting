using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("----HEALTH----")]
    [SerializeField] protected float maxHealth;
    [SerializeField] StatsBar onHeadHealthBar;
    [SerializeField] bool showOnHeadHealthBar = true;

    [SerializeField] GameObject deathVFX;
    [SerializeField] AudioData[] deathSFX;
    protected float health;

    protected virtual void OnEnable()
    {
        health = maxHealth;
        if (showOnHeadHealthBar)
        {
            ShowOnHeadHealthBar();
        }
        else
        {
            HideOnHeadHealthBar();
        }
    }

    public void ShowOnHeadHealthBar()
    {
        onHeadHealthBar.gameObject.SetActive(true);
        onHeadHealthBar.Initialized(health, maxHealth);
    }

    public void HideOnHeadHealthBar()
    {
        onHeadHealthBar.gameObject.SetActive(false);
    }

    public virtual void TakeDamage(float damage)
    {
        if (health == 0f) return;
        health -= damage;

        if (showOnHeadHealthBar)
        {
            onHeadHealthBar.UpdateStats(health, maxHealth);
        }

        if (health <= 0f)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        health = 0f;
        PoolManager.Release(deathVFX, transform.position);
        AudioManager.Instance.PlayRandomSFX(deathSFX);
        gameObject.SetActive(false);
    }

    //角色回复生命
    public virtual void RestoreHealth(float value)
    {
        if (health == maxHealth) return;
        health = Mathf.Clamp(health + value, 0f, maxHealth);
        if (showOnHeadHealthBar)
        {
            onHeadHealthBar.UpdateStats(health, maxHealth);
        }
    }

    //角色持续回复血量
    protected IEnumerator HealthRegenerateCoroutine(WaitForSeconds waitTime,float percent)
    {
        while (health < maxHealth)
        {
            yield return waitTime;
            RestoreHealth(maxHealth * percent);
        }
    }

    //持续受伤
    protected IEnumerator DamageOverTimeCoroutine(WaitForSeconds waitTime, float percent)
    {
        while (health >0f)
        {
            yield return waitTime;
            TakeDamage(maxHealth * percent);
        }
    }
}
