using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    menu,
    inGame,
    gameOver,
    win
}

public class GameManager : MonoBehaviour
{
    public GameState currentGameState = GameState.menu;
    public static GameManager sharedInstance;

    private PlayerController playerController;

    public int collectedObject = 0;


    private void Awake()
    {
        sharedInstance = this;
    }


    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }


    void Update()
    {
        if (Input.GetButtonDown("Submit") && currentGameState != GameState.inGame)
        {
            StartGame();
        }
    }


    public void StartGame()
    {
        SetGameState(GameState.inGame);
    }

    public void BackToMenu()
    {
        SetGameState(GameState.menu);
    }

    public void GameOver()
    {
        SetGameState(GameState.gameOver);
    }

    public void WinGame()
    {
        SetGameState(GameState.win);
    }


    void SetGameState(GameState newGameState)
    {
        HideAllMenues();
        ActivateMenuGameStateCases(newGameState);

        this.currentGameState = newGameState;

    }

    void HideAllMenues()
    {
        MenuManager.sharedInstance.HideGameCanvas();
        MenuManager.sharedInstance.HideMainMenu();
        MenuManager.sharedInstance.HideMenuGameOver();
        MenuManager.sharedInstance.HideGameWinCanvas();
    }

    void ActivateMenuGameStateCases(GameState newGameState)
    {
        switch (newGameState)
        {
            case GameState.menu:
                MenuManager.sharedInstance.ShowMainMenu();
                break;
            case GameState.inGame:
                HandleInGame();
                break;
            case GameState.gameOver:
                MenuManager.sharedInstance.ShowMenuGameOver();
                break;
            case GameState.win:
                MenuManager.sharedInstance.ShowGameWinCanvas();
                break;
        }

    }

    void HandleInGame()
    {
        LevelManager.sharedInstance.RemoveAllLevelBlock();
        LevelManager.sharedInstance.GenerateInitialBlock();
        LevelManager.sharedInstance.GenerationBlocks();
        playerController.StartGame();

        MenuManager.sharedInstance.ShowGameCanvas();

    }

    public void CollectableObject(Collectable collectable)
    {
        collectedObject += collectable.value;
    }

    public void RestartGame()
    {
        SetGameState(GameState.inGame);
    }

    public void RestartCollectableObject()
    {
        collectedObject = 0;
    }

}