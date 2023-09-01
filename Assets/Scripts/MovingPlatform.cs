using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MovingPlatform : MonoBehaviour
{
    public GameObject ObjectToMove;

    public Transform StartPoint;
    public Transform EndPoint;

    public float velocity;

    private Vector3 _moveTowards; 

  

    private void Start()
    {
        _moveTowards = EndPoint.position; 
    }

    private void Update()
    {
        if (GameManager.SharedInstance.CurrentGameState == GameState.InGame)
        {
            MoveObject();
            CheckForMoventCompletion();
        }
    }

    private void MoveObject()
    {
        ObjectToMove.transform.position = Vector3.MoveTowards(ObjectToMove.transform.position, _moveTowards, velocity * Time.deltaTime); //desde donde hacia donde
    }
    private void CheckForMoventCompletion()
    {
        if (IsAtPosition(EndPoint.position))
        {
            ChangeDirection(StartPoint.position);
        }

        if (IsAtPosition(StartPoint.position))
        {
            ChangeDirection(EndPoint.position);
        }
    }

    private bool IsAtPosition(Vector3 position)
    {
        return ObjectToMove.transform.position == position;
    }

    private void ChangeDirection(Vector3 newTarget)
    {
        _moveTowards = newTarget;
    }

}


