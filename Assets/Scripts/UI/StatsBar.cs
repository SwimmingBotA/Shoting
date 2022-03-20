using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar : MonoBehaviour
{
    [SerializeField] Image fillImageFront;
    [SerializeField] Image fillImageBack;
    [SerializeField] bool delayFill=true;
    [SerializeField] float fillSpeed = 0.1f;
    [SerializeField] float fillDelay = 0.5f;

    float currentFillAmount;
    protected float targetFillAmount;
    float t;
    WaitForSeconds waitForDelayFill;
    Coroutine bufferedFillingCoroutine;
    float previousFillAmount;

    private void Awake()
    {
        if (TryGetComponent<Canvas>(out Canvas canvas))
        {
            canvas.worldCamera = Camera.main;
        }

        waitForDelayFill = new WaitForSeconds(fillDelay);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public virtual void Initialized(float currentValue,float maxValue)
    {
        currentFillAmount = currentValue / maxValue;
        targetFillAmount = currentFillAmount;
        fillImageFront.fillAmount = currentFillAmount;
        fillImageBack.fillAmount = currentFillAmount;
    }

    public void UpdateStats(float currentValue, float maxValue)
    {
        targetFillAmount = currentValue / maxValue;

        if (bufferedFillingCoroutine != null)
        {
            StopCoroutine(bufferedFillingCoroutine);
        }


        // ‹…À ±∫Ú
        if (targetFillAmount < currentFillAmount)
        {
            fillImageFront.fillAmount = targetFillAmount;
            bufferedFillingCoroutine = StartCoroutine(BufferedFillCoroutine(fillImageBack));

            return;
        }

        //ª÷∏¥ ±∫Ú
        if (targetFillAmount > currentFillAmount)
        {
            fillImageBack.fillAmount = targetFillAmount;
            bufferedFillingCoroutine = StartCoroutine(BufferedFillCoroutine(fillImageFront));
        }
    }


    protected virtual IEnumerator BufferedFillCoroutine(Image image)
    {

        if (delayFill)
        {
            yield return waitForDelayFill;
        }


        t = 0f;
        previousFillAmount = currentFillAmount;
        while (t < 1f)
        {
            t += Time.deltaTime * fillSpeed;
            currentFillAmount = Mathf.Lerp(previousFillAmount, targetFillAmount, t);
            image.fillAmount = currentFillAmount;
            yield return null;
        }
    }
}
