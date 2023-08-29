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

    Vector3 _startPosition; 

    const string STATE_ALIVE = "IsAlive";
    const string STATE_ON_THE_GROUND = "IsOnTheGround";


    [SerializeField] private int _healthPoints, _manaPoints;
    public const int INITIAL_HEALTH = 100,
                     MAX_HEALTH = 200,
                     MIN_HEALTH = 10;

    public float MaxDistanceScore = 0f;
    private int score = 0;// no se usa


    private void Awake()
    {
        _InicializeComponents();
    }

    private void _InicializeComponents()
    {
        playerRigiBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        _InitializePlayer();
    }

    private void _InitializePlayer()
    {
        _DesactivatePlayerAnimator();
        _startPosition = this.transform.position; 
    }

    private void _ActivatePlayerAnimator()
    {
        playerAnimator.enabled = true;
    }

    private void _DesactivatePlayerAnimator()
    {
        playerAnimator.enabled = false;
    }

    public void StartGame()
    {
        _InitializePlayerState();

        _ResetPlayer();
        playerAnimator.Play("Walk");

        _InitizalizePoints();
    }

    private void _InitializePlayerState()
    {
        _ReactivatePlayer();
        _SetAnimatorState(STATE_ALIVE, true);
        _SetAnimatorState(STATE_ON_THE_GROUND, false);
    }
    private void _InitizalizePoints()
    {
        _healthPoints = INITIAL_HEALTH;
        MaxDistanceScore = 0f;
        GameManager.SharedInstance.RestartCollectableObject();
    }

    private void _ReactivatePlayer()
    {
        this.gameObject.SetActive(true);
    }

    private void _SetAnimatorState(string stateName, bool value)
    {
        playerAnimator.SetBool(stateName, value);
    }

    private void _ResetPlayer()
    {
        _ResetPlayerPosition();
        _ResetPlayerVelocity();
    }
    
    private void _ResetPlayerPosition()
    {
        this.transform.position = _startPosition;
    }

    private void _ResetPlayerVelocity()
    {
        this.playerRigiBody.velocity = Vector2.zero; // le bajo la vel a 0 para que la restablezca la gravedad para evitar el bug de atravesa el collider por la velocidad de muerte
    }


    void Update()
    {
        _HandleJump();
        _HandleMovement();
    }

    private void _HandleJump()
    {
        if (Input.GetButtonDown("Jump") && CheckGameState(GameState.InGame))
        {
            _ActivatePlayerAnimator();
            GetComponent<AudioSource>().Play();
            _Jump(); 
        }
    }

    private void _HandleMovement()
    {
        bool _isGrounded = _IsTouchingTheMask();
        bool _isMovingHorizontally = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f;
       
        if (!_isMovingHorizontally && _isGrounded)
        {
            _SetPlayerIdle();
        }
        else
        {
            _ActivatePlayerAnimator();
        }

        playerAnimator.SetBool(STATE_ON_THE_GROUND, _isGrounded);
    }

    private void _SetPlayerIdle()
    {
        _DesactivatePlayerAnimator();
        playerRigiBody.GetComponent<SpriteRenderer>().sprite = SpriteIdle;
    }

    public bool CheckGameState(GameState targetState)
    {
        return GameManager.SharedInstance.CurrentGameState == targetState;
    }

    private void FixedUpdate()
    {
        _CheckPosibilityOfMovement();
    }

    private void _CheckPosibilityOfMovement()
    {
        if (CheckGameState(GameState.InGame))
        {
            _Move();
        }
    }

    private void _Move()
    {
        float moveSpeed = _CheckTouchRunKey() ? WalkingSpeed * 2 : WalkingSpeed;

        if (_CheckTouchMoveKey() < 0)
        {
            _FlipCharacter(true);
            _MoveHorizontally(-moveSpeed);

        }
        else if(_CheckTouchMoveKey() > 0)
        {
            _FlipCharacter(false);
            _MoveHorizontally(moveSpeed);
        }
        else if(_CheckTouchMoveKey() == 0)
        {
            _MoveHorizontally(0f);
        }

        _AdjustAnimatorSpeed(_CheckTouchRunKey());

    }

    private void _FlipCharacter(bool flip)
    {
        playerRigiBody.GetComponent<SpriteRenderer>().flipX = flip;
    }

    private void _MoveHorizontally(float speed) 
    { 
        playerRigiBody.velocity = new Vector2(speed, playerRigiBody.velocity.y);
    }

    private void _AdjustAnimatorSpeed(bool isRunning)
    {
        float animatorSpeed = isRunning ? 2f: 1f;
        playerAnimator.speed = animatorSpeed;
    }

    private bool _CheckTouchRunKey()
    {
        return Input.GetKey(KeyCode.LeftShift);  
    }

    private float _CheckTouchMoveKey()
    {
        float horizontalInmput = Input.GetAxis("Horizontal");
        return horizontalInmput;
    }
    private bool _CheckTouchJumpKey()
    {
        return Input.GetButtonDown("Jump");
    }

    private void _Jump()
    {
        if (_IsTouchingTheMask()) 
        {
            float jumpForceMultiplier = _CheckTouchRunKey() ? 1.5f : 1.3f;
            _ActivatePlayerAnimator();
            playerRigiBody.AddForce(Vector2.up * (JumpForce * jumpForceMultiplier), ForceMode2D.Impulse);
        }
    }


    private bool _IsTouchingTheMask()
    {
        return Physics2D.Raycast(this.transform.position, Vector2.down, 2f, GroundMask);
    }

    public void Die()
    {
        _SetPlayerAnimatorParametersOnDeath();

        GameManager.SharedInstance.GameOver();
    }

    private void _SetPlayerAnimatorParametersOnDeath()
    {
        _SetAnimatorState(STATE_ALIVE, false);
        _SetAnimatorState(STATE_ON_THE_GROUND, false);
    }

    public float GetTravelledDistance()
    {
        if (CheckGameState(GameState.InGame))// para evitar que tome la distancia en el game over
        {
            float distance = this.transform.position.x - _startPosition.x; // a que distancia me hayo - el punto inicial = total de espacio recorrido en la dimension x
            if (distance > MaxDistanceScore)
            { // solo se incrementa si la distancia actual es mayor que la maxima registrada
                MaxDistanceScore = distance;
            }
        }
        return MaxDistanceScore;
    }

    public void CollectHealth(int points) // deberia ir en collectable
    {
        _ClampHealthPoints();
        this._healthPoints += points;

        if(_healthPoints <= 0) 
        {
            Die();
        }
    }

    private void _ClampHealthPoints() // collectable
    {
        _healthPoints = Mathf.Clamp(_healthPoints, MIN_HEALTH, MAX_HEALTH);
    }

    public void CollectDie() // collectable
    {
        Die();
    }

    public int GetHealth()  // collectable
    {
        return _healthPoints;
    }

    public void CollectPoints(int pointsToAdd) // collectable
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
        }    }


    public void PlayerWin()
    {   
        GameManager.SharedInstance.WinGame(); 
    }

}




