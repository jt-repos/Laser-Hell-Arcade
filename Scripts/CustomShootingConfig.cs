using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Shooting Configuration")]

public class CustomShootingConfig : ScriptableObject
{
    [SerializeField] int shotsToShootCustom = 25;
    [SerializeField] bool defaultChangeInWaveAngle;
    [SerializeField] bool defaultAngleBetweenShots; //wyjeb
    [SerializeField] [Range(-360, 360)] float changeInWaveAngle; //doesn't work if default
    [SerializeField] [Range(-360, 360)] float angleBetweenShots; //doesn't work if default
    [SerializeField] [Range(0, 360)] float initialAngle; //0 is up
    [SerializeField] int numberOfWaves = 1;
    [SerializeField] float timeBetweenShots = 0.4f;
    [SerializeField] int numberOfProjectilesInWave = 8;
    [SerializeField] string description; //wyjeb

    public float GetChangeInWaveRotation()
    {
        if (defaultChangeInWaveAngle)
        {
            changeInWaveAngle = 180 / numberOfProjectilesInWave; //default change in rotation for each spawn of the shots
        }
        return changeInWaveAngle;
    }

    public int GetShotsToShootCustom() { return shotsToShootCustom; }
    public bool GetDefaultAngleBetweenShots() { return defaultAngleBetweenShots; }
    public float GetChangeInWaveAngle() { return changeInWaveAngle; }
    public float GetAngleBetweenShots() { return angleBetweenShots; }
    public float GetInitialAngle() { return initialAngle; }
    public int GetNumberOfWaves() { return numberOfWaves; }
    public float GetTimeBetweenShots() { return timeBetweenShots; }
    public int GetNumberOfProjectilesInWave() { return numberOfProjectilesInWave; }
}
