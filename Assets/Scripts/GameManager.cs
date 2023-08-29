using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Menu, 
    InGame,
    GameOver,
    Win
}

public class GameManager : MonoBehaviour
{
    public GameState CurrentGameState = GameState.Menu;
    public static GameManager SharedInstance; 

    private PlayerController _playerController; 

    public int CollectedObject = 0;


    private void Awake()
    {
        SharedInstance = this;
    }

    private void Start()
    {
        _playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Submit") && CurrentGameState != GameState.InGame)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        _SetGameState(GameState.InGame);
    }

    public void BackToMenu()
    {
        _SetGameState(GameState.Menu);
    }

    public void GameOver()
    {
        _SetGameState(GameState.GameOver);
    }

    public void WinGame()
    {
        _SetGameState(GameState.Win);
    }


    private void _SetGameState(GameState newGameState)
    {
        _HideAllMenues();
        ActivateMenuGameStateCases(newGameState);

        this.CurrentGameState = newGameState;
    }

    private void _HideAllMenues()
    {
        MenuManager.SharedInstance.HideGameCanvas();
        MenuManager.SharedInstance.HideMainMenu();
        MenuManager.SharedInstance.HideMenuGameOver();
        MenuManager.SharedInstance.HideGameWinCanvas();
    }

    void ActivateMenuGameStateCases(GameState newGameState)
    {
        switch (newGameState)
        {
            case GameState.Menu:
                MenuManager.SharedInstance.ShowMainMenu();
                break;
            case GameState.InGame:
                HandleInGame();
                break;
            case GameState.GameOver:
                MenuManager.SharedInstance.ShowMenuGameOver();
                break;
            case GameState.Win:
                MenuManager.SharedInstance.ShowGameWinCanvas();
                break;
        }

    }

    void HandleInGame()
    {
        LevelManager.SharedInstance.RemoveAllLevelBlock();
        LevelManager.SharedInstance.GenerateInitialBlock();
        LevelManager.SharedInstance.GenerateBlocks();
        _playerController.StartGame();

        MenuManager.SharedInstance.ShowGameCanvas();

    }

    public void CollectableObject(Collectable collectable)
    {
        CollectedObject += collectable.ValueCoin;
    }

    public void RestartGame()
    {
        _SetGameState(GameState.InGame);
    }

    public void RestartCollectableObject()
    {
        CollectedObject = 0;
    }

}