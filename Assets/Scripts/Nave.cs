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

    private MovementState movementState = MovementState.Idle; 

    private void Start()
    {
        _controller = GameObject.Find("Player").GetComponent<PlayerController>();
        _initialPosition = ObjectToMove.transform.position;
    }

    private void Update()
    {
        switch (movementState)
        {
            case MovementState.MovingUp:
                MoveObjectTowards(EndPoint.position);
                CheckForMovementCompletion(EndPoint.position, MovementState.Returning);
                break;

            case MovementState.Returning:
                MoveObjectTowards(_initialPosition);
                CheckForMovementCompletion(_initialPosition, MovementState.Idle);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           HandlePlayerCollision(collision.gameObject);
        }
    }

    private void HandlePlayerCollision(GameObject player)
    {
        StartMovingUp();
        DesactivatePlayer(player);
        StartCoroutine(_WaitForWin());
    }


    private void StartMovingUp()
    {
        movementState = MovementState.MovingUp;
    }

    private void DesactivatePlayer(GameObject player)
    {
        player.SetActive(false);
    }

    private IEnumerator _WaitForWin()
    {
        yield return new WaitForSeconds(3f);
        _controller.PlayerWin();
    }



    private void MoveObjectTowards(Vector3 targetPosition)
    {
        ObjectToMove.transform.position = Vector3.MoveTowards(ObjectToMove.transform.position, targetPosition, Velocity * Time.deltaTime);
    }

    private void CheckForMovementCompletion(Vector3 targetPosition, MovementState nextState)
    {
        if (ObjectToMove.transform.position == targetPosition)
        {
            movementState = nextState;
        }
    }
}

 





