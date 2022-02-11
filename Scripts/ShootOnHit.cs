using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootOnHit : MonoBehaviour
{
    [Header("ShootOnHit")]
    [SerializeField] AudioClip shootSFX;
    [SerializeField] GameObject laserPrefab;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.3f;
    [SerializeField] float minShootOnHitCooldown = 0.9f;
    [SerializeField] float maxShootOnHitCooldown = 1.1f;
    float shootOnHitCooldown;
    float currentRotation;

    [Header("Spawning")]
    [SerializeField] bool defaultChangeInRotation;
    [SerializeField] [Range(-360, 360)] float changeInRotation; //doesn't work if default
    [SerializeField] [Range(0, 360)] float initialRotation; //0 is up
    [SerializeField] int numberOfWaves = 1;
    [SerializeField] float timeBetweenShots = 0.4f;
    [SerializeField] int numberOfProjectiles = 8;
    [SerializeField] float projectileSpeed = 2f;

    // Use this for initialization
    void Start ()
    {
        if (defaultChangeInRotation)
        {
            changeInRotation = 180 / numberOfProjectiles; //default change in rotation for each spawn of the shots
        }
        shootOnHitCooldown = Random.Range(minShootOnHitCooldown, maxShootOnHitCooldown);
    }
	
	// Update is called once per frame
	void Update ()
    {
        shootOnHitCooldown -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (shootOnHitCooldown <= 0f)
        {
            StartCoroutine(ShootAround());
            shootOnHitCooldown = Random.Range(minShootOnHitCooldown, maxShootOnHitCooldown);
        }
    }

    IEnumerator ShootAround()
    {
        for (int numberOfShotsFired = 0; numberOfShotsFired < numberOfWaves; numberOfShotsFired++)
        {
            SpawnShots(numberOfShotsFired); //passes amount of shots fire to adjust the change in rotation
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    private void SpawnShots(int numberOfShotsFired)
    {
        for (int i = 1; i < numberOfProjectiles + 1; i++)
        {
        currentRotation = i * 360 / numberOfProjectiles + 180 / numberOfProjectiles + changeInRotation*numberOfShotsFired + initialRotation; //każdy pocisk dostaje własny kąt, tak żeby były równomiernie rozłożone + połowa początkowego kąta, żeby nie było pocisku celującego centralnie na dół
        GameObject laser = Instantiate
            (laserPrefab,
            transform.position,
            Quaternion.Euler(0f, 0f, currentRotation)) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = -(laser.GetComponent<Rigidbody2D>().transform.up * projectileSpeed); //leci w strone w która patrzy
        }
        AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, shootSoundVolume);
    }
}

