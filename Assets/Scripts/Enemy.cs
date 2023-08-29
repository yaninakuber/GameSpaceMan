using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float VelocityEnemy = 1.5f;
    public float PlayerJumpForceOnDeath =5f; 
    public float PlayerColorChangeDurationOnCollision = 0.2f; 


    Rigidbody2D _enemyRigidBody;

    public bool FacingRight = false; // para saber donde esta mirando el enemigo, como esta para la izq. la inicializamos en false
    public int EnemyDamage = 10;

    private Vector3 _startPosition; // para resetear la posicion del enemigo a la original cuando lo reutilizo

    private SpriteRenderer _enemySpriteRenderer; // no se usa

    private PlayerController _playerController; // no se usa

    private void Awake()
    {
        _enemyRigidBody = GetComponent<Rigidbody2D>();
        _startPosition = this.transform.position;

        _enemySpriteRenderer = GetComponent<SpriteRenderer>();
    }


    void Start()
    {
        this.transform.position = _startPosition; 
    }


    private void FixedUpdate()
    {
        _UpdateDirectionEnemy();
    }

    private void _UpdateDirectionEnemy()
    {
        float currentVelocityEnemy = _CalculateCurrentVelocity();
        float rotationY = FacingRight ? 180 : 0;

        _SetFacingDirection(rotationY);
        _UpdateEnemyVelocity(currentVelocityEnemy);
    }

    private float _CalculateCurrentVelocity()
    {
        return FacingRight ? VelocityEnemy : -VelocityEnemy;
    }

    private void _SetFacingDirection(float rotationY)
    {
        this.transform.eulerAngles = new Vector3(0, rotationY, 0);
    }

    private void _UpdateEnemyVelocity(float currentVelocity)
    {
        _enemyRigidBody.velocity = GameManager.SharedInstance.CurrentGameState == GameState.InGame
                                  ? new Vector2(currentVelocity, _enemyRigidBody.velocity.y)
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
            _HandlePlayerCollision(collision.gameObject);
        }
        else // atacar todos los casos
        {
            _RotateEnemy();
        }
    }

    private void _HandlePlayerCollision(GameObject player)
    {
        GetComponent<AudioSource>().Play();

        PlayerController _playerController = player.GetComponent<PlayerController>();
        _playerController.CollectHealth(-EnemyDamage);
        
        StartCoroutine(ChangePlayerColorAndRevert(player));
        
        _MakePlayerJump(player);
    }

    private void _MakePlayerJump(GameObject player)
    {
        Rigidbody2D _playerRigidbody = player.GetComponent<Rigidbody2D>();
        _playerRigidbody.velocity = new Vector2(_playerRigidbody.velocity.x, PlayerJumpForceOnDeath);
    }

    private void _RotateEnemy()
    {
        FacingRight = !FacingRight;
    }


    private IEnumerator ChangePlayerColorAndRevert(GameObject player)
    {
        Color _originalColor = player.GetComponent<SpriteRenderer>().color;
        Color _temporaryColor = new Color(1f, 0.7f, 0.7f); // Rojo más claro

        _SetPlayerColor(player, _temporaryColor);

        yield return new WaitForSeconds(PlayerColorChangeDurationOnCollision);

        _SetPlayerColor(player, _originalColor);

        StartCoroutine(_RevertPlayerColor(player)); // Agregamos la llamada a la corrutina para revertir el color nuevamente después de un tiempo
    }

    private void _SetPlayerColor(GameObject player, Color color)
    {
        player.GetComponent<SpriteRenderer>().color = color;
    }

    private IEnumerator _RevertPlayerColor(GameObject player)
    {
        Color _originalColor = player.GetComponent<SpriteRenderer>().color; //guardamos el color original
        Color _defaultColor = Color.white; 
        
        float elapsedTime = 0f;
        while (elapsedTime < PlayerColorChangeDurationOnCollision) // mientras el tiempo transcurrido sea menor que la duracion deseada para el cambio de color. Garantiza que el cambio de color se realice gradualmente. 
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / PlayerColorChangeDurationOnCollision; //calculo de progreso //renombrar a otro nombre que no sea t
            _SetPlayerColor(player, Color.Lerp(_originalColor, _defaultColor, t)); //color lerp inerpola suavemente entre el color original y el nuevo color 
            yield return null; //retornamos a la corruntina. //tratar de evitar null 
        }
    }

}
