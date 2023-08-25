using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision) // pide como parametro un collider
    {
        if(collision.tag == "Player") { // si es un player el que colisiona
            PlayerController controller = collision.GetComponent<PlayerController>(); //Llamo al objeto PlayerController lo renombro a controller que va a representar el colisionador del player
            controller.Die(); //invoco el met de morir del playerController
        }
    }

}

