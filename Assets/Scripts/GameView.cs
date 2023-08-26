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
        int coinScore = CalculateScore(coins, POINTS_PER_COIN);
        float totalScore = CalculateTotalScore(distanceScore, coinScore);
        float maxScore = GetMaxScore();


        if (playerController.IsInGame())
        {
            if (coinText != null)
            {
                coinText.text = coins.ToString(); // resuele bug q indicaba valor nulo en la variabe
            }
            scoreText.text = AssignScoreText("Score; ", totalScore); 
            maxScoreText.text = AssignScoreText("Max. Score; ", maxScore);

        }

        if (GameManager.sharedInstance.currentGameState == GameState.gameOver)
        {
            if (totalScore > maxScore)
            {
                maxScore = totalScore;
                PlayerPrefs.SetFloat("maxScore", maxScore); // Almacenamos el nuevo maxScore en PlayerPrefs
            }


            scoreText.text = AssignScoreText("Your Score; ", totalScore);
            maxScoreText.text = AssignScoreText("Max. Score; ", maxScore);
        }


    }

    int CalculateScore(int values, int pointPerUnit)
    {
        return values * pointPerUnit;
    }

    float CalculateTotalScore(float distanceScore, int coinScore)
    {
        return distanceScore + coinScore;
    }

    float GetMaxScore()
    {
        return PlayerPrefs.GetFloat("maxScore", 0f);
    }

    string AssignScoreText(string texto, float score)
    {
        return texto + ConvertFloatToString(score);
    }

    string ConvertFloatToString(float number)
    {
        return number.ToString("f1");
    }

}


