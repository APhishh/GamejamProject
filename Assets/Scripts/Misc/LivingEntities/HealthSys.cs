using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSys : MonoBehaviour
{
   [SerializeField] private float health;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(float dmg)
    {
        health-=dmg;

        if (health <= 0)
        {
            Death();
        }

    }

    public void Death()
    {
        Destroy(gameObject);
    }

}
