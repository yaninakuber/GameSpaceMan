using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float VelocityEnemy = 1.5f;
    public float PlayerJumpForceOnDeath =5f; 
    public float PlayerColorChangeDurationOnCollision = 0.2f; 


    Rigidbody2D enemyRigidBody;

    public bool FacingRight = false; // para saber donde esta mirando el enemigo, como esta para la izq. la inicializamos en false
    public int EnemyDamage = 10;

    private Vector3 startPosition; // para resetear la posicion del enemigo a la original cuando lo reutilizo

    private SpriteRenderer enemySpriteRenderer; // no se usa

    private PlayerController _playerController; // no se usa

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

    private void UpdateDirectionEnemy()
    {
        float currentVelocityEnemy = CalculateCurrentVelocity();
        float rotationY = FacingRight ? 180 : 0;

        SetFacingDirection(rotationY);
        UpdateEnemyVelocity(currentVelocityEnemy);
    }

    private float CalculateCurrentVelocity()
    {
        return FacingRight ? VelocityEnemy : -VelocityEnemy;
    }

    private void SetFacingDirection(float rotationY)
    {
        this.transform.eulerAngles = new Vector3(0, rotationY, 0);
    }

    private void UpdateEnemyVelocity(float currentVelocity)
    {
        enemyRigidBody.velocity = GameManager.SharedInstance.CurrentGameState == GameState.InGame
                                  ? new Vector2(currentVelocity, enemyRigidBody.velocity.y)
                                  : Vector2.zero; // if
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
        else // atacar todos los casos
        {
            RotateEnemy();
        }
    }

    private void HandlePlayerCollision(GameObject player)
    {
        GetComponent<AudioSource>().Play();

        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.CollectHealth(-EnemyDamage);
        
        StartCoroutine(ChangePlayerColorAndRevert(player));
        
        MakePlayerJump(player);
    }

    private void MakePlayerJump(GameObject player)
    {
        Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();
        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, PlayerJumpForceOnDeath);
    }

    private void RotateEnemy()
    {
        FacingRight = !FacingRight;
    }


    private IEnumerator ChangePlayerColorAndRevert(GameObject player)
    {
        Color originalColor = player.GetComponent<SpriteRenderer>().color;
        Color temporaryColor = new Color(1f, 0.7f, 0.7f); // Rojo más claro

        SetPlayerColor(player, temporaryColor);

        yield return new WaitForSeconds(PlayerColorChangeDurationOnCollision);

        SetPlayerColor(player, originalColor);

        StartCoroutine(RevertPlayerColor(player)); // Agregamos la llamada a la corrutina para revertir el color nuevamente después de un tiempo
    }

    private void SetPlayerColor(GameObject player, Color color)
    {
        player.GetComponent<SpriteRenderer>().color = color;
    }

    private IEnumerator RevertPlayerColor(GameObject player)
    {
        Color originalColor = player.GetComponent<SpriteRenderer>().color; //guardamos el color original
        Color defaultColor = Color.white; 
        
        float elapsedTime = 0f;
        while (elapsedTime < PlayerColorChangeDurationOnCollision) // mientras el tiempo transcurrido sea menor que la duracion deseada para el cambio de color. Garantiza que el cambio de color se realice gradualmente. 
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / PlayerColorChangeDurationOnCollision; //calculo de progreso //renombrar a otro nombre que no sea t
            SetPlayerColor(player, Color.Lerp(originalColor, defaultColor, t)); //color lerp inerpola suavemente entre el color original y el nuevo color 
            yield return null; //retornamos a la corruntina. //tratar de evitar null 
        }
    }

}
