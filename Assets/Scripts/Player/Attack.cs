using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Attack : MonoBehaviour
{

    [SerializeField] GameObject hitbox;
    [SerializeField] GameObject hitParticle;
    [SerializeField] Animator animator;
    [SerializeField]private bool attacking;
    [SerializeField] private float swingDelay;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && attacking == false)
        {
            attacking = true;
            animator.SetBool("Attacking", attacking);
            StartCoroutine(Attacking());
        }
        animator.SetBool("Attacking", attacking);
    }

    IEnumerator Attacking()
    {
            yield return new WaitForSeconds(0.3f);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            Vector3 dir = (mousePos - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
            GameObject newObj = Instantiate(hitbox, transform.position + new Vector3(dir.x*1,dir.y*1), Quaternion.Euler(0, 0, angle));
            HitboxFollowCharacter followScript = newObj.GetComponent<HitboxFollowCharacter>();
            CollisionDetection colScript = newObj.GetComponent<CollisionDetection>();
            Animator hitboxAnimator = newObj.GetComponent<Animator>();
            colScript.setAttacker(gameObject);
            followScript.Set(transform,dir);
            StartCoroutine(Despawn(newObj));
    }

    IEnumerator Despawn(GameObject obj)
    {
        yield return new WaitForSeconds(swingDelay);
        Destroy(obj);
        attacking = false;
    }
}
