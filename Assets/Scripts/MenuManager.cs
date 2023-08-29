using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public static MenuManager SharedInstance;
    public Canvas MenuCanvas; 
    public Canvas GameCanvas;
    public Canvas GameOverCanvas;
    public Canvas GameWinCanvas;


    private void Awake()
    {
        if(SharedInstance == null)
        {
            SharedInstance = this;
        }
    }

    public void ShowMainMenu() // mostrar el Menu principal
    {
        MenuCanvas.enabled = true; // activa el Menu canvas
    }

    public void HideMainMenu() // se encarga de ocultar el Menu canva
    {
        MenuCanvas.enabled = false;
    }

    public void ShowMenuGameOver()
    {
        GameOverCanvas.enabled = true;
        GetComponent<AudioSource>().Play();
    }

    public void HideMenuGameOver()
    {
        GameOverCanvas.enabled = false;
    }

    public void ShowGameCanvas()
    {
        GameCanvas.enabled = true;
    }

    public void HideGameCanvas()
    {
        GameCanvas.enabled = false;
    }

    public void ShowGameWinCanvas()
    {
        GameWinCanvas.enabled = true;
    }

    public void HideGameWinCanvas()
    {
        GameWinCanvas.enabled = false;
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
