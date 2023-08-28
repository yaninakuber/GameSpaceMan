using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 
    public Vector3 offset = new Vector3(-3f, 0.0f, -10f); // a que distancia debe seguir al personaje
    public float dampingTime = 0.3f; // tiempo de amortiguacion para que no sea un choque muy bruzco, la cam tardara un poco en seguir, efecto cine 
    public Vector3 velocityCam = Vector3.zero; 

    private bool isResettingCamera = false; 
    private bool isFirstMove = true; // arregla bugg de movimiento cortado la primera vez


    void Awake()
    {
        Application.targetFrameRate = 60;  // va a intentar ir a 60 frame x seg
        MoveCamera(true); // inicializa el movimiento suave
    }

    private void Update()
    {
        GameState gameState = GameManager.sharedInstance.currentGameState;

        if(IsInGameOrMenuState(gameState))
        {
            HandleInGameOrMenuState();
        } 
        else if (IsGameOverOrWinState(gameState)) 
        {
            HandleGameOverOrWinState();
        }
    }

    private bool IsInGameOrMenuState(GameState state)
    {
        return state == GameState.inGame || state == GameState.menu;
    }
    private bool IsGameOverOrWinState(GameState state)
    {
        return state == GameState.gameOver || state == GameState.win;
    }

    void HandleInGameOrMenuState()
    {
        if (isFirstMove)
        {
            isFirstMove = false;
            MoveCamera(true);
        }
        else
        {
            MoveCamera(true);
            isResettingCamera = false;
        }
    }

    void HandleGameOverOrWinState()
    {
        if (!isResettingCamera)
        {
            Invoke("ResetCameraPosition", 0.5f);
            isResettingCamera = true;
        }
    }

    public void SmoothMoveCamera(bool smooth)
    {
        Vector3 destination = smooth ? CalculateSmoothDestination() : CalculateResetDestination();
        this.transform.position = destination;
    }

    Vector3 CalculateSmoothDestination()
    {
        Vector3 targetPosition = target.position;
        Vector3 restrictedPosition = new Vector3(Mathf.Max(targetPosition.x - offset.x, this.transform.position.x), offset.y, offset.z); // destino que restringe mov de cam a la izquierda
        return Vector3.SmoothDamp(this.transform.position, restrictedPosition, ref velocityCam, dampingTime); // lo sigue lentamente 
    }

    private void ResetCameraPosition()
    {
        MoveCamera(false);
    }

    private void MoveCamera(bool shouldSmooth)
    {
        Vector3 destination = shouldSmooth ? CalculateSmoothDestination() : CalculateResetDestination();
        this.transform.position = destination;
    }

    private Vector3 CalculateResetDestination()
    {
        return new Vector3(offset.x, offset.y, offset.z);
    }
}