using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectableType
{
    healthPotion,
    diedPotion,
    money
}

public class Collectable : MonoBehaviour
{

    public CollectableType type = CollectableType.money;

    private SpriteRenderer sprite; //accede a la imagen visual
    private CircleCollider2D itemCollider; //para acceder directamente al collider del objeto

    bool hasBeenCollected = false; // para saber si ya fue recolecta

    public int value = 1; // indicar el valor del objeto

    private const int POINTS_PER_COINS = 50;// Establece el valor de puntos por moneda

    GameObject player; //inicializamos una variable player para posiones

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        itemCollider = GetComponent<CircleCollider2D>(); // configuramos las variables desde el inicio, permite acceder y manejar las propiedades y el comportamiento
    }

    private void Start()
    {
        player = GameObject.Find("Player"); // busco el player 
    }

    void Show()
    {
        sprite.enabled = true;
        itemCollider.enabled = true;
        hasBeenCollected = false;
    }

    void Hide()
    {
        sprite.enabled = false;
        itemCollider.enabled = false;
    }

    void Collect()
    {
        Hide();
        hasBeenCollected = true;

        switch (this.type)
        {
            case CollectableType.money:
                GameManager.sharedInstance.CollectableObject(this);
                GetComponent<AudioSource>().Play();
                break;
            case CollectableType.healthPotion:
                player.GetComponent<PlayerController>().CollectHealth(this.value); // una vez localizado puedo entrar en su componente del scrpt y buscar el metod collect health indicando el valor
                break;
            case CollectableType.diedPotion:
                player.GetComponent<PlayerController>().CollectDie(); 
                break;
        }
        if (this.type == CollectableType.money)
        {
            player.GetComponent<PlayerController>().CollectPoints(POINTS_PER_COINS); // Suma puntos por moneda recolectada
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Collect();
        }
    }

}

