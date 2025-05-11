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
    [SerializeField] private LayerMask groundLayer;
     Ray ray;
    RaycastHit2D hit;
    
  

    // Update is called once per frame
    void Update()
    {

        if(Direction == "Left")
        {
            ray = new Ray(transform.position - new Vector3(1,0,0),Vector2.down*5);
            enemyRB.velocity = new Vector2(-walkspeed, enemyRB.velocity.y);
        }
        else if(Direction == "Right")
        {
            ray = new Ray(transform.position + new Vector3(0.75f,0,0),Vector2.down*5);
            enemyRB.velocity = new Vector2(walkspeed, enemyRB.velocity.y);
        }

        Debug.DrawRay(ray.origin, ray.direction * 5, Color.red);

        hit = Physics2D.Raycast(ray.origin, ray.direction, 5f,groundLayer);

        
        if (hit.collider == null  || !hit.collider.CompareTag("Ground"))
        {
            Debug.Log("yea");
            if(Direction == "Left")
            {
                Direction = "Right";
            }
            else if(Direction == "Right")
            {
                Direction = "Left";
            }
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
            enemyRB.velocity = new Vector2(dir.normalized.x * walkspeed, enemyRB.velocity.y);

            if(Vector2.Distance(transform.position, player.position) <= 2 && canAttack)
            {
                Attack();
            }

        }

        if (state == "Wander")
        {

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
