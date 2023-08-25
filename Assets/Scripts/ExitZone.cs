using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitZone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") // si el player entra al collider
        {
            LevelManager.sharedInstance.RemoveLevelBlock(); // tmb tengo q eliminar un bloque asi conservo siempre dos bloques en el presente
        }
    }

}
