using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatcherDetection : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") )
        {
            PlayerStats PS = collision.GetComponent<PlayerStats>();
            PS.TakeDamage(1000);
        }
        
    }
}
