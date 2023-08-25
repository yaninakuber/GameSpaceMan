using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    public Text coinText, scoreText, maxScoreText;

    private PlayerController controller;

    private const int POINTS_PER_COIN = 50; // Valor de puntos por moneda


    void Start()
    {
        controller = GameObject.Find("Player").GetComponent<PlayerController>();
    }


    void Update() 
    {
        if(GameManager.sharedInstance.currentGameState == GameState.inGame)//si estoy jugando quiero actualizar tod el tiempo el puntaje
        {
            int coins = GameManager.sharedInstance.collectedObject; 
            float distanceScore = controller.GetTravelledDistance(); // se los voy a pasar de acuerdo a la distancia recorrida por eso float
            int coinScore = coins * POINTS_PER_COIN; // Puntos por monedas recolectadas
            float totalScore = distanceScore + coinScore; // Total de puntos

            float maxScore = PlayerPrefs.GetFloat("maxScore", 0f);

            if (coinText != null)
            {
                coinText.text = coins.ToString(); // resuele bug q indicaba valor nulo en la variabe
            }
            scoreText.text = "Score: " + totalScore.ToString("f1");
            maxScoreText.text = "Max. Score: " + maxScore.ToString("f1"); // con un decimal

        }

        if(GameManager.sharedInstance.currentGameState == GameState.gameOver)
        {
            float distanceScore = controller.GetTravelledDistance(); // Puntos por distancia
            int coinScore = GameManager.sharedInstance.collectedObject * POINTS_PER_COIN; // Puntos por monedas recolectadas
            float totalScore = distanceScore + coinScore; // Total de puntos
            float maxScore = PlayerPrefs.GetFloat("maxScore", 0f);

            scoreText.text = "Your Score: " + totalScore.ToString("f1");
            maxScoreText.text = "Max. Score: " + maxScore.ToString("f1"); // con un decimal
        }
    }
}
