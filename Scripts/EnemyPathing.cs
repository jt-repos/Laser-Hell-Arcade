using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    [SerializeField] bool standardPathing = true;

    //config parameters
    WaveConfig waveConfig;
    List<Transform> waypoints;
    int waypointIndex = 0;
    float moveSpeed;

    // Use this for initialization
    void Start ()
    {
        waypoints = waveConfig.GetWaypoints();
        transform.position = waypoints[waypointIndex].position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Move();
	}

    public void SetWaveConfig(WaveConfig waveConfig)
    {
        Debug.Log(222);
        this.waveConfig = waveConfig;
    }

    private void Move()
    {
        if (waypointIndex <= waypoints.Count - 1)
        {
            var targetPosition = waypoints[waypointIndex].position;
            var movementThisFrame = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);
            if (transform.position == targetPosition)
            {
                waypointIndex++;
            }
        }
        else
        {
            if (standardPathing)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetMoveSpeed(float newMoveSpeed)
    {
        moveSpeed = newMoveSpeed;
    }
}
