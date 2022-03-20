using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class GamePlayUIControl : MonoBehaviour
{
    [SerializeField] AudioData pauseSFX;
    [SerializeField] AudioData unPauseSFX;

    [SerializeField] PlayerInput input;
    [SerializeField] Canvas hUDCanvas;
    [SerializeField] Canvas menuCanvas;
    [SerializeField] Canvas audioMenu;
    [Header("----BUTTON----")]
    [SerializeField] Button resumeButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button backMenuButton;


    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider slider;

    int buttonPressedParameterID = Animator.StringToHash("Pressed");

    private void OnEnable()
    {
        input.onPause += Pause;
        input.onUnPause += UnPause;


        ButtonPressedBehavior.buttonFunctionTable.Add(resumeButton.gameObject.name, OnResumeButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(optionsButton.gameObject.name, OnOptionsButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(mainMenuButton.gameObject.name, OnMainMenuButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(backMenuButton.gameObject.name, OnBackClick);
    }

    private void OnDisable()
    {
        input.onPause -= Pause;
        input.onUnPause -= UnPause;

        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }

    void Pause()
    {
        TimeController.Instance.Pause();
        GameManager.GameState = GameState.Pausing;
        input.EnablePauseMenuInput();
        input.SwitchToDynamicUpdateMode();
        hUDCanvas.enabled = false;
        menuCanvas.enabled = true;
        UIInput.Instance.SelectUI(resumeButton);
        AudioManager.Instance.PlaySFX(pauseSFX);
    }

    void UnPause()
    {
        resumeButton.Select();
        resumeButton.animator.SetTrigger(buttonPressedParameterID);
        AudioManager.Instance.PlaySFX(unPauseSFX);
    } 

    void OnResumeButtonClick()
    {
        TimeController.Instance.UnPause();
        GameManager.GameState = GameState.Playing;
        input.EnableGameplayerInput();
        input.SwitchToFixedUpdateMode();
        hUDCanvas.enabled = true;
        menuCanvas.enabled = false;
    }

    void OnOptionsButtonClick()
    {
        menuCanvas.enabled = false;
        audioMenu.enabled = true;
        UIInput.Instance.SelectUI(backMenuButton);
    }

    void OnMainMenuButtonClick()
    {
        menuCanvas.enabled = false;
        SceneLoader.Instance.LoadMainMenuScene();
    }

    void OnBackClick()
    {
        audioMenu.enabled = false;
        menuCanvas.enabled = true;
        input.EnablePauseMenuInput();
        input.SwitchToDynamicUpdateMode();
        UIInput.Instance.SelectUI(resumeButton);
    }

    public void SetVolume()
    {
        audioMixer.SetFloat("GlobalVoice", slider.value);
    }
}
