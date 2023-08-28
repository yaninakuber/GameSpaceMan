using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    public Text coinText, scoreText, maxScoreText;

    private PlayerController playerController;

    private const int POINTS_PER_COIN = 50;


    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        
    }


    void Update()
    {
        int coins = GameManager.sharedInstance.collectedObject;

        float distanceScore = playerController.GetTravelledDistance();
        int coinScore = CalculateValueMultiplication(coins, POINTS_PER_COIN);
        float totalScore = SumFloatValues(distanceScore, coinScore);
        float maxScore = GetValueFromPlayerPrefs("maxScore", 0f);


        if (playerController.CheckGameState(GameState.inGame))
        {
            UpdateInGameUI(coins, totalScore, maxScore);
        }

        if (playerController.CheckGameState(GameState.gameOver))
        {
            UpdateGameOverUI(totalScore, maxScore);
        }
    }

    int CalculateValueMultiplication(int values, int multiplier)
    {
        return values * multiplier;
    }

    float SumFloatValues(params float[] scores)
    {
        float total = 0;
        foreach (float score in scores)
        {
            total += score;
        }
        return total;

    }

    float GetValueFromPlayerPrefs(string key, float defaultValue)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    void UpdateInGameUI(int coins, float totalScore, float maxScore)
    {
        if (coinText != null)
        {
            coinText.text = coins.ToString(); // resuele bug q indicaba valor nulo en la variabe
        }
        scoreText.text = FormatScoreText("Score; ", totalScore);
        maxScoreText.text = FormatScoreText("Max. Score; ", maxScore);
    }

    void UpdateGameOverUI(float totalScore, float maxScore)
    {
        if (totalScore > maxScore)
        {
            maxScore = totalScore;
            PlayerPrefs.SetFloat("maxScore", maxScore); // Almacenamos el nuevo maxScore en PlayerPrefs
        }


        scoreText.text = FormatScoreText("Your Score; ", totalScore);
        maxScoreText.text = FormatScoreText("Max. Score; ", maxScore);
    }

    string FormatScoreText(string tittle, float score)
    {
        return tittle + ConvertFloatToString(score);
    }

    string ConvertFloatToString(float number)
    {
        return number.ToString("f1");
    }



}


