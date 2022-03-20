using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{

    public GameObject RandomEnemy => enemyList.Count == 0 ? null : enemyList[Random.Range(0, enemyList.Count)];
    public int WaveNumber => waveNumber;
    public float TimeBetweenWaves => timeBetweenWaves;


    [SerializeField] bool spawnEnemy = true;
    [SerializeField] GameObject waveUI;

    [SerializeField] GameObject[] enemyPrefabs;

    [SerializeField] float timeBetweenSpawns = 1f;
    [SerializeField] float timeBetweenWaves = 1f;

    [SerializeField] int minEnemyAmount = 4;
    [SerializeField] int maxEnemyAmount = 10;

    int waveNumber = 1;
    int enemyAmount;
    List<GameObject> enemyList;

    WaitForSeconds waitTimeBetweenSpawns;
    WaitForSeconds waitTimeBetweenWaves;
    WaitUntil waitUntilNoEnemy;
    [Header("----BOSS---")]
    [SerializeField] GameObject bossPrefab;
    [SerializeField] int boosWaveNumber = 2;

    protected override void Awake()
    {
        base.Awake();
        waitTimeBetweenSpawns = new WaitForSeconds(timeBetweenSpawns);
        waitTimeBetweenWaves = new WaitForSeconds(timeBetweenWaves);
        enemyList = new List<GameObject>();
        waitUntilNoEnemy = new WaitUntil(() => enemyList.Count == 0);
    }


    IEnumerator Start()
    {
        while (spawnEnemy&&GameManager.GameState!=GameState.GameOver)
        {
            waveUI.SetActive(true);
            yield return waitTimeBetweenWaves;
            waveUI.SetActive(false);
            yield return StartCoroutine(nameof(RandomlySpawnCoroutine));
        }
    }

    IEnumerator RandomlySpawnCoroutine()
    {
        if (waveNumber % boosWaveNumber == 0)
        {
            enemyList.Add(PoolManager.Release(bossPrefab));
        }
        else
        {
            enemyAmount = Mathf.Clamp(enemyAmount, minEnemyAmount + waveNumber / boosWaveNumber, maxEnemyAmount);

            for (int i = 0; i < enemyAmount; i++)
            {
                enemyList.Add(
                    PoolManager.Release(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]));   //Random.RangeÎª×ó±ÕÓÒ¿ª
                yield return waitTimeBetweenSpawns;
            }
        }
        yield return waitUntilNoEnemy;

        waveNumber++;
    }

    public void RemoveFromList(GameObject enemy) => enemyList.Remove(enemy);

}
