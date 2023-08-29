using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nave : MonoBehaviour
{
    public GameObject ObjectToMove;
    public Transform StartPoint;
    public Transform EndPoint;
    public float Velocity;

    private Vector3 _initialPosition;

    private PlayerController _controller;

    private enum MovementState
    {
        Idle,
        MovingUp,
        Returning
    }

    private MovementState _movementState = MovementState.Idle; 

    private void Start()
    {
        _controller = GameObject.Find("Player").GetComponent<PlayerController>();
        _initialPosition = ObjectToMove.transform.position;
    }

    private void Update()
    {
        switch (_movementState)
        {
            case MovementState.MovingUp:
                _MoveObjectTowards(EndPoint.position);
                _CheckForMovementCompletion(EndPoint.position, MovementState.Returning);
                break;

            case MovementState.Returning:
                _MoveObjectTowards(_initialPosition);
                _CheckForMovementCompletion(_initialPosition, MovementState.Idle);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           _HandlePlayerCollision(collision.gameObject);
        }
    }

    private void _HandlePlayerCollision(GameObject player)
    {
        _StartMovingUp();
        _DesactivatePlayer(player);
        StartCoroutine(_WaitForWin());
    }


    private void _StartMovingUp()
    {
        _movementState = MovementState.MovingUp;
    }

    private void _DesactivatePlayer(GameObject player)
    {
        player.SetActive(false);
    }

    private IEnumerator _WaitForWin()
    {
        yield return new WaitForSeconds(3f);
        _controller.PlayerWin();
    }



    private void _MoveObjectTowards(Vector3 targetPosition)
    {
        ObjectToMove.transform.position = Vector3.MoveTowards(ObjectToMove.transform.position, targetPosition, Velocity * Time.deltaTime);
    }

    private void _CheckForMovementCompletion(Vector3 targetPosition, MovementState nextState)
    {
        if (ObjectToMove.transform.position == targetPosition)
        {
            _movementState = nextState;
        }
    }
}

 





