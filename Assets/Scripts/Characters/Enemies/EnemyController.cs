using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("---MOVE---")]
    protected float paddingX;
    protected float paddingY;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float moveRotationAngle = 25;

    [Header("----FIRE---")]
    [SerializeField] protected float minFireInterval;
    [SerializeField] protected float maxFireInterval;
    [SerializeField] protected AudioData[] projectileLaunchSFX;

    [SerializeField] protected GameObject[] projectiles;
    [SerializeField] protected Transform muzzle;
    [SerializeField] protected ParticleSystem muzzleVFX;

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    protected Vector3 targetPosition;

    protected virtual void Awake()
    {
        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2.0f;
        paddingY = size.y / 2.0f;
    }


    protected virtual void OnEnable()
    {
        StartCoroutine(nameof(RandomlyMovingCoroutine));
        StartCoroutine(nameof(RandomlyFireCoroutine));
    }


    private void OnDisable()
    {
        StopAllCoroutines();
    }


    IEnumerator RandomlyMovingCoroutine()
    {
        transform.position = ViewPort.Instance.RandomEnemySpawnPosition(paddingX, paddingY);
        targetPosition = ViewPort.Instance.RandomRightHalfPosition(paddingX, paddingY);

        while (gameObject.activeSelf)
        {
            if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
                transform.rotation = Quaternion.AngleAxis((targetPosition-transform.position).normalized.y*moveRotationAngle, Vector3.right);
            }
            else
            {
                targetPosition = ViewPort.Instance.RandomRightHalfPosition(paddingX, paddingY);
            }
            yield return waitForFixedUpdate;
        }
    }

    protected virtual IEnumerator RandomlyFireCoroutine()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(Random.Range(minFireInterval, maxFireInterval));

            if (GameManager.GameState == GameState.GameOver) yield break;

            foreach (var projectile in projectiles)
            {
                PoolManager.Release(projectile, muzzle.position);
            }
            muzzleVFX.Play();
            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);
        }
    }
}
