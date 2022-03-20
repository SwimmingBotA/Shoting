using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : EnemyController
{
    [SerializeField] float continuousFireDuaration = 1.5f;

    [Header("----Player Detection----")]
    [SerializeField] Transform playerDetectionTransform;
    [SerializeField] Vector3 playerDetectionSize;
    [SerializeField] LayerMask playerLayer;



    WaitForSeconds waitForContinuousFireInterval;
    WaitForSeconds waitForFireInterval;

    List<GameObject> magazine;
    AudioData launchSFX;

    [Header("---Beam---")]
    [SerializeField]float beamCooldownTime = 12f;
    [SerializeField] AudioData beamChargingSFX;
    [SerializeField] AudioData beamLaunchSFX;
    bool isBeamReady;
    WaitForSeconds waitForBeamCoolDownTime;
    Animator animator;
    int launchBeamID = Animator.StringToHash("launchBeam");

    Transform playerTransform;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        waitForContinuousFireInterval = new WaitForSeconds(minFireInterval);
        waitForFireInterval = new WaitForSeconds(maxFireInterval);
        waitForBeamCoolDownTime = new WaitForSeconds(beamCooldownTime);
        magazine = new List<GameObject>(projectiles.Length);

        playerTransform = FindObjectOfType<Player>().transform;
    }

    protected override void OnEnable()
    {
        isBeamReady = false;
        muzzleVFX.Stop();
        StartCoroutine(nameof(BeamCooldownCoroutine));
        base.OnEnable();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(playerDetectionTransform.position, playerDetectionSize);
    }

    void ActivateBeamWeapon()
    {
        isBeamReady = false;
        animator.SetTrigger(launchBeamID);
        AudioManager.Instance.PlayRandomSFX(beamChargingSFX);
    }

    void AnimationEventLaunchBeam()
    {
        AudioManager.Instance.PlayRandomSFX(launchSFX);
    }

    void AnimationEventStopBeam()
    {
        StopCoroutine(nameof(ChasingPlayerCoroutine));
        StartCoroutine(nameof(RandomlyFireCoroutine));
        StartCoroutine(nameof(BeamCooldownCoroutine));
    }

    void LoadProjectiles()
    {
        magazine.Clear();
        if (Physics2D.OverlapBox(playerDetectionTransform.position, playerDetectionSize,0f, playerLayer))
        {
            magazine.Add(projectiles[0]);
            launchSFX = projectileLaunchSFX[0];
        }
        else
        {
            if(Random.value < 0.5f)
            {
                magazine.Add(projectiles[1]);
                launchSFX = projectileLaunchSFX[1];
            }
            else
            {
                for (int i = 2; i < projectiles.Length; i++)
                {
                    magazine.Add(projectiles[i]);
                }
                launchSFX = projectileLaunchSFX[2];
            }
        }
    
    }

    protected override IEnumerator RandomlyFireCoroutine()
    {
        while (isActiveAndEnabled)
        {
            if (GameManager.GameState == GameState.GameOver) yield break;
            if (isBeamReady)
            {
                ActivateBeamWeapon();
                StartCoroutine(nameof(ChasingPlayerCoroutine));
                yield break;
            }
            yield return waitForFireInterval;
            yield return StartCoroutine(nameof(ContinouusFireCoroutine));
        }
    }

    IEnumerator ContinouusFireCoroutine()
    {

        LoadProjectiles();
        muzzleVFX.Play();
        float continousFireTimer = 0f;

        while (continousFireTimer < continuousFireDuaration)    //1.5
        {
            foreach (var projectile in magazine)
            {
                PoolManager.Release(projectile, muzzle.position);
            }
        continousFireTimer += minFireInterval;
        AudioManager.Instance.PlayRandomSFX(launchSFX);
        yield return waitForContinuousFireInterval;   //0.15
        }
        muzzleVFX.Stop();
    }

    IEnumerator BeamCooldownCoroutine()
    {
        yield return waitForBeamCoolDownTime;
        isBeamReady = true;
    }

    IEnumerator ChasingPlayerCoroutine()
    {
        while (isActiveAndEnabled)
        {
            targetPosition.x = ViewPort.Instance.MaxX - paddingX;
            targetPosition.y = playerTransform.position.y;
            yield return null;
        }
    }
}
