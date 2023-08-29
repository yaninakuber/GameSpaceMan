using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    public Text CoinText, ScoreText, MaxScoreText;

    private PlayerController _playerController;

    private const int POINTS_PER_COIN = 50;


    void Start()
    {
        _playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }


    void Update()
    {
        int coins = GameManager.SharedInstance.CollectedObject;

        float distanceScore = _playerController.GetTravelledDistance();
        int coinScore = CalculateValueMultiplication(coins, POINTS_PER_COIN);
        float totalScore = SumFloatValues(distanceScore, coinScore); // sumScore
        float maxScore = GetValueFromPlayerPrefs("maxScore", 0f);


        if (_playerController.CheckGameState(GameState.InGame))
        {
            UpdateInGameUI(coins, totalScore, maxScore);
        }

        if (_playerController.CheckGameState(GameState.GameOver))
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
        if (CoinText != null)
        {
            CoinText.text = coins.ToString(); // resuele bug q indicaba valor nulo en la variabe
        }
        ScoreText.text = FormatScoreText("Score; ", totalScore);
        MaxScoreText.text = FormatScoreText("Max. Score; ", maxScore);
    }

    void UpdateGameOverUI(float totalScore, float maxScore)
    {
        if (totalScore > maxScore)
        {
            maxScore = totalScore;
            PlayerPrefs.SetFloat("maxScore", maxScore); // Almacenamos el nuevo maxScore en PlayerPrefs
        }


        ScoreText.text = FormatScoreText("Your Score; ", totalScore);
        MaxScoreText.text = FormatScoreText("Max. Score; ", maxScore);
    }

    string FormatScoreText(string tittle, float score)
    {
        return tittle + ConvertFloatToString(score);
    }

    string ConvertFloatToString(float number)//helper de C# otra clase
    {
        return number.ToString("f1");
    }

}


