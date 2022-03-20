using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : PersistentSingleton<SceneLoader>
{
    [SerializeField] Image transitionImage;
    [SerializeField] float fadeTime = 3.5f;

    Color color;


    const string MAINMENU = "Level0";
    const string GAMEPLAY = "Level1";
    const string SCORE = "level2";
    //void Load(string sceneName)
    //{
    //    SceneManager.LoadScene(sceneName);
    //}

    IEnumerator LoadCoroutine(string sceneName)
    {
        //“Ï≤Ωº”‘ÿ
        var loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        loadingOperation.allowSceneActivation = false;

        transitionImage.gameObject.SetActive(true);

        while (color.a < 1f)
        {
            color.a=Mathf.Clamp01(color.a + Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;

            yield return null;
        }

        yield return new WaitUntil(() => loadingOperation.progress >= 0.9f);

        loadingOperation.allowSceneActivation = true;

        while (color.a > 0f)
        {
            color.a = Mathf.Clamp01(color.a - Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;

            yield return null;
        }

        transitionImage.gameObject.SetActive(false);

    }

    public void LoadGameplayScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadCoroutine(GAMEPLAY));
    }

    public void LoadMainMenuScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadCoroutine(MAINMENU));
    }

    public void LoadScore()
    {
        StopAllCoroutines();
        StartCoroutine(LoadCoroutine(SCORE));
    }
}
