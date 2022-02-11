using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayWaves : MonoBehaviour
{
    Text waveText;
    EnemySpawner enemySpawner;

    // Use this for initialization
    void Start()
    {
        waveText = GetComponent<Text>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        waveText.text = "Wave " + enemySpawner.GetWaveIndex().ToString();
        if (enemySpawner.GetWaveIndex() < 0)
        {
            waveText.text = "Wave 0";
        }
    }
}
