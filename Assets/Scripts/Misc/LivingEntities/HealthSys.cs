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
        health -= dmg;

        // Notify other components that this entity took damage
        SendMessage("OnDamageTaken", dmg, SendMessageOptions.DontRequireReceiver);

        if (health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        // Notify other components that this entity has died
        SendMessage("OnDeath", SendMessageOptions.DontRequireReceiver);

        // Destroy the GameObject
        Destroy(gameObject);
    }
}
