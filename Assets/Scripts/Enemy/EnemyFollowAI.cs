using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyFollowAI : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody2D enemyRB;
    [SerializeField] private string state;
    [SerializeField] private float walkspeed;
    [SerializeField] GameObject hitbox;
    [SerializeField] float attCooldown;
    [SerializeField] private bool canAttack = true;
    [SerializeField] private String Direction = "Left";
    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            state = "Wander";
        }
        else
        {
             if (Vector2.Distance(transform.position, player.position) <= 6)
            {
                state = "Detected";
            }
        }
       

        if (state == "Detected")
        {
            Vector3 dir = (player.position - transform.position).normalized;
            enemyRB.velocity = new Vector2(dir.normalized.x * walkspeed, 0);

            if(Vector2.Distance(transform.position, player.position) <= 2 && canAttack)
            {
                Attack();
            }

        }

    }

    private void Attack()
    {
                Vector3 dir = (player.position - transform.position).normalized;
                canAttack = false;
                float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
                GameObject attHitbox = Instantiate(hitbox,transform.position ,Quaternion.Euler(0,0,angle));
                HitboxFollowCharacter followScript = attHitbox.GetComponent<HitboxFollowCharacter>();
                CollisionDetection colScript = attHitbox.GetComponent<CollisionDetection>();
                colScript.setAttacker(gameObject);  
                followScript.Set(transform,dir);
                StartCoroutine(Despawn(attHitbox));
                StartCoroutine(AttackCooldown());
    }
    IEnumerator Despawn(GameObject obj)
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(obj);
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attCooldown);
        canAttack = true;
    }
}
