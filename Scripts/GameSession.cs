using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour {

    int score = 0;
    int playerHealth;
    [SerializeField] int killsToPowerup = 50;
    [SerializeField] List<GameObject> powerUpPrefabs;
    [SerializeField] List<GameObject> post20PowerUpPrefabs;
    [SerializeField] float powerUpFallSpeed = 3f;
    [SerializeField] AnimationCurve powerUpRandomCurve;
    int killsToPowerupCounter;

    void Start()
    {
        playerHealth = FindObjectOfType<Player>().GetHealth();
        killsToPowerupCounter = killsToPowerup;
    }

    private void Awake()
    {
        SetUpSingleton();
    }

    private void SetUpSingleton()
    {
        int numberGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numberGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public int GetScore()
    {
        return score;
    }

    public void AddToScore(int scoreValue)
    {
        score += scoreValue;
    }

    public void ResetGame()
    {
        Destroy(gameObject);
    }

    public void UpdateHealth(int healthValue)
    {
        playerHealth += healthValue;
    }

    float CurveWeightedRandom(AnimationCurve curve)
    {
        return curve.Evaluate(Random.value);
    }

    public void SpawnPowerUp(Vector3 spawnPosition)
    {
        float roll = CurveWeightedRandom(powerUpRandomCurve); //value from curve rounded to nearest int
        int powerUpIndex = (int)(roll * powerUpPrefabs.Count); //length of list * roll, rounded down to get index
        GameObject powerUp = Instantiate(powerUpPrefabs[powerUpIndex], spawnPosition, Quaternion.identity) as GameObject;
        powerUp.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -powerUpFallSpeed);
        killsToPowerupCounter = killsToPowerup;
    }

    public int GetKillsToPowerup()
    {
        return killsToPowerupCounter;
    }

    public void DecreaseKillsToPowerup()
    {
        killsToPowerupCounter = killsToPowerupCounter - 1;
    }

    public void AddPowerUpPrefabs()
    {
        powerUpPrefabs.AddRange(post20PowerUpPrefabs);
    }

}
