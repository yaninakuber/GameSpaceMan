using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nave : MonoBehaviour
{
    public GameObject objectToMove; // Objeto a mover
    public Transform startPoint;
    public Transform endPoint;
    public float velocity;

    private bool isMovingUp = false;
    private bool isReturning = false;
    private Vector3 initialPosition;

    private PlayerController controller;

    private void Start()
    {
        controller = GameObject.Find("Player").GetComponent<PlayerController>(); // Obtiene una referencia al controlador del jugador
        initialPosition = objectToMove.transform.position; // Almacena la posición inicial del objeto
    }

    private void Update()
    {
        if (isMovingUp)
        {
            MoveObjectUp();  // Si está en movimiento hacia arriba, llama a la función correspondiente
        }
        else if (isReturning)
        {
            ReturnToObjectStart(); // Si está regresando a la posición inicial, llama a la función correspondiente
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isMovingUp = true; // Comienza a mover el objeto hacia arriba
            collision.gameObject.SetActive(false); // Desactiva al jugador
            StartCoroutine(WaitForWin()); // Inicia una corrutina para esperar y luego llamar al método ganador del jugador
        }
    }

    private IEnumerator WaitForWin()
    {
        yield return new WaitForSeconds(3f); // Espera 3 segundos
        controller.PlayerWin(); // Llama al método ganador
    }

    private void MoveObjectUp()
    {
        objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, endPoint.position, velocity * Time.deltaTime);

        if (objectToMove.transform.position == endPoint.position) // da la vuelta
        {
            isMovingUp = false; // Detiene el movimiento hacia arriba
            isReturning = true; // Comienza a regresar
        }
    }

    private void ReturnToObjectStart()
    {
        objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, initialPosition, velocity * Time.deltaTime);

        if (objectToMove.transform.position == initialPosition)
        {
            isReturning = false; // Detiene el regreso
        }
    }
}

/*
private Animator animator;
private PlayerController controller;

private Vector3 startPosition;


// Start is called before the first frame update
void Start()
{
    controller = GameObject.Find("Player").GetComponent<PlayerController>();
    startPosition = this.transform.position; //guardo en la variable la posicion en la que inicio 

    animator = GetComponent<Animator>();// Obtener la referencia al Animator
    animator.enabled = false; // me aseguro que este desactivada
}

private IEnumerator WaitForWin()
{
    yield return new WaitForSeconds(3f); // espera 3 segundos
    controller.PlayerWin(); //llama al metodo ganador
}



private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Player"))
    {
        animator.enabled = true;
        animator.Play("Nave 1");

        // Desactivar al jugador
        collision.gameObject.SetActive(false);

        StartCoroutine(WaitForWin());
    }
}*/

