using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // declaro variable que siga al objetivo
    public Vector3 offset = new Vector3(-3f, 0.0f, -10f); // a que distancia debe seguir al personaje
    public float dampingTime = 0.3f; // tiempo de amortiguacion para que no sea un choque muy bruzco, la cam tardara un poco en seguir, efecto cine 
    public Vector3 velocityCam = Vector3.zero; //velocidad que va la camara, inicializamos con cero

    private bool isResettingCamera = false; // Variable para controlar si la cámara se está reiniciando
    private bool isFirstMove = true; // arregla bugg de movimiento cortado la primera vez


    void Awake()
    {
        Application.targetFrameRate = 60;  // va a intentar ir a 60 frame x seg
        MoveCamera(true); // inicializa el movimiento suave

    }


    void Start()
    {

    }


    void Update()
    {
        if (GameManager.sharedInstance.currentGameState == GameState.inGame || GameManager.sharedInstance.currentGameState == GameState.menu)
        {
            if (isFirstMove) {
                isFirstMove = false;
                MoveCamera(true);
            } else {
                MoveCamera(true);
                isResettingCamera = false;
            }
        } else if(GameManager.sharedInstance.currentGameState == GameState.gameOver && !isResettingCamera || GameManager.sharedInstance.currentGameState == GameState.win && !isResettingCamera) {
            Invoke("ResetCameraPosition", 0.5f); // Llamar al método ResetCameraPosition con un retraso de 2 segundos;
            isResettingCamera = true;
        }
    }

    public void MoveCamera(bool smooth)
    {
        Vector3 destinationInGame = new Vector3(Mathf.Max(target.position.x - offset.x, this.transform.position.x), offset.y, offset.x); // destino que restringe mov de cam a la izquierda
        Vector3 destinationReset = new Vector3(offset.x, offset.y, offset.z); // destino que deja quieta la camara

        if (smooth) {
            this.transform.position = Vector3.SmoothDamp( // lo sigue lentamente 
                            this.transform.position,
                            destinationInGame,
                            ref velocityCam,
                            dampingTime);
        }
       else
       {
           this.transform.position = destinationReset;
       }

    }

    public void ResetCameraPosition() {
        MoveCamera(false);
    }
}