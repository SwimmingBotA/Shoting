using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : PersistentSingleton<ScoreManager>
{
    public int Score => score;
    int score;
    int currentScore;
    [SerializeField] Vector3 scoreTextScale = new Vector3(1.2f, 1.2f, 1f);

    public void ResetScore()
    {
        score = 0;
        currentScore = 0;
        ScoreDisplay.UpadateText(score);
    }

    public void AddScore(int scorePoint)
    {
        currentScore += scorePoint;
        StartCoroutine(nameof(AddScoreCoroutine));
    }

    IEnumerator AddScoreCoroutine()
    {
        ScoreDisplay.ScaleText(scoreTextScale);
        while (score < currentScore)
        {
            score += 1;
            ScoreDisplay.UpadateText(score);
            yield return null;
        }
        ScoreDisplay.ScaleText(Vector3.one);
    }


    [System.Serializable]public class PlayerScore
    {
        public int score;
        public string playerNamer;

        public PlayerScore(int score,string playerNamer)
        {
            this.score = score;
            this.playerNamer = playerNamer;
        }
    }

    [System.Serializable]
    public class PlayerScoreData
    {
        public List<PlayerScore> list = new List<PlayerScore>();
    }

    readonly string saveFileName = "player_score.json";
    int defaultScore = 0;
    string defaultPlayerNamer = "No Name";

    public bool HasNewHighScore => LoadPlayerScoreData().list[9].score < score;
    public void SetPlayerName(string newName)
    {
        defaultPlayerNamer = newName;       
    }

    public void SavePlayerScoreData()
    {
        var playerScoreData = LoadPlayerScoreData();
        playerScoreData.list.Add(new PlayerScore(score, defaultPlayerNamer));
        playerScoreData.list.Sort((x,y)=>y.score.CompareTo(x.score));

        SaveSystem.SaveJsonData(saveFileName, playerScoreData);
    }
    
    public PlayerScoreData LoadPlayerScoreData()
    {
        var playerScoreData = new PlayerScoreData();


        if (SaveSystem.SaveFileExit(saveFileName))
        {
            playerScoreData = SaveSystem.LoadJsonData<PlayerScoreData>(saveFileName);
        }
        else  //无数据时候
        {
            while (playerScoreData.list.Count < 10)
            { 
            playerScoreData.list.Add(new PlayerScore(defaultScore, defaultPlayerNamer));      
            }
            SaveSystem.SaveJsonData(saveFileName, playerScoreData);
        }
        return playerScoreData;
    }
}
