using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float velocityEnemy = 1.5f;
    public float jumpForce = 1f; // Fuerza de salto al chocar con el enemigo
    public float colorChangeDuration = 0.2f; // Duración del cambio de color en segundos


    Rigidbody2D enemyRigidBody;

    public bool facingRight = false; // para saber donde esta mirando el enemigo, como esta para la izq. la inicializamos en false
    public int enemyDamage = 10;// daño al enemigo

    private Vector3 startPosition; // para resetear la posicion del enemigo a la original cuando lo reutilizo

    private SpriteRenderer enemySpriteRenderer; //// Agrega esta variable para acceder al SpriteRenderer del enemigo


    private void Awake()
    {
        enemyRigidBody = GetComponent<Rigidbody2D>();
        startPosition = this.transform.position; // asi se donde arranca exactamente al comienzo

        enemySpriteRenderer = GetComponent<SpriteRenderer>();// Inicializa el SpriteRenderer
    }


    void Start()
    {
        this.transform.position = startPosition; //para que cada vez que arranque vuelva a la posicion inicial
    }


    private void FixedUpdate()
    {
        float currentVelocityEnemy = velocityEnemy; 

        if (facingRight)//siempre vamos a velocidad fija dependiendo del sentido
        {
            currentVelocityEnemy = velocityEnemy;//Mirando a la derecha, velocidad positiva
            this.transform.eulerAngles = new Vector3(0, 180, 0); // rota 180 grados con respecto a la pocision inicial
        }
        else
        {
            currentVelocityEnemy = -velocityEnemy;//Mirando a la izq, velocidad negativa
            this.transform.eulerAngles = Vector3.zero; // vuelve a la posicion original
        }

        if(GameManager.sharedInstance.currentGameState == GameState.inGame) // en el caso de estar en el juego, para que no se mueva en menu o en game over 
        {
            enemyRigidBody.velocity = new Vector2(currentVelocityEnemy, enemyRigidBody.velocity.y); // genero velocidad de acuerdo a lo calculado anteriormente en x y en y cero pero lo ponemos asi
        }
        else
        {
            enemyRigidBody.velocity = Vector2.zero; 
        }

    }


    private void OnTriggerEnter2D(Collider2D collision) // para que sea llamado cuando el enemy entra en collision con otro collider
    {
        if(collision.tag == "Coin" || collision.tag == "ExitZone" || collision.tag == "Potion")
        {
            return;
        }
        if (collision.tag == "Player")
        {
            GetComponent<AudioSource>().Play();
            collision.gameObject.GetComponent<PlayerController>().CollectHealth(-enemyDamage);// localizo al player controller, le paso valor negativo para que reste a la vida

            // Cambiar el color del jugador temporalmente y luego revertirlo
            StartCoroutine(ChangePlayerColorAndRevert(collision.gameObject));

            // Agregar la lógica para hacer que el jugador salte aquí
            Rigidbody2D playerRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);

            return;
        }
        // si llego hasta aca no choque ni con monedas ni con player
        // Lo mas normal es que aqui haya otro enemigo o el escenario
        //vamos a hacer que el enemigo rote
        facingRight = !facingRight; //lo contrario
    }

    private IEnumerator ChangePlayerColorAndRevert(GameObject player)
    {
        // Cambiar el color del jugador a uno temporal (por ejemplo, rojo)
        Color originalColor = player.GetComponent<SpriteRenderer>().color;
        Color newColor = new Color(1f, 0.7f, 0.7f); // Rojo más claro
        player.GetComponent<SpriteRenderer>().color = newColor;

        // Esperar durante unos segundos
        yield return new WaitForSeconds(colorChangeDuration);

        // Revertir el color original del jugador
        player.GetComponent<SpriteRenderer>().color = originalColor;

        // Agregamos la llamada a la corrutina para revertir el color nuevamente después de un tiempo
        StartCoroutine(RevertPlayerColor(player));
    }




    private IEnumerator RevertPlayerColor(GameObject player)
    {
        Color originalColor = player.GetComponent<SpriteRenderer>().color; //guardamos el color original
        Color newColor = new Color(1f, 1f, 1f); // Color blanco (color original)
        float elapsedTime = 0f;

        while (elapsedTime < colorChangeDuration) // mientras el tiempo transcurrido sea menor que la duracion deseada para el cambio de color. Garantiza que el cambio de color se realice gradualmente. 
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / colorChangeDuration; //calculo de progreso
            player.GetComponent<SpriteRenderer>().color = Color.Lerp(originalColor, newColor, t); //color lerp inerpola suavemente entre el color original y el nuevo color 
            yield return null; //retornamos a la corruntina. 
        }
    }

}
