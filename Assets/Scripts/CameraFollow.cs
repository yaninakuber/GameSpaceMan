using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target; 
    public Vector3 Offset = new Vector3(-3f, 0.0f, -10f); // a que distancia debe seguir al personaje
    public float DampingTime = 0.3f; // tiempo de amortiguacion para que no sea un choque muy bruzco, la cam tardara un poco en seguir, efecto cine 
    public Vector3 VelocityCam = Vector3.zero; 

    private bool _isResettingCamera = false; 
    private bool _isFirstMove = true; // arregla bugg de movimiento cortado la primera vez


    void Awake()
    {
        Application.targetFrameRate = 60;  // va a intentar ir a 60 frame x seg
        _MoveCamera(true); // inicializa el movimiento suave
    }

    private void Update()
    {
        GameState gameState = GameManager.SharedInstance.CurrentGameState;

        if(_IsInGameOrMenuState(gameState))
        {
            _HandleInGameOrMenuState();
        } 
        else if (IsGameOverOrWinState(gameState)) 
        {
            _HandleGameOverOrWinState();
        }
    }

    private bool _IsInGameOrMenuState(GameState state) => state == GameState.InGame || state == GameState.Menu;

    private bool IsGameOverOrWinState(GameState state) => state == GameState.GameOver || state == GameState.Win; 
    

    private void _HandleInGameOrMenuState()
    {
        if (_isFirstMove)
        {
            _isFirstMove = false;
            _MoveCamera(true);
        }
        else
        {
            _MoveCamera(true);
            _isResettingCamera = false;
        }
    }

    private void _HandleGameOverOrWinState() //dos diferentes estados 
    {
        if (!_isResettingCamera)
        {
            Invoke("_ResetCameraPosition", 0.5f);
            _isResettingCamera = true;
        }
    }

    public void SmoothMoveCamera(bool smooth)
    {
        Vector3 destination = smooth ? _CalculateSmoothDestination() : _CalculateResetDestination();
        this.transform.position = destination;
    }

    private Vector3 _CalculateSmoothDestination()
    {
        Vector3 _targetPosition = Target.position;
        Vector3 _restrictedPosition = new Vector3(Mathf.Max(_targetPosition.x - Offset.x, this.transform.position.x), Offset.y, Offset.z);
        // destino que restringe mov de cam a la izquierda
        // dificil de leer
        return Vector3.SmoothDamp(this.transform.position, _restrictedPosition, ref VelocityCam, DampingTime); // lo sigue lentamente 
    }

    private void _ResetCameraPosition()
    {
        _MoveCamera(false); // un metodo hace dos cosas
    }

    private void _MoveCamera(bool shouldSmooth)
    {
        Vector3 destination = shouldSmooth ? _CalculateSmoothDestination() : _CalculateResetDestination();
        this.transform.position = destination;
    }

    private Vector3 _CalculateResetDestination()
    {
        return new Vector3(Offset.x, Offset.y, Offset.z);
    }
}