using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerBar : MonoBehaviour
{
    private Slider slider;
    public PlayerController playerController;

    void Start()
    {
        slider = GetComponent<Slider>(); 
        slider.maxValue = PlayerController.MAX_HEALTH; 
    }

    void Update()
    {
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        slider.value = playerController.GetHealth(); 
    }

}
