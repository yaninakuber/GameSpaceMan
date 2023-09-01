using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerBar : MonoBehaviour
{
    private Slider _slider;
    public PlayerController PlayerController;

    void Start()
    {
        _slider = GetComponent<Slider>(); 
        _slider.maxValue = PlayerController.MAX_HEALTH; 
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        _slider.value = PlayerController.GetHealth(); 
    }

}
