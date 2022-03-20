using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainMenuUIControl : MonoBehaviour
{
    [SerializeField] Canvas mainMenu;
    [SerializeField] Canvas audioMenu;
    [SerializeField] PlayerInput input;

    [SerializeField] Button startButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button backButton;

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider slider;
    private void OnEnable()
    {
        ButtonPressedBehavior.buttonFunctionTable.Add(startButton.gameObject.name, OnStartGameButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(optionsButton.gameObject.name, OnOptionsButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(quitButton.gameObject.name, OnQuitButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(backButton.gameObject.name, OnBackButtonClick);

        input.EnablePauseMenuInput();
    }

    private void OnDisable()
    {
        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }

    private void Start()
    {
        GameManager.GameState = GameState.Playing;
        UIInput.Instance.SelectUI(startButton);
        Time.timeScale = 1.0f;
    }

    void OnStartGameButtonClick()
    {
        mainMenu.enabled = false;
        SceneLoader.Instance.LoadGameplayScene();
    }

    void OnOptionsButtonClick()
    {
        mainMenu.enabled = false;
        audioMenu.enabled = true;
        UIInput.Instance.SelectUI(backButton);
    }

    void OnBackButtonClick()
    {
        mainMenu.enabled = true;
        audioMenu.enabled = false;
        UIInput.Instance.SelectUI(startButton);
    }


    void OnQuitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }

    public void SetVolume()
    {
        audioMixer.SetFloat("GlobalVoice", slider.value);
    }

}
