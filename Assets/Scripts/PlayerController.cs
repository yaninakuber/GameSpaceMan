using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float JumpForce = 10f;
    public float WalkingSpeed = 2f;
    public LayerMask GroundMask;

    Rigidbody2D playerRigiBody;
    Animator playerAnimator;
    
    public Sprite SpriteIdle;

    Vector3 startPosition; 

    const string STATE_ALIVE = "IsAlive";
    const string STATE_ON_THE_GROUND = "IsOnTheGround";


    [SerializeField] private int healthPoints, _manaPoints;
    public const int INITIAL_HEALTH = 100,
                     MAX_HEALTH = 200,
                     MIN_HEALTH = 10;

    public float MaxDistanceScore = 0f;
    private int _score = 0;// no se usa


    private void Awake()
    {
        InicializeComponents();
    }

    private void InicializeComponents()
    {
        playerRigiBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        InitializePlayer();
    }

    private void InitializePlayer()
    {
        DesactivatePlayerAnimator();
        startPosition = this.transform.position; 
    }

    private void ActivatePlayerAnimator()
    {
        playerAnimator.enabled = true;
    }

    private void DesactivatePlayerAnimator()
    {
        playerAnimator.enabled = false;
    }

    public void StartGame()
    {
        InitializePlayerState();

        ResetPlayer();
        playerAnimator.Play("Walk");

        InitizalizePoints();
    }

    private void InitializePlayerState()
    {
        ReactivatePlayer();
        SetAnimatorState(STATE_ALIVE, true);
        SetAnimatorState(STATE_ON_THE_GROUND, false);
    }
    private void InitizalizePoints()
    {
        healthPoints = INITIAL_HEALTH;
        MaxDistanceScore = 0f;
        GameManager.SharedInstance.RestartCollectableObject();
    }

    private void ReactivatePlayer()
    {
        this.gameObject.SetActive(true);
    }

    private void SetAnimatorState(string stateName, bool value)
    {
        playerAnimator.SetBool(stateName, value);
    }

    private void ResetPlayer()
    {
        ResetPlayerPosition();
        ResetPlayerVelocity();
    }
    
    private void ResetPlayerPosition()
    {
        this.transform.position = startPosition;
    }

    private void ResetPlayerVelocity()
    {
        this.playerRigiBody.velocity = Vector2.zero; // le bajo la vel a 0 para que la restablezca la gravedad para evitar el bug de atravesa el collider por la velocidad de muerte
    }


    void Update()
    {
        HandleJump();
        HandleMovement();
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && CheckGameState(GameState.InGame))
        {
            ActivatePlayerAnimator();
            GetComponent<AudioSource>().Play();
            Jump(); 
        }
    }

    private void HandleMovement()
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
        playerRigiBody.GetComponent<SpriteRenderer>().sprite = SpriteIdle;
    }

    public bool CheckGameState(GameState targetState)
    {
        return GameManager.SharedInstance.CurrentGameState == targetState;
    }

    private void FixedUpdate()
    {
        CheckPosibilityOfMovement();
    }

    private void CheckPosibilityOfMovement()
    {
        if (CheckGameState(GameState.InGame))
        {
            Move();
        }
    }

    private void Move()
    {
        float moveSpeed = CheckTouchRunKey() ? WalkingSpeed * 2 : WalkingSpeed;

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

    private void FlipCharacter(bool flip)
    {
        playerRigiBody.GetComponent<SpriteRenderer>().flipX = flip;
    }

    private void MoveHorizontally(float speed) 
    { 
        playerRigiBody.velocity = new Vector2(speed, playerRigiBody.velocity.y);
    }

    private void AdjustAnimatorSpeed(bool isRunning)
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

    private void Jump()
    {
        if (IsTouchingTheMask()) 
        {
            float jumpForceMultiplier = CheckTouchRunKey() ? 1.5f : 1.3f;
            ActivatePlayerAnimator();
            playerRigiBody.AddForce(Vector2.up * (JumpForce * jumpForceMultiplier), ForceMode2D.Impulse);
        }
    }


    private bool IsTouchingTheMask()
    {
        return Physics2D.Raycast(this.transform.position, Vector2.down, 2f, GroundMask);
    }

    public void Die()
    {
        SetPlayerAnimatorParametersOnDeath();

        GameManager.SharedInstance.GameOver();
    }

    private void SetPlayerAnimatorParametersOnDeath()
    {
        SetAnimatorState(STATE_ALIVE, false);
        SetAnimatorState(STATE_ON_THE_GROUND, false);
    }

    public float GetTravelledDistance()
    {
        if (CheckGameState(GameState.InGame))// para evitar que tome la distancia en el game over
        {
            float distance = this.transform.position.x - startPosition.x; // a que distancia me hayo - el punto inicial = total de espacio recorrido en la dimension x
            if (distance > MaxDistanceScore)
            { // solo se incrementa si la distancia actual es mayor que la maxima registrada
                MaxDistanceScore = distance;
            }
        }
        return MaxDistanceScore;
    }

    public void CollectHealth(int points) // deberia ir en collectable
    {
        ClampHealthPoints();
        this.healthPoints += points;

        if(healthPoints <= 0) 
        {
            Die();
        }
    }

    private void ClampHealthPoints() // collectable
    {
        healthPoints = Mathf.Clamp(healthPoints, MIN_HEALTH, MAX_HEALTH);
    }

    public void CollectDie() // collectable
    {
        Die();
    }

    public int GetHealth()  // collectable
    {
        return healthPoints;
    }

    public void CollectPoints(int pointsToAdd) // collectable
    {
        _score += pointsToAdd;
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
        }    }


    public void PlayerWin()
    {   
        GameManager.SharedInstance.WinGame(); 
    }

}




