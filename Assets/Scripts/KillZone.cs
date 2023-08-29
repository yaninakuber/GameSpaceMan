using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.tag == "Player") 
        { 
            _KillPlayer(collision.gameObject);
        }
    }

    private void _KillPlayer (GameObject player)
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        controller.Die();
    }
}

