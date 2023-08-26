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

    Vector3 startPosition; 

    const string STATE_ALIVE = "IsAlive";
    const string STATE_ON_THE_GROUND = "IsOnTheGround";

    public LayerMask groundMask; 


    [SerializeField] private int healthPoints, manaPoints;

    public const int INITIAL_HEALTH = 100,
                     MAX_HEALTH = 200,
                     MIN_HEALTH = 10;

    public float maxDistanceScore = 0f;
    private int score = 0;


    private void Awake()
    {
        InicializeComponents();
    }

    private void InicializeComponents()
    {
        playerRigiBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        InitializePlayer();
    }
    private void InitializePlayer()
    {
        playerAnimator.enabled = false;
        startPosition = this.transform.position; 
    }


    public void StartGame()
    {
        InitializePlayerState();

        ReStartPosition();
        playerAnimator.Play("Walk");

        InitizalizePoints();

    }

    private void InitializePlayerState()
    {
        ReactivatePlayer();
        playerAnimator.SetBool(STATE_ALIVE, true);
        playerAnimator.SetBool(STATE_ON_THE_GROUND, false);
        
    }
    private void InitizalizePoints()
    {
        healthPoints = INITIAL_HEALTH;
        maxDistanceScore = 0f;
        GameManager.sharedInstance.RestartCollectableObject();
    }

    private void ReactivatePlayer()
    {
        this.gameObject.SetActive(true);
    }


    void ReStartPosition()
    {
        ResetPlayerPosition();
        ResetPlayerVelocity();
    }
    
    void ResetPlayerPosition()
    {
        this.transform.position = startPosition;
    }

    void ResetPlayerVelocity()
    {
        this.playerRigiBody.velocity = Vector2.zero; // le bajo la vel a 0 para que la restablezca la gravedad para evitar el bug de atravesa el collider por la velocidad de muerte
    }


    void Update()
    {
        HandleJump();
        HandleMovement();
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && IsInGame())
        {
            ActivatePlayerAnimator();
            GetComponent<AudioSource>().Play();
            Jump();
        }
    }

    void HandleMovement()
    {
        bool isGrounded = IsTouchingTheMask();
        bool isMovingHorizontally = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f;
       
        if (!isMovingHorizontally && isGrounded)
        {
            SetPlayerIdle();
        }
        else
        {
            ActivatePlayerAnimator();
        }

        playerAnimator.SetBool(STATE_ON_THE_GROUND, isGrounded);
    }

    private void SetPlayerIdle()
    {
        DesactivatePlayerAnimator();
        playerRigiBody.GetComponent<SpriteRenderer>().sprite = spriteIdle;
    }

    void ActivatePlayerAnimator()
    {
        playerAnimator.enabled = true;
    }

    void DesactivatePlayerAnimator()
    {
        playerAnimator.enabled = false;
    }

    public bool IsInGame()
    {
        return GameManager.sharedInstance.currentGameState == GameState.inGame;
    }

    void FixedUpdate()
    {
        CheckPosibilityOfMovement();
    }

    void CheckPosibilityOfMovement()
    {
        if (IsInGame())
        {
            Move();
        }
    }


    void Move()
    {
        float moveSpeed = CheckTouchRunKey() ? walkingSpeed * 2 : walkingSpeed;

        if (CheckTouchMoveKey() < 0)
        {
            FlipCharacter(true);
            MoveHorizontally(-moveSpeed);

        }
        else if(CheckTouchMoveKey() > 0)
        {
            FlipCharacter(false);
            MoveHorizontally(moveSpeed);
        }
        else if(CheckTouchMoveKey() == 0)
        {
            MoveHorizontally(0f);
        }

        AdjustAnimatorSpeed(CheckTouchRunKey());

    }

    void FlipCharacter(bool flip)
    {
        playerRigiBody.GetComponent<SpriteRenderer>().flipX = flip;
    }

    void MoveHorizontally(float speed) { 
        playerRigiBody.velocity = new Vector2(speed, playerRigiBody.velocity.y);
    }

    void AdjustAnimatorSpeed(bool isRunning)
    {
        float animatorSpeed = isRunning ? 2f: 1f;
        playerAnimator.speed = animatorSpeed;
    }

    private bool CheckTouchRunKey()
    {
        return Input.GetKey(KeyCode.LeftShift);
        
    }

    private float CheckTouchMoveKey()
    {
        float horizontalInmput = Input.GetAxis("Horizontal");
        return horizontalInmput;
    }
    private bool CheckTouchJumpKey()
    {
        return Input.GetButtonDown("Jump");
    }



    void Jump()
    {
        if (IsTouchingTheMask()) 
        {
            float jumpForceMultiplier = CheckTouchJumpKey() ? 1.2f : 1.0f;
            ActivatePlayerAnimator();
            playerRigiBody.AddForce(Vector2.up * (jumpForce * jumpForceMultiplier), ForceMode2D.Impulse);
        }
    }

    bool IsTouchingTheMask()
    {
        return Physics2D.Raycast(this.transform.position, Vector2.down, 2f, groundMask);
    }


    public void Die()
    {
        SetPlayerAnimatorParametersOnDeath();

        GameManager.sharedInstance.GameOver();
    }


    

    private void SetPlayerAnimatorParametersOnDeath()
    {
        playerAnimator.SetBool(STATE_ALIVE, false);
        playerAnimator.SetBool(STATE_ON_THE_GROUND, false);
    }

    public float GetTravelledDistance()
    {
        if (IsInGame())// para evitar que tome la distancia en el game over
        {
            float distance = this.transform.position.x - startPosition.x; // a que distancia me hayo - el punto inicial = total de espacio recorrido en la dimension x
            if (distance > maxDistanceScore)
            { // solo se incrementa si la distancia actual es mayor que la maxima registrada
                maxDistanceScore = distance;
            }
        }
        return maxDistanceScore;
    }

    public void CollectHealth(int points)
    {
        ClampHealthPoints();
        this.healthPoints += points;

        if(healthPoints <= 0) 
        {
            Die();
        }
    }

    private void ClampHealthPoints()
    {
        healthPoints = Mathf.Clamp(healthPoints, MIN_HEALTH, MAX_HEALTH);
    }

    public void CollectDie()
    {
        Die();
    }

    public int GetHealth() {
        return healthPoints;
    }

    public void CollectPoints(int pointsToAdd)
    {
        score += pointsToAdd;
    }


    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.tag == "MovingPlatform")
        {
            transform.parent = collision.transform; 
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MovingPlatform")
        {
            transform.parent = null; 
        }
    }

    public void PlayerWin()
    {   
        GameManager.sharedInstance.WinGame(); 
    }

}




