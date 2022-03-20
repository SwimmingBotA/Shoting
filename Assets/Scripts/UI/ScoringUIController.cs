using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoringUIController : MonoBehaviour
{
    [Header("----SCORING----")]
    [SerializeField] Button mainMenuButton;
    [SerializeField] Canvas scoreCanvas;
    [SerializeField] Text scoreText;


    [SerializeField] Transform hightScoreLeaderBoardContainer;
    [SerializeField] Canvas submitHighScore;
    [SerializeField] Button submitButton;
    [SerializeField] Button cancleButton;
    [SerializeField] InputField playerNameInputField;

    [Header("-----BACKGROUND---")]
    [SerializeField] Sprite[] backGroundImages;
    [SerializeField] Image backGroundImage;

    

    private void OnDisable()
    {
        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        ShowRandomBackground();
        if (ScoreManager.Instance.HasNewHighScore)
        {
            ShowNewHighScoreScreen();
        }
        else
        {
            ShowScoringScreen();
        }
        ButtonPressedBehavior.buttonFunctionTable.Add(mainMenuButton.gameObject.name, OnButtonMainMenuClicked);
        ButtonPressedBehavior.buttonFunctionTable.Add(submitButton.gameObject.name, OnButtonSubmit);
        ButtonPressedBehavior.buttonFunctionTable.Add(cancleButton.gameObject.name, OnButtonCancle);
        GameManager.GameState = GameState.Scoring;
    }


    void ShowRandomBackground()
    {
        backGroundImage.sprite = backGroundImages[Random.Range(0, backGroundImages.Length)];
    }

    void ShowScoringScreen()
    {
        scoreCanvas.enabled = true;
        scoreText.text = ScoreManager.Instance.Score.ToString();
        UIInput.Instance.SelectUI(mainMenuButton);
        UpdateHighScoreLeadrBoard();
    }

    void ShowNewHighScoreScreen()
    {
        submitHighScore.enabled = true;
        UIInput.Instance.SelectUI(cancleButton);
    }

    void UpdateHighScoreLeadrBoard()
    {
        var loadPlayerData = ScoreManager.Instance.LoadPlayerScoreData().list;

        for (int i = 0; i < hightScoreLeaderBoardContainer.childCount; i++)
        {
            var child = hightScoreLeaderBoardContainer.GetChild(i);
            child.Find("Rank").GetComponent<Text>().text=(1+i).ToString();
            child.Find("Score").GetComponent<Text>().text=loadPlayerData[i].score.ToString();
            child.Find("Name").GetComponent<Text>().text = loadPlayerData[i].playerNamer;
        }
    }

    void OnButtonMainMenuClicked()
    {
        scoreCanvas.enabled = false;
        SceneLoader.Instance.LoadMainMenuScene();
    }

    private void OnButtonSubmit()
    {
        if (!string.IsNullOrEmpty(playerNameInputField.text))
        {
            ScoreManager.Instance.SetPlayerName(playerNameInputField.text);
        }

        OnButtonCancle();
    }

    void OnButtonCancle()
    {
        submitHighScore.enabled = false;
        ScoreManager.Instance.SavePlayerScoreData();
        ShowRandomBackground();
        ShowScoringScreen();
    }
}
