using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvasiveMovement : MonoBehaviour
{

    [Header("Evasion")]
    [SerializeField] bool evasive = true;
    [SerializeField] float timeOfEvasion = 0.5f;
    [SerializeField] float evasionCooldown = 2f;

    [Header("Movement Changes")]
    [SerializeField] float radiusDivisor = 70f; //raczej nie ruszaj
    [SerializeField] float rotateSpeedDivisor = 2f; //i tego bo sie psuje
    private float rotateSpeed;
    private float radius;
    private float angle;

    [Header("Audio")]
    [SerializeField] AudioClip evadeSFX;
    [SerializeField] [Range(0, 1)] float evadeSoundVolume = 0.3f;

    [Header("Video")]
    [SerializeField] GameObject evadeVFX;
    [SerializeField] [Range(0f, 1f)] float fade = 0.5f;
    [SerializeField] float durationOfEffect = 1f;
    Color color;


    // Start is called before the first frame update
    void Start()
    {
        var waveConfig = FindObjectOfType<EnemySpawner>().GetCurrentWave();
        rotateSpeed = waveConfig.GetMoveSpeed() / rotateSpeedDivisor; //lepiej nie zmieniac bo sie psuje
        radius = waveConfig.GetMoveSpeed() / radiusDivisor; //to tez
        color = GetComponent<SpriteRenderer>().color;
        if (evasive)
        {
            StartCoroutine(EnterEvade());
        }
    }

    private void Update()
    {
        if (FindObjectOfType<EvasiveMovement>().GetEvasive() == true) //(if equals true), robi siu wziu
        {
            angle += rotateSpeed * Time.deltaTime;
            var offset = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * radius;
            transform.position -= offset;
        }
    }

    IEnumerator EnterEvade()
    {
        color.a = fade;
        GetComponent<SpriteRenderer>().color = color;
        gameObject.layer = 13; //Evasive();
        GameObject evadeEffect = Instantiate(evadeVFX, transform.position, Quaternion.identity);
        Destroy(evadeEffect, durationOfEffect);
        AudioSource.PlayClipAtPoint(evadeSFX, Camera.main.transform.position, evadeSoundVolume);
        yield return new WaitForSeconds(timeOfEvasion);
        StartCoroutine(ExitEvade());
    }

    IEnumerator ExitEvade()
    {
        color.a = 1f;
        GetComponent<SpriteRenderer>().color = color;
        gameObject.layer = 9; //Enemy
        GameObject evadeEffect = Instantiate(evadeVFX, transform.position, Quaternion.identity);
        Destroy(evadeEffect, durationOfEffect);
        AudioSource.PlayClipAtPoint(evadeSFX, Camera.main.transform.position, evadeSoundVolume);
        yield return new WaitForSeconds(evasionCooldown);
        StartCoroutine(EnterEvade());
    }

    public bool GetEvasive()
    {
        return evasive;
    }
}
