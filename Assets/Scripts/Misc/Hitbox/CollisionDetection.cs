using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Hitable"))
        {
            Debug.Log("NIGGA");
        }
    }
}
