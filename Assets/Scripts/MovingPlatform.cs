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
            _MoveObject();
            _CheckForMoventCompletion();
        }
    }

    private void _MoveObject()
    {
        ObjectToMove.transform.position = Vector3.MoveTowards(ObjectToMove.transform.position, _moveTowards, velocity * Time.deltaTime); //desde donde hacia donde
    }
    private void _CheckForMoventCompletion()
    {
        if (_IsAtPosition(EndPoint.position))
        {
            _ChangeDirection(StartPoint.position);
        }

        if (_IsAtPosition(StartPoint.position))
        {
            _ChangeDirection(EndPoint.position);
        }
    }

    private bool _IsAtPosition(Vector3 position)
    {
        return ObjectToMove.transform.position == position;
    }

    private void _ChangeDirection(Vector3 newTarget)
    {
        _moveTowards = newTarget;
    }

}


