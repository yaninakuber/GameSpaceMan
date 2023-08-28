using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MovingPlatform : MonoBehaviour
{
    public GameObject objectToMove;

    public Transform startPoint;
    public Transform endPoint;

    public float velocity;

    private Vector3 moveTowards; 

  

    private void Start()
    {
            moveTowards = endPoint.position; 
    }

    private void Update()
    {
        if (GameManager.sharedInstance.currentGameState == GameState.inGame)
        {
            MoveObject();
            CheckForMoventCompletion();
        }
    }

    void MoveObject()
    {
        objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, moveTowards, velocity * Time.deltaTime); //desde donde hacia donde
    }
    void CheckForMoventCompletion()
    {
        if (IsAtPosition(endPoint.position))
        {
            ChangeDirection(startPoint.position);
        }

        if (IsAtPosition(startPoint.position))
        {
            ChangeDirection(endPoint.position);
        }
    }

    private bool IsAtPosition(Vector3 position)
    {
        return objectToMove.transform.position == position;
    }

    void ChangeDirection(Vector3 newTarget)
    {
        moveTowards = newTarget;
    }

}


