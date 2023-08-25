using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public GameObject objectToMove; //objeto a mover

    public Transform startPoint;
    public Transform endPoint;

    public float velocity;

    private Vector3 moveTowards; //Mover hacia

  

    private void Start()
    {
            moveTowards = endPoint.position; //cuando arranca

    }

    private void Update()
    {
        if (GameManager.sharedInstance.currentGameState == GameState.inGame)
        {
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, moveTowards, velocity * Time.deltaTime); //desde donde hacia donde,hacia donde, en que tiempo que esta guardando en el start el endpoint

            if (objectToMove.transform.position == endPoint.position)// si el objeto llego al punto final de movimiento
            {
                moveTowards = startPoint.position; //ahora se va a mover hacia el start
            }

            if (objectToMove.transform.position == startPoint.position)// empieza de nuevo
            {
                moveTowards = endPoint.position;
            }
        }
      
    }

    
}


