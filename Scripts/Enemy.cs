using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] float health = 1;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] int scoreValue = 10;
    [SerializeField] [Range(0, 100)] int chanceToSpawnPowerUp = 100;
    [SerializeField] int minExplosionDamage = 5;
    bool powerUpSpawned = false;

    [Header("Projectiles")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] bool standardShooting = true;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] float projectileSpeed = 2f;
    [SerializeField] int numberOfProjectileSpawns = 1;
    [SerializeField]  float timeBetweenShots = 0.4f;
    [SerializeField] [Range(0, 1)] float chanceToShootOnCondition = 0.5f;
    float fireCooldown;
    
    [Header("Audio")]
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip damagedSFX;
    [SerializeField] AudioClip shootSFX;
    [SerializeField] [Range(0, 1)] float deathSoundVolume = 0.7f;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.5f;


    [Header("Video")]
    [SerializeField] GameObject deathVFX;
    [SerializeField] float durationOfExplosion = 1f;

    [Header("Colors")]
    [SerializeField] ColorConfig acidColor;

    // Use this for initialization
    void Start ()
    {
        GetComponent<EnemyPathing>().SetMoveSpeed(moveSpeed);
        fireCooldown = Random.Range(minTimeBetweenShots, maxTimeBetweenShots)/2; //first shot is spawned faster
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (standardShooting)
        {
            CountdownAndShoot();
        }
    }

    private void CountdownAndShoot()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            StartCoroutine(SpawnAllProjectiles());
            fireCooldown = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    public void ShootNow()
    {
        float roll = Random.Range(0f, 1f);
        if (roll < chanceToShootOnCondition)
        {
            StartCoroutine(SpawnAllProjectiles());
        }
    }

    public IEnumerator SpawnAllProjectiles()
    {
        for (int numberOfShotsFired = 0; numberOfShotsFired < numberOfProjectileSpawns; numberOfShotsFired++) //nie dziala na razie we napraw
        {
            Fire(); 
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    private void Fire()
    {
        GameObject laser = Instantiate(projectilePrefab, transform.position, Quaternion.identity) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -projectileSpeed);
        AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, shootSoundVolume);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer damageDealer = collision.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
        var acidCollision = collision.transform.parent.GetComponent<PlayerProjectile>().GetIsAcid();
        if (acidCollision)
        {
            StartCoroutine(Poisoned(collision));
        }
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        var damageMultiplier = FindObjectOfType<Player>().GetProjectileDamage();
        var damage = damageDealer.GetDamage() * damageMultiplier;
        if (damage >= minExplosionDamage)
        {
            AudioSource.PlayClipAtPoint(damagedSFX, Camera.main.transform.position, deathSoundVolume);
        }
        health -= damageDealer.GetDamage() * damageMultiplier;
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    IEnumerator Poisoned(Collider2D collision)
    {
        var player = collision.transform.parent.GetComponent<PlayerProjectile>().GetPlayer();
        var percentageSlow = player.GetAcidSlow();
        var acidSlowTime = player.GetAcidSlowTime();
        var newMoveSpeed = moveSpeed * (100 - percentageSlow) / 100;
        var color = GetComponent<SpriteRenderer>().color;
        GetComponent<EnemyPathing>().SetMoveSpeed(newMoveSpeed);
        ChangeColor(color, acidColor.GetRed(), acidColor.GetGreen(), acidColor.GetBlue());
        yield return new WaitForSeconds(acidSlowTime);
        GetComponent<EnemyPathing>().SetMoveSpeed(moveSpeed);
        ChangeColor(color, 1, 1, 1); //1,1,1 to biały
    }

    private void ChangeColor(Color color, float red, float green, float blue)
    {
        color.r = red;
        color.g = green;
        color.b = blue;
        GetComponent<SpriteRenderer>().color = color;
    }

    private void Die()
    {
        SpawnPowerUp();
        FindObjectOfType<GameSession>().AddToScore(scoreValue);
        Destroy(gameObject);
        GameObject deathEffect = Instantiate(deathVFX, transform.position, Quaternion.identity);
        Destroy(deathEffect, durationOfExplosion);
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathSoundVolume);
    }

    private void SpawnPowerUp()
    {
        var gameSession = FindObjectOfType<GameSession>();
        var killsToPowerup = gameSession.GetKillsToPowerup();
        if (!powerUpSpawned)
        {
            powerUpSpawned = true;
            if (killsToPowerup <= 0) //guaranteed powerup per x kills without a powerup
            {
                gameSession.SpawnPowerUp(transform.position);
            }
            else
            {
                var randomRoll = Random.Range(0, 100);
                if (randomRoll < chanceToSpawnPowerUp)
                {
                    gameSession.SpawnPowerUp(transform.position);
                }
                else
                {
                    gameSession.DecreaseKillsToPowerup();
                }
            } 
        }
    }

    public float GetMoveSpeed() { return moveSpeed; }
}