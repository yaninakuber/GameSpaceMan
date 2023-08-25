using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public enum BarType
{
    healthBar,
    manaBar
}



public class PlayerBar : MonoBehaviour
{

    private Slider slider; // referenciar al slider, privada por que lo coloco dentro del start
    public BarType type; // para acceder al tipo de barra en lo grafico en el desplegable 



    void Start()
    {
        slider = GetComponent<Slider>(); // accedo al script del tipo slider

        switch (type) // Cuando arranca se cual es el valor de cada barra
        {
            case BarType.healthBar:
                slider.maxValue = PlayerController.MAX_HEALTH; //Al querer acceder a una constante no necesito instanciar al player con el find, simplemente puedo acceder
                break;
            case BarType.manaBar:
                slider.maxValue = PlayerController.MAX_HEALTH;
                break;
        }
    }

    void Update()
    {
        //consultar la vida y el mana del usuario

        switch (type)
        {
            case BarType.healthBar:
                slider.value = GameObject.Find("Player").GetComponent<PlayerController>().GetHealth(); // Aqui si necesito instanciar para llegar al elem por no ser un singleton ni const
                break;
            case BarType.manaBar:
                slider.value = GameObject.Find("Player").GetComponent<PlayerController>().GetHealth(); // me devuelve el valor actualizado constantemente y se lo entrego al valor de la barra
                break; 
        }

    }
}
