using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{  // Enumerado, al declararla fuera de la clase puedo acceder desde otro script //3 posibles estados del juego
    menu,
    inGame,
    gameOver,
    win
}

public class GameManager : MonoBehaviour
{
    public GameState currentGameState = GameState.menu; //inicializar declarando la variable del tipo GameState y seleccionado como comenzaria. 
    public static GameManager sharedInstance; //variable estatica que lo declara como singleton 
    private PlayerController controller; //variable el managger pueda acceder al personaje

    public int collectedObject = 0;


    private void Awake()
    {
        sharedInstance = this; //llamo al singleton y lo asigno como unico
    }


    void Start()
    {
        controller = GameObject.Find("Player").GetComponent<PlayerController>(); //Al comenzar localizamos el player. como eso devuelve un objeto debo acceder al componenete player controller

    }


    void Update()
    {
        if (Input.GetButtonDown("Submit") && currentGameState != GameState.inGame) // ni bien apretamos enter pasamos del menu al ocmienzo del juego. Para q no pueda reiniciar en cualquier momento
        {
            StartGame();
        }
    }


    public void StartGame()
    {
        SetGameState(GameState.inGame); //el unico que se encarga de modificar es el SetGameState por lo que lo llamo y le doy como parametro lo que quiero hacer
    }

    public void GameOver()
    {
        SetGameState(GameState.gameOver);
    }

    public void BackToMenu()
    {
        SetGameState(GameState.menu);
    }

    public void WinGame()
    {
        SetGameState(GameState.win);
    }

    void SetGameState(GameState newGameState) // encargado de realizar cambios de estados y modif. ese nuevo estado se llamara newGameState
    {
        if (newGameState == GameState.menu)
        {
            MenuManager.sharedInstance.ShowMainMenu(); // llamo a mostrar el menu
            MenuManager.sharedInstance.HideMenuGameOver();
            MenuManager.sharedInstance.HideGameCanvas();
            MenuManager.sharedInstance.HideGameWinCanvas();
        }
        else if (newGameState == GameState.inGame)
        {
            LevelManager.sharedInstance.RemoveAllLevelBlock();// me aseguro que se borren tod los bloques anteriores
            LevelManager.sharedInstance.GenerateInitialBlock(); // genera escena 
            LevelManager.sharedInstance.GenerationBlocks();
            controller.StartGame(); // cuando comienza la partida tengo q llamar al startGame del player Controller para que prepare tod.

            MenuManager.sharedInstance.HideMenuGameOver();
            MenuManager.sharedInstance.HideMainMenu(); // llamo a ocultar el menu cuando estoy in game
            MenuManager.sharedInstance.HideGameWinCanvas();
            MenuManager.sharedInstance.ShowGameCanvas();
        }
        else if (newGameState == GameState.gameOver)
        {
            MenuManager.sharedInstance.HideGameCanvas();
            MenuManager.sharedInstance.HideMainMenu();
            MenuManager.sharedInstance.HideGameWinCanvas();
            MenuManager.sharedInstance.ShowMenuGameOver();

        }
        else if (newGameState == GameState.win)
        {
            MenuManager.sharedInstance.HideGameCanvas();
            MenuManager.sharedInstance.HideMainMenu();
            MenuManager.sharedInstance.HideMenuGameOver();
            MenuManager.sharedInstance.ShowGameWinCanvas();
        }

        this.currentGameState = newGameState; //Actualiza el estado actual 
    }


    public void CollectableObject(Collectable collectable)
    {
        collectedObject += collectable.value; //agarra la variable que inicializamos en 0 y le suma el valor del objeto que recolectamos

    }

    public void RestartGame()
    {
        SetGameState(GameState.inGame);
    }
}