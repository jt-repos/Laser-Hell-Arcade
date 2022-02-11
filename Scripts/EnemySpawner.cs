using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<WaveConfig> waveConfigs;
    [SerializeField] int startingWave = 0;
    WaveConfig currentWave;
    float timeForTheNextWave = 0f;
    int waveIndexPointer;
    bool spawnNextWaveNow = false;

	// Use this for initialization
	void Start()
    {
        StartCoroutine(SpawnAllWaves());
    }

    private void Update()
    {
        timeForTheNextWave -= Time.deltaTime;
        if (timeForTheNextWave <= 0f)
        {
            spawnNextWaveNow = true;
        }
    }

    private IEnumerator SpawnAllWaves()
    {
        for (int waveIndex =  startingWave; waveIndex < waveConfigs.Count; waveIndex++)
        {
            spawnNextWaveNow = false;
            while (!spawnNextWaveNow) //zatrzymuje funkcje az otrzyma komende zeby spawnic nastepny wave
            {
                yield return null;
            }
            currentWave = waveConfigs[waveIndex];
            waveIndexPointer = waveIndex; //tylko po to zeby spawn enemies mogl se spojrzec
            StartCoroutine(SpawnAllEnemiesInWave(currentWave));
            timeForTheNextWave = currentWave.GetTimeToSpawnTheNextWave(); //dodaje cooldown do nastepnego wave'a
            if (waveIndexPointer == 20) //dodaje nowe powerupy zeby bo by sie znudzily jakby byly dostepne przedtem
            {
                FindObjectOfType<GameSession>().AddPowerUpPrefabs();
            }
        }
        yield return new WaitForSeconds(currentWave.GetTimeToSpawnTheNextWave());
        FindObjectOfType<Level>().LoadGameFinished();
    }

    private IEnumerator SpawnAllEnemiesInWave(WaveConfig waveConfig)
    {
        var currentWaveIndex = waveIndexPointer; //do sprawdzenia czy nastepny wave nie zostal juz scallowany
        for (int enemyCount = 0; enemyCount < waveConfig.GetNumberOfEnemies(); enemyCount++)
        {
            var newEnemy = Instantiate(
            waveConfig.GetEnemyPrefab(),
            waveConfig.GetWaypoints()[0].transform.position,
            Quaternion.identity);
            newEnemy.GetComponent<EnemyPathing>().SetWaveConfig(waveConfig);
            yield return new WaitForSeconds(waveConfig.GetTimeBetweenSpawns());
        }
        if (currentWaveIndex == waveIndexPointer) //zdarza sie ze w trakcie for loopa, wave index pointer jest wiekszy niz current wave index, calluj nastepny wave tylko jesli oba sa nadal rowne
        {
            spawnNextWaveNow = true; //jesli kolejny wave od razu po tym zostal juz scallowany, to nie calluj jeszcze nastepnego
        }
    }

    public int GetWaveIndex()
    {
        return waveIndexPointer + 1;
    }

    public WaveConfig GetCurrentWave()
    {
        return currentWave;
    }
}
