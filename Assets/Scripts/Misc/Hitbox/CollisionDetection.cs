using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    private AttackHitboxProperties AHP;
    [SerializeField] private GameObject attacker;
    [SerializeField] GameObject hitParticle;
    private float damage;

    void Start()
    {
        AHP = gameObject.GetComponent<AttackHitboxProperties>();
        if (AHP != null)
        {
            damage = AHP.getDamage();
        }
        else
        {
            Debug.Log("Unable to get damage, AHP is NULL");
        }
    }

    public void setAttacker(GameObject attacker)
    {
        this.attacker = attacker;
    } 

    void OnTriggerEnter2D(Collider2D collision)
    {
        if((collision.CompareTag("Player") ||collision.CompareTag("Enemy")) && collision.gameObject != attacker)
        {
            if(collision.CompareTag("Player"))
            {
                 PlayerStats targetHealthSys = collision.GetComponent<PlayerStats>();
                 targetHealthSys.TakeDamage(damage,attacker);
            }
            else if(collision.CompareTag("Enemy"))
            {
                HealthSys targetHealthSys = collision.GetComponent<HealthSys>();
                targetHealthSys.Damage(damage);
            }
           
            Vector2 dir = (collision.transform.position - transform.position).normalized;
            float Angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
            Vector3 direction3D = new Vector3(transform.right.x, transform.right.y, 0f);
            Debug.Log(direction3D);
            GameObject hitPart = Instantiate(hitParticle, collision.transform.position, Quaternion.LookRotation(direction3D, Vector3.back));
            Debug.DrawRay(hitPart.transform.position, hitPart.transform.right * 2, Color.red, 1f);

            Destroy(hitPart,1f);
        }
    }
}
