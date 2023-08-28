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
    public int value = 1;

    private SpriteRenderer sprite; 
    private CircleCollider2D itemCollider; 

    bool hasBeenCollected = false; 

    private const int POINTS_PER_COINS = 50;

    GameObject player;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        itemCollider = GetComponent<CircleCollider2D>(); 
    }

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    void Hide()
    {
        sprite.enabled = false;
        itemCollider.enabled = false;
    }

    void CollectMoney()
    {
        GameManager.sharedInstance.CollectableObject(this);
        GetComponent<AudioSource>().Play();
        player.GetComponent<PlayerController>().CollectPoints(POINTS_PER_COINS);
    }

    void CollectHealthPotion()
    {
        player.GetComponent<PlayerController>().CollectHealth(this.value);
    }

    void CollectDiedPotion()
    {
        player.GetComponent<PlayerController>().CollectDie();
    }

    void Collect()
    {
        Hide();

        switch (type)
        {
            case CollectableType.money:
                CollectMoney();
                break;
            case CollectableType.healthPotion:
                CollectHealthPotion();
                break;
            case CollectableType.diedPotion:
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

