using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSystem : MonoBehaviour
{
    [SerializeField] GameObject missile =null;
    [SerializeField] AudioData launchSFX=null;

    [SerializeField] int defalutAmount = 5;
    [SerializeField] float cooldownTime = 3f;

    bool isReady = true;
    int currentAmount;
    WaitForSeconds waitForCoolDown;

    private void Awake()
    {
        currentAmount = defalutAmount;
        waitForCoolDown = new WaitForSeconds(cooldownTime);
    }

    private void Start()
    {
        MissileDisplay.UpadateText(defalutAmount);
    }


    public void Launch(Transform muzzleTransform)
    {
        if (currentAmount == 0||!isReady) return;

        isReady = false;
        PoolManager.Release(missile, muzzleTransform.position);
        AudioManager.Instance.PlayRandomSFX(launchSFX);
        currentAmount--;
        MissileDisplay.UpadateText(currentAmount);
        if (currentAmount == 0)
        {
            MissileDisplay.UpdateCoolDownImage(1.0f);
        }
        else
        {
            StartCoroutine(nameof(CoolDownCoroutine));
        }
    }

    IEnumerator CoolDownCoroutine()
    {
        var cooldown = cooldownTime;
        while (cooldown>0f)
        {
            MissileDisplay.UpdateCoolDownImage(cooldown / cooldownTime);
            cooldown = Mathf.Max(0f, cooldown - Time.deltaTime);
            yield return null;
        }
        isReady = true;
    }

    public void PickUp()
    {
        currentAmount++;
        MissileDisplay.UpadateText(currentAmount);

        if (currentAmount == 1)
        {
            MissileDisplay.UpdateCoolDownImage(0f);
            isReady = true;
        }

    }
}
