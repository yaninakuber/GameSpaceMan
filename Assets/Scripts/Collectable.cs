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

    bool hasBeenCollected = false; // no se usa  

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

    private void _HideCollectable() 
    {
        _sprite.enabled = false;
        _itemCollider.enabled = false;
    }

    private void _CollectMoney()
    {
        GameManager.SharedInstance.CollectableObject(this); //deberia estar aca la funcion
        GetComponent<AudioSource>().Play();
        player.GetComponent<PlayerController>().CollectPoints(POINTS_PER_COINS);
    }

    private void _CollectHealthPotion()
    {
        player.GetComponent<PlayerController>().CollectHealth(this.ValueCoin); //deberia estar aca
    }

    private void _CollectDiedPotion()
    {
        player.GetComponent<PlayerController>().CollectDie();
    }

    private void _Collect()
    {
        _HideCollectable();

        switch (TypeCollectable) //cambiar TypeCollectable
        {
            case CollectableType.Money:
                _CollectMoney();
                break;
            case CollectableType.HealthPotion:
                _CollectHealthPotion();
                break;
            case CollectableType.DiedPotion:
                _CollectDiedPotion();
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _Collect();
        }
    }
}

