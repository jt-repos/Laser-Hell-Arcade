using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    //[SerializeField] Transform[,] rushWaypoints;
    [SerializeField] Transform[,] MoveWaypoints;
    //[SerializeField] Transform[] MoveCentreWaypoints;
    [SerializeField] List<GameObject> movePathPrefabs;
    int waypointIndex = 0;
    bool processingAttack = false;
    private int numberOfAttacks = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!processingAttack)
        {
            int randomRoll = Random.Range(0, numberOfAttacks); //no hard code no co ci powiem 
            if (randomRoll == 0)
            {
                MoveAndFire();
            }
            else if (randomRoll == 1)
            {
                A();
            }
        }
    }

    private void MoveAndFire() //zreturnowac to do updata albo ustawic serialize na currentWaypoints i tu je zmienic zeby update uzywal
    {
        processingAttack = true;
        GetComponent<Enemy>().SpawnAllProjectiles();
        var randomIndex = Random.Range(0, MoveWaypoints.Length);
        var waypoints = GetWaypoints(movePathPrefabs[randomIndex]);
        Move(waypoints);
    }

    private List<Transform> GetWaypoints(GameObject pathPrefab)
    {
        var waypoints = new List<Transform>();
        foreach (Transform child in pathPrefab.transform)
        {
            waypoints.Add(child);
        }
        return waypoints;
    }

    private void ArrayMove(int randomIndex)
    {
        List<Transform> waypoints = new List<Transform>(); //jejuuu, każdy waypoint w wybranych waypointach
        for (int i = 0; i < MoveWaypoints.GetLength(randomIndex); i++)
        {
            waypoints.Add(MoveWaypoints[randomIndex, i]); //jest dodany do nowej listy oddzielnie
        }
        Move(waypoints); //i ta lista jest passowana do funkcji
    }

    private void A()
    {
    }

    private void LowHealthBoost()
    {
        //projectile number up
        //rush speed up
    }

    private void Move(List<Transform> waypoints) //update
    {
        if (waypointIndex <= waypoints.Count - 1)
        {
            var targetPosition = waypoints[waypointIndex].position;
            var moveSpeed = GetComponent<Enemy>().GetMoveSpeed();
            var movementThisFrame = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);
            if (transform.position == targetPosition)
            {
                waypointIndex++;
            }
        }
    }
}
