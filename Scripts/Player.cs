using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //config parameters
    [Header("Player")]
    [SerializeField] Sprite rocketShipSprite;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = 1f;
    [SerializeField] int health = 5;
    [SerializeField] bool isDummy = false;
    float xMin;
    float xMax;
    float yMin;
    float yMax;

    [Header("Projectile")]
    [SerializeField] List<GameObject> laserPrefabs;
    [SerializeField] GameObject rocketPrefab;
    [SerializeField] GameObject barrageProjectilePrefab;
    [SerializeField] int shotsToRocket = 5;
    [SerializeField] int projectileDamage = 1;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectilesPerSecond = 10;
    [SerializeField] int levelOfLaser = 0;
    Coroutine firingCoroutine;
    int shotCounter = 1;

    [Header("Audio")]
    [SerializeField] AudioClip processHitSFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip shootLaserSFX;
    [SerializeField] AudioClip shootRocketSFX;
    [SerializeField] [Range(0, 1)] float damageSoundVolume = 0.7f;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.7f;
    

    [Header("Power Up Effects")]
    [SerializeField] bool isAcid = false;
    [SerializeField] bool isRocketOn = false;
    [SerializeField] bool isBarrageOn = false;
    [SerializeField] bool isCustomShooting = false;
    [SerializeField] float acidPercentageSlow = 50f;
    [SerializeField] float acidSlowTime = 1f;
    [SerializeField] int projectileDamageIncrease = 1;
    [SerializeField] int maxProjectileDamage = 5;
    [SerializeField] float projectileSpeedIncrease = 5f;
    [SerializeField] float maxProjectileSpeed = 25f;
    [SerializeField] float projectilesPerSecondIncrease = 2f;
    [SerializeField] float maxProjectilesPerSecond = 20f;
    [SerializeField] int healthUpValue = 1;

    [Header("Colors")]
    [SerializeField] ColorConfig acidColor;

    [Header("Barrage")]
    [SerializeField] int shotsToBarrage = 20;
    [SerializeField] int barrageShotSpawns = 10;
    [SerializeField] float intervalBarrageShots = 0.1f;

    [Header("Custom Shot Config")]
    [SerializeField] bool defaultConfigIndex;
    [SerializeField] List<CustomShootingConfig> customShootingConfigs;
    int configIndex;
    int shotsToShootCustom;
    bool defaultAngleBetweenShots;
    float changeInWaveAngle;
    float angleBetweenShots;
    float initialAngle;
    int numberOfWaves;
    float timeBetweenShots;
    int numberOfProjectilesInWave;
    float currentRotation;

    // Use this for initialization
    void Start ()
    {
        if (!defaultConfigIndex && !isDummy) //jesli nie jest dumiesem i nie defaultowy to laduj normalny
        {
            configIndex = GameObject.Find("Level").GetComponent<Level>().GetAugmentIndex();
        }
        if (isDummy) //zeby sie nie mieszaly
        {
            configIndex = GameObject.Find("Level").GetComponent<Level>().GetDummyAugmentIndex();
            StartCoroutine(ShowCustom());
            StartCoroutine(FireContinuously());
        }
        SetUpMoveBoundaries();
	}

    IEnumerator ShowCustom()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(ShootCustom());
    }

    // Update is called once per frame
    void Update ()
    {
        if(!isDummy)
        {
            Move();
            Fire();
        }
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        { 
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }
    
    IEnumerator FireContinuously()
    {
        while (true)
        {
            if (isRocketOn && shotCounter % shotsToRocket == 0)
            {
                if (GetComponent<SpriteRenderer>().sprite != rocketShipSprite)
                {
                    GetComponent<SpriteRenderer>().sprite = rocketShipSprite;
                }
                SpawnRocket();
            }
            else
            {
                SpawnStandardProjectile();
            }
            if (isBarrageOn && shotCounter % shotsToBarrage == 0)
            {
                StartCoroutine(SpawnAllBarrageShots());
            }
            if (isCustomShooting)
            {
                shotsToShootCustom = customShootingConfigs[configIndex].GetShotsToShootCustom();
                if (shotCounter % shotsToShootCustom == 0)
                {
                    StartCoroutine(ShootCustom());
                }
            }
            shotCounter++;
            yield return new WaitForSeconds(1f / projectilesPerSecond); //interwał pomiedzy strzelaniem
        }
    }

    private void SpawnRocket()
    {
        GameObject currentRocketPrefab = rocketPrefab;
        Instantiate(currentRocketPrefab, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(shootRocketSFX, Camera.main.transform.position, shootSoundVolume);
    }

    private void SpawnStandardProjectile()
    {
        GameObject currentLaserPrefab = laserPrefabs[levelOfLaser];
        Instantiate(currentLaserPrefab, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(shootLaserSFX, Camera.main.transform.position, shootSoundVolume);
    }

    IEnumerator SpawnAllBarrageShots()
    {
        for (int i = 0; i <= barrageShotSpawns; i++) //spawnuje jeden dodatkowy zeby nie bylo przerwy na koncu ekranu
        {
            float startX = xMin + padding; //gets the vanilla view to world point values
            float endX = xMax - padding;
            float xPosForNewProjectile = startX + (2 * endX / barrageShotSpawns * (i));
            Vector2 spawnPosition = new Vector2(xPosForNewProjectile, yMax + padding);
            GameObject currentBarrageShotPrefab = barrageProjectilePrefab;
            Instantiate(currentBarrageShotPrefab, spawnPosition, Quaternion.identity);
            AudioSource.PlayClipAtPoint(shootLaserSFX, Camera.main.transform.position, shootSoundVolume);
            yield return new WaitForSeconds(intervalBarrageShots);
        }
    }

    IEnumerator ShootCustom()
    {
        SetUpCustomShot();
        for (int numberOfWavesFired = 0; numberOfWavesFired < numberOfWaves; numberOfWavesFired++)
        {
            SpawnCustomShotConfig(numberOfWavesFired); //passes amount of shots fire to adjust the change in rotation
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    private void SpawnCustomShotConfig(int numberOfWavesFired)
    {
        for (int numberOfProjectilesSpawned = 1; numberOfProjectilesSpawned < numberOfProjectilesInWave + 1; numberOfProjectilesSpawned++)
        {
            SetUpCurrentAngle(numberOfWavesFired, numberOfProjectilesSpawned);
            Instantiate(laserPrefabs[levelOfLaser], transform.position, Quaternion.Euler(0f, 0f, currentRotation));
        }
        AudioSource.PlayClipAtPoint(shootLaserSFX, Camera.main.transform.position, shootSoundVolume);
    }

    private void SetUpCustomShot()
    {
        defaultAngleBetweenShots = customShootingConfigs[configIndex].GetDefaultAngleBetweenShots();
        changeInWaveAngle = customShootingConfigs[configIndex].GetChangeInWaveAngle();
        angleBetweenShots = customShootingConfigs[configIndex].GetAngleBetweenShots();
        initialAngle = customShootingConfigs[configIndex].GetInitialAngle();
        numberOfWaves = customShootingConfigs[configIndex].GetNumberOfWaves();
        timeBetweenShots = customShootingConfigs[configIndex].GetTimeBetweenShots();
        numberOfProjectilesInWave = customShootingConfigs[configIndex].GetNumberOfProjectilesInWave();
    }

    private void SetUpCurrentAngle(int numberOfWavesFired, int numberOfProjectilesSpawned)
    {
        if (defaultAngleBetweenShots)
        {
            currentRotation = numberOfProjectilesSpawned * 360 / numberOfProjectilesInWave + 180 / numberOfProjectilesInWave + changeInWaveAngle * numberOfWavesFired + initialAngle; //każdy pocisk dostaje własny kąt, tak żeby były równomiernie rozłożone + połowa początkowego kąta, żeby nie było pocisku celującego centralnie na dół
        }
        else
        {
            currentRotation = numberOfProjectilesSpawned * angleBetweenShots + changeInWaveAngle * numberOfWavesFired + initialAngle;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        AudioSource.PlayClipAtPoint(processHitSFX, Camera.main.transform.position, damageSoundVolume);
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, damageSoundVolume);
        FindObjectOfType<Level>().LoadGameOver();
    }

    public int GetHealth()
    {
        return health;
    }

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newXPos, newYPos);
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x - padding; 
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + padding; 
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding * 0.5f;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding * 0.5f;
    }

    private void ChangeColor(float red, float green, float blue)
    {
        var color = GetComponent<SpriteRenderer>().color;
        color.r = red;
        color.g = green;
        color.b = blue;
        GetComponent<SpriteRenderer>().color = color;
    }

    public void TeleportToOppositeSide(float teleportDisplacementX)
    {
        var newXPos = transform.position.x + teleportDisplacementX;
        transform.position = new Vector2(newXPos, transform.position.y);
    }


    //power up methods
    public bool ProjectileNumberUp(int addToScoreValueIfMaxLevel)
    {
        var maxedOut = false;
        if (levelOfLaser < laserPrefabs.Count - 1)
        {
            levelOfLaser++;
        }
        else
        {
            FindObjectOfType<GameSession>().AddToScore(addToScoreValueIfMaxLevel);
            maxedOut = true;
        }
        return maxedOut;
    }

    

    public bool RocketUp(int addToScoreValueIfMaxLevel)
    {
        var maxedOut = false;
        if (!isRocketOn)
        {
            isRocketOn = true;
        }
        else
        {
            FindObjectOfType<GameSession>().AddToScore(addToScoreValueIfMaxLevel);
            maxedOut = true;
        }
        return maxedOut;
    }

    public bool AcidUp(int addToScoreValueIfMaxLevel)
    {
        var maxedOut = false;
        if (!isAcid)
        {
            isAcid = true;
            ChangeColor(acidColor.GetRed(), acidColor.GetGreen(), acidColor.GetBlue());
        }
        else
        {
            FindObjectOfType<GameSession>().AddToScore(addToScoreValueIfMaxLevel);
            maxedOut = true;
        }
        return maxedOut;
    }

    public bool BarrageUp(int addToScoreValueIfMaxLevel)
    {
        var maxedOut = false;
        if (!isBarrageOn)
        {
            isBarrageOn = true;
        }
        else
        {
            FindObjectOfType<GameSession>().AddToScore(addToScoreValueIfMaxLevel);
            maxedOut = true;
        }
        return maxedOut;
    }

    public void CustomShooting(int addToScoreValueIfMaxLevel)
    {
        if (!isCustomShooting)
        {
            isCustomShooting = true;
        }
        else
        {
            FindObjectOfType<GameSession>().AddToScore(addToScoreValueIfMaxLevel);
        }
    }
    public bool DamageUp(int addToScoreValueIfMaxLevel)
    {
        var maxedOut = false;
        if (projectileDamage < maxProjectileDamage)
        {
            projectileDamage += projectileDamageIncrease;
        }
        else
        {
            FindObjectOfType<GameSession>().AddToScore(addToScoreValueIfMaxLevel);
            maxedOut = true;
        }
        return maxedOut;
    }

    public bool AttackSpeedUp(int addToScoreValueIfMaxLevel)
    {
        var maxedOut = false;
        if (projectilesPerSecond < maxProjectilesPerSecond)
        {
            projectilesPerSecond += projectilesPerSecondIncrease;
        }
        else
        {
            FindObjectOfType<GameSession>().AddToScore(addToScoreValueIfMaxLevel);
            maxedOut = true;
        }
        return maxedOut;
    }

    public bool ProjetileSpeedUp(int addToScoreValueIfMaxLevel) //the other 2 functions work analogically
    {
        var maxedOut = false; //by default it is not at maximum level
        if (projectileSpeed < maxProjectileSpeed) //if statistic is not upgraded to maximum
        {
            projectileSpeed += projectileSpeedIncrease; //increase the variable by amount given
        }
        else
        {
            FindObjectOfType<GameSession>().AddToScore(addToScoreValueIfMaxLevel); //increase score
            maxedOut = true; //set the variable to show that the upgrade is at max level
        }
        return maxedOut; //return whether it is at max level or not
    }

    public void HealthUp() //maximum health remains unlimited
    {
        health += healthUpValue; //increase the value by the amount given
    }

    public float GetProjectileSpeed()
    {
        return projectileSpeed;
    }

    public int GetProjectileDamage()
    {
        return projectileDamage;
    }

    public bool GetIsAcid()
    {
        return isAcid;
    }

    public float GetAcidSlow()
    {
        return acidPercentageSlow;
    }

    public float GetAcidSlowTime()
    {
        return acidSlowTime;
    }
}
