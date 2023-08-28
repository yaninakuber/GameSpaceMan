using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float velocityEnemy = 1.5f;
    public float playerJumpForceOnDeath =5f; 
    public float playerColorChangeDurationOnCollision = 0.2f; 


    Rigidbody2D enemyRigidBody;

    public bool facingRight = false; // para saber donde esta mirando el enemigo, como esta para la izq. la inicializamos en false
    public int enemyDamage = 10;

    private Vector3 startPosition; // para resetear la posicion del enemigo a la original cuando lo reutilizo

    private SpriteRenderer enemySpriteRenderer;

    private PlayerController playerController;

    private void Awake()
    {
        enemyRigidBody = GetComponent<Rigidbody2D>();
        startPosition = this.transform.position;

        enemySpriteRenderer = GetComponent<SpriteRenderer>();
    }


    void Start()
    {
        this.transform.position = startPosition; 
    }


    private void FixedUpdate()
    {
        UpdateDirectionEnemy();
    }

    void UpdateDirectionEnemy()
    {
        float currentVelocityEnemy = CalculateCurrentVelocity();
        float rotationY = facingRight ? 180 : 0;

        SetFacingDirection(rotationY);
        UpdateEnemyVelocity(currentVelocityEnemy);
    }

    float CalculateCurrentVelocity()
    {
        return facingRight ? velocityEnemy : -velocityEnemy;
    }

    void SetFacingDirection(float rotationY)
    {
        this.transform.eulerAngles = new Vector3(0, rotationY, 0);
    }

    void UpdateEnemyVelocity(float currentVelocity)
    {
        enemyRigidBody.velocity = GameManager.sharedInstance.currentGameState == GameState.inGame
                                  ? new Vector2(currentVelocity, enemyRigidBody.velocity.y)
                                  : Vector2.zero;
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coin" || collision.tag == "ExitZone" || collision.tag == "Potion")
        {
            return;
        }
        if (collision.CompareTag("Player"))
        {
            HandlePlayerCollision(collision.gameObject);
        }
        else
        {
            RotateEnemy();
        }
    }

    void HandlePlayerCollision(GameObject player)
    {
        GetComponent<AudioSource>().Play();

        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.CollectHealth(-enemyDamage);
        
        StartCoroutine(ChangePlayerColorAndRevert(player));
        
        MakePlayerJump(player);
    }

    void MakePlayerJump(GameObject player)
    {
        Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();
        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, playerJumpForceOnDeath);
    }

    void RotateEnemy()
    {
        facingRight = !facingRight;
    }


    private IEnumerator ChangePlayerColorAndRevert(GameObject player)
    {
        Color originalColor = player.GetComponent<SpriteRenderer>().color;
        Color temporaryColor = new Color(1f, 0.7f, 0.7f); // Rojo más claro

        SetPlayerColor(player, temporaryColor);

        yield return new WaitForSeconds(playerColorChangeDurationOnCollision);

        SetPlayerColor(player, originalColor);

        StartCoroutine(RevertPlayerColor(player)); // Agregamos la llamada a la corrutina para revertir el color nuevamente después de un tiempo
    }

    void SetPlayerColor(GameObject player, Color color)
    {
        player.GetComponent<SpriteRenderer>().color = color;
    }

    private IEnumerator RevertPlayerColor(GameObject player)
    {
        Color originalColor = player.GetComponent<SpriteRenderer>().color; //guardamos el color original
        Color defaultColor = Color.white; 
        
        float elapsedTime = 0f;
        while (elapsedTime < playerColorChangeDurationOnCollision) // mientras el tiempo transcurrido sea menor que la duracion deseada para el cambio de color. Garantiza que el cambio de color se realice gradualmente. 
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / playerColorChangeDurationOnCollision; //calculo de progreso
            SetPlayerColor(player, Color.Lerp(originalColor, defaultColor, t)); //color lerp inerpola suavemente entre el color original y el nuevo color 
            yield return null; //retornamos a la corruntina. 
        }
    }

}
