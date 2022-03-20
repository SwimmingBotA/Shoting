using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : PersistentSingleton<TimeController>
{
    [SerializeField,Range(0f,1f)] float bulletTimeScale = 0.1f;
    float defaultFixedDeltaTime;
    float t;
    bool isPause=false;
    float timeScaleBeforePause;


    protected override void Awake()
    {
        base.Awake();
        defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void BulletTime(float duration)
    {
        Time.timeScale = bulletTimeScale;
        StartCoroutine(SlowOutCoroutine(duration));
    }

    public void Pause()
    {
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0f;
    }

    public void UnPause()
    {
        Time.timeScale = timeScaleBeforePause;
    }



    public void BulletTime(float inDuration, float outDuration)
    {
        StartCoroutine(SlowInAndOutCoroutine(inDuration, outDuration));
    }

    public void BulletTime(float inDuration, float keepingDuration, float outDuration)
    {
        StartCoroutine(SlowInKeepAndOutDurationCoroutine(inDuration, keepingDuration, outDuration));
    }

    IEnumerator SlowInKeepAndOutDurationCoroutine(float inDuration, float keepingDuration, float outDuration)
    {
        yield return StartCoroutine(SlowInCoroutine(inDuration));

        yield return new WaitForSecondsRealtime(keepingDuration);

        StartCoroutine(SlowOutCoroutine(outDuration));
    }

    IEnumerator SlowInAndOutCoroutine(float inDuration,float outDuration)
    {
        yield return StartCoroutine(SlowInCoroutine(inDuration));

        StartCoroutine(SlowOutCoroutine(outDuration));
    }

    IEnumerator SlowInCoroutine(float duration)
    {
        t = 0f;
        while (t < 1f)
        {
            if (GameManager.GameState!=GameState.Pausing)
            {
                t += Time.unscaledDeltaTime / duration;
                Time.timeScale = Mathf.Lerp(1f, bulletTimeScale, t);
                Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
            }
            yield return null;
        }
    }

    IEnumerator SlowOutCoroutine(float duration)
    {
        t = 0f;
        while (t < 1f)
        {
            if (GameManager.GameState != GameState.Pausing)
            {
                t += Time.unscaledDeltaTime / duration;
                Time.timeScale = Mathf.Lerp(bulletTimeScale, 1f, t);
                Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
            }
            yield return null;
        }
    }
}
