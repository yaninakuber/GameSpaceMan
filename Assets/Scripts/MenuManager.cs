using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public static MenuManager sharedInstance;
    public Canvas menuCanvas; // Este canvas me permite gestionar tod el canvas donde se gestionan tod los objetos.
    public Canvas gameCanvas;
    public Canvas gameOverCanvas;
    public Canvas gameWinCanvas;


    private void Awake()
    {
        if(sharedInstance == null)
        {
            sharedInstance = this;
        }
    }

    public void ShowMainMenu() // mostrar el menu principal
    {
        menuCanvas.enabled = true; // activa el menu canvas
    }

    public void HideMainMenu() // se encarga de ocultar el menu canva
    {
        menuCanvas.enabled = false;
    }

    public void ShowMenuGameOver()
    {
        gameOverCanvas.enabled = true;
        GetComponent<AudioSource>().Play();
    }

    public void HideMenuGameOver()
    {
        gameOverCanvas.enabled = false;
    }

    public void ShowGameCanvas()
    {
        gameCanvas.enabled = true;
    }

    public void HideGameCanvas()
    {
        gameCanvas.enabled = false;
    }

    public void ShowGameWinCanvas()
    {
        gameWinCanvas.enabled = true;
    }

    public void HideGameWinCanvas()
    {
        gameWinCanvas.enabled = false;
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR // al agregar # informo que son metod que dependen de la plataforma
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

}
