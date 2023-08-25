using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Sprite spriteIdle;

    public float jumpForce = 10f;
    public float walkingSpeed = 2f;

    Rigidbody2D playerRigiBody;
    Animator playerAnimator;
    Vector3 startPosition; //va a guardar la posicion en donde comienza el player

    const string STATE_ALIVE = "IsAlive";
    const string STATE_ON_THE_GROUND = "IsOnTheGround";

    public LayerMask groundMask; // variable que se encarga de saber quien es el suelo configurando en el layer de unity como ground

    //Variables de posiones
    [SerializeField] // me permite visualizar en el edito una variable priuvada 
    private int healthPoints, manaPoints; //traquea puntos de vida y mana points

    public const int INITIAL_HEALTH = 100, INITIAL_MANA = 15, // Rangos establecidos del juego
                     MAX_HEALTH = 200, MAX_MANA = 30,
                     MIN_HEALTH = 10, MIN_MANA = 0;

    // variable score
    public float maxDistanceScore = 0f;
    private int score = 0;


    private void Awake()
    {
        playerRigiBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>(); //Tengo acceso al animator

    }

    void Start()
    {
        playerAnimator.enabled = false;
        startPosition = this.transform.position; //guardo en la variable la posicion en la que inicio 
    }


    public void StartGame()
    {
        this.gameObject.SetActive(true); //hay q reactivarlo por q despues de ganar desaparece

        playerAnimator.SetBool(STATE_ALIVE, true); //configuro las variables como las quiero en un comienzo
        playerAnimator.SetBool(STATE_ON_THE_GROUND, false);

        ReStartPosition();
        playerAnimator.Play("Walk");

        healthPoints = INITIAL_HEALTH; //Inicializo los puntos de vida y los de mana en inicial
        manaPoints = INITIAL_MANA;
    }

    void ReStartPosition()
    {
        this.transform.position = startPosition; // va a mandar al personaje a la posicion inicial
        this.playerRigiBody.velocity = Vector2.zero; // le bajo la vel a 0 para que la restablezca la gravedad para evitar el bug de atravesa el collider por la velocidad de muerte
    }
    

    void Update()
    {
        if (Input.GetButtonDown("Jump") && GameManager.sharedInstance.currentGameState == GameState.inGame)
        {
            playerAnimator.enabled = true;
            GetComponent<AudioSource>().Play();
            Jump();
        }


        bool isGrounded = IsTouchingTheMask();
        bool isMovingHorizontally = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f; //

        if (!isMovingHorizontally && isGrounded) {
            playerAnimator.enabled = false;
            playerRigiBody.GetComponent<SpriteRenderer>().sprite = spriteIdle;
        }
        else {
            playerAnimator.enabled = true;
        }


        playerAnimator.SetBool(STATE_ON_THE_GROUND, IsTouchingTheMask()); //Configuro para que siempre se alcualice al valor que consulta si esta tocando el suelo
       
    }


    void FixedUpdate()
    {
            if (GameManager.sharedInstance.currentGameState == GameState.inGame) { // solo se puede mover si estoy In Game
                Move();
            }
    }


    void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        bool jumping = Input.GetButtonDown("Jump");
        bool isRunning = Input.GetKey(KeyCode.LeftShift); // Verifica si se mantiene presionada la tecla Shift

        float moveSpeed = isRunning ? walkingSpeed * 2 : walkingSpeed; // Ajusta la velocidad de movimiento

        if (horizontalInput < 0)
        {
            playerRigiBody.GetComponent<SpriteRenderer>().flipX = true; // da vuelta el sprit
            playerRigiBody.velocity = new Vector2(-moveSpeed, playerRigiBody.velocity.y); // da vuelta el movimiento
        }
        else if (horizontalInput > 0)
        {
            playerRigiBody.GetComponent<SpriteRenderer>().flipX = false;
            playerRigiBody.velocity = new Vector2(moveSpeed, playerRigiBody.velocity.y);
        }
 
        float animatorSpeed = isRunning ? playerAnimator.speed = 2f : playerAnimator.speed = 1f; //Si esta corriendo entro y cambio el speed a dos sino a 1
    }

   
    void Jump()
    {
        if (IsTouchingTheMask()) {
            bool isRunning = Input.GetKey(KeyCode.LeftShift); // Verifica si se mantiene presionada la tecla Shift

            playerAnimator.enabled = true;
            if (isRunning)
            {
                playerRigiBody.AddForce(Vector2.up * (jumpForce * 1.2f), ForceMode2D.Impulse); // si esta apretado shif salta mas alto 
            } else
            {
                playerRigiBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    bool IsTouchingTheMask()
    {
        if (Physics2D.Raycast(this.transform.position, Vector2.down, 2f, groundMask))
        {
            return true;
        } else {
            return false;
        }
    }

    public void Die()
    {
        float travelledDistance = GetTravelledDistance();
        float previousMaxDistance = PlayerPrefs.GetFloat("maxScore", 0f); // le pido el max score, configurandola en 0 si nadie la configuro 
        if(travelledDistance > previousMaxDistance) // si la distancia que hago supera la distancia previa 
        {
            PlayerPrefs.SetFloat("maxScore", travelledDistance); // a player prefs le indico que configure en max score la variable travelledDistance actual 
        }

        playerAnimator.SetBool(STATE_ALIVE, false);

        playerAnimator.SetBool(STATE_ON_THE_GROUND, false);
        GameManager.sharedInstance.GameOver(); // Invoco al met game over del Game Manager para que le informe en que estado estoy
    }

    //Necesito metod para recoger vidas y mana

    public void CollectHealth(int points)//parametro: cuandos puntos de vida recuperaremos
    {
        this.healthPoints += points; //le sumo a los puntos de vida los puntos recibidos
        if (this.healthPoints >= MAX_HEALTH) // Si los puntos de vida actual superan los puntos maximos 
        {
            this.healthPoints = MAX_HEALTH; // los puntos de vida quedaran como los max.
        }

        if(healthPoints <= 0) // matar añ personaje
        {
            Die();
        }
    }

    public void CollectDie()//parametro: cuandos puntos de mana recuperaremos
    {
        Die();
    }

    public int GetHealth() {
        return healthPoints;
    }

    public int GetMana() {
        return manaPoints;
    }


    // para el score
    public float GetTravelledDistance()
    {
        if (GameManager.sharedInstance.currentGameState == GameState.inGame)// para evitar que tome la distancia en el game over
        {
            float distance = this.transform.position.x - startPosition.x; // a que distancia me hayo - el punto inicial = total de espacio recorrido en la dimension x
            if (distance > maxDistanceScore)
            { // solo se incrementa si la distancia actual es mayor que la maxima registrada
                maxDistanceScore = distance;
            }
        }
        return maxDistanceScore;
    }

    public void CollectPoints(int pointsToAdd)
    {
        // Suma los puntos recibidos al puntaje actual del jugador
        score += pointsToAdd;
    }

    //movimiento de la plataforma:
    private void OnCollisionEnter2D(Collision2D collision) //verifica cuando colisionamos con algo
    {
        if (collision.gameObject.tag == "MovingPlatform")
        {
            transform.parent = collision.transform; //nuestro padre va a ser el collision
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MovingPlatform")
        {
            transform.parent = null; //dejo de ser hijo
        }
    }

    public void PlayerWin()
    {   //se repite con die
        float travelledDistance = GetTravelledDistance();
        float previousMaxDistance = PlayerPrefs.GetFloat("maxScore", 0f); // le pido el max score, configurandola en 0 si nadie la configuro 
        if (travelledDistance > previousMaxDistance) // si la distancia que hago supera la distancia previa 
        {
            PlayerPrefs.SetFloat("maxScore", travelledDistance); // a player prefs le indico que configure en max score la variable travelledDistance actual 
        }
        GameManager.sharedInstance.WinGame(); // Invoco al met game over del Game Manager para que le informe en que estado estoy
    }
 

}




