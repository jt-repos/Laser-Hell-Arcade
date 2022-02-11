using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpeed : MonoBehaviour
{
    private ParticleSystem particles;
    [SerializeField] float slowSimulationSpeed = 0.8f;
    [SerializeField] float fastSimulationSpeed = 2f;
    [SerializeField] float transitionTime = 1f;
    [SerializeField] int numberOfTransitions = 60;
    float currentSimulationSpeed;


    void Start()
    {
        currentSimulationSpeed = slowSimulationSpeed;
        particles = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        var main = particles.main;
        main.simulationSpeed = currentSimulationSpeed;
    }

    private void Awake()
    {
        SetUpSingleton();
    }

    private void SetUpSingleton()
    {
        if (FindObjectsOfType(GetType()).Length > 2) //cause there are 2 objects of the same type
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SlowDownSimulation()
    {
        StartCoroutine(ChangeSpeed(slowSimulationSpeed));
    }

    public void SpeedUpSimulation()
    {
        StartCoroutine(ChangeSpeed(fastSimulationSpeed));
    }

    private IEnumerator ChangeSpeed(float newSimulationSpeed)
    {
        float interval = transitionTime / numberOfTransitions - Time.deltaTime;
        float speedIncrease = (newSimulationSpeed - currentSimulationSpeed) / numberOfTransitions;
        for (int i = 0; i <= numberOfTransitions; i++)
        {
            currentSimulationSpeed = currentSimulationSpeed + speedIncrease;
            yield return new WaitForSeconds(interval);
        }
    }
}
