using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] PlayerInput input;

    [SerializeField] Canvas playerCanvas;
    [SerializeField] AudioData exit;

    Canvas gameoverCanvas;
    Animator animator;
    int exitAnimation = Animator.StringToHash("GameOverScreenExit");

    private void Awake()
    {
        gameoverCanvas = GetComponent<Canvas>();
        animator = GetComponent<Animator>();
        gameoverCanvas.enabled = false;
        animator.enabled = false;
    }

    private void OnEnable()
    {
        input.onGameOver += OnConfirmGameOver;
        GameManager.onGameOver += OnGameOver;
    }

    private void OnDisable()
    {
        input.onGameOver -= OnConfirmGameOver;
        GameManager.onGameOver -= OnGameOver;
    }

    void OnGameOver()
    {
        playerCanvas.enabled = false;
        gameoverCanvas.enabled = true;
        animator.enabled = true;
        input.DisableAllInputs();
    }

    public void ConfirmGameOver()
    {
        input.EnableGameOverScreenInput();
    }

    void OnConfirmGameOver()
    {
        AudioManager.Instance.PlaySFX(exit);
        animator.Play(exitAnimation);
        input.DisableAllInputs();
        SceneLoader.Instance.LoadScore();  
    }
}
