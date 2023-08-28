using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nave : MonoBehaviour
{
    public GameObject objectToMove;
    public Transform startPoint;
    public Transform endPoint;
    public float velocity;

    private Vector3 initialPosition;

    private PlayerController controller;

    private enum MovementState
    {
        Idle,
        MovingUp,
        Returning
    }

    private MovementState movementState = MovementState.Idle;

    private void Start()
    {
        controller = GameObject.Find("Player").GetComponent<PlayerController>();
        initialPosition = objectToMove.transform.position;
    }

    private void Update()
    {
        switch (movementState)
        {
            case MovementState.MovingUp:
                MoveObjectTowards(endPoint.position);
                CheckForMovementCompletion(endPoint.position, MovementState.Returning);
                break;

            case MovementState.Returning:
                MoveObjectTowards(initialPosition);
                CheckForMovementCompletion(initialPosition, MovementState.Idle);
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
        StartCoroutine(WaitForWin());
    }


    private void StartMovingUp()
    {
        movementState = MovementState.MovingUp;
    }

    private void DesactivatePlayer(GameObject player)
    {
        player.SetActive(false);
    }

    private IEnumerator WaitForWin()
    {
        yield return new WaitForSeconds(3f);
        controller.PlayerWin();
    }



    private void MoveObjectTowards(Vector3 targetPosition)
    {
        objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, targetPosition, velocity * Time.deltaTime);
    }

    private void CheckForMovementCompletion(Vector3 targetPosition, MovementState nextState)
    {
        if (objectToMove.transform.position == targetPosition)
        {
            movementState = nextState;
        }
    }
}

 





