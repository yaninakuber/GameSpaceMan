using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectableType
{
    HealthPotion,
    DiedPotion,
    Money
}

public class Collectable : MonoBehaviour
{
    public CollectableType TypeCollectable = CollectableType.Money;
    public int ValueCoin = 1; 

    private SpriteRenderer _sprite; 
    private CircleCollider2D _itemCollider; 

    private const int POINTS_PER_COINS = 50;

    GameObject player;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _itemCollider = GetComponent<CircleCollider2D>(); 
    }

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    private void HideCollectable() 
    {
        _sprite.enabled = false;
        _itemCollider.enabled = false;
    }

    private void CollectMoney()
    {
        GameManager.SharedInstance.CollectableObject(this); //deberia estar aca la funcion
        GetComponent<AudioSource>().Play();
        player.GetComponent<PlayerController>().CollectPoints(POINTS_PER_COINS);
    }

    private void CollectHealthPotion()
    {
        player.GetComponent<PlayerController>().CollectHealth(this.ValueCoin); //deberia estar aca
    }

    private void CollectDiedPotion()
    {
        player.GetComponent<PlayerController>().CollectDie();
    }

    private void Collect()
    {
        HideCollectable();

        switch (TypeCollectable) //cambiar TypeCollectable
        {
            case CollectableType.Money:
                CollectMoney();
                break;
            case CollectableType.HealthPotion:
                CollectHealthPotion();
                break;
            case CollectableType.DiedPotion:
                CollectDiedPotion();
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Collect();
        }
    }
}

