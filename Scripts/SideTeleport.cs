using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideTeleport : MonoBehaviour
{

    [SerializeField] float teleportDisplacementX;
    [SerializeField] Player player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "player")
        {
            player.TeleportToOppositeSide(teleportDisplacementX);
        }
    }
}
