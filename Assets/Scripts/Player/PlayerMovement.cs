using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float jumpCooldown;
    [SerializeField] Animator animator;
    [SerializeField] private SpriteRenderer SR;
    private bool canJump = true;
    // Update is called once per frame
    void Update()
    {

        animator.SetFloat("Speed", math.abs(playerRB.velocity.x));
        if (Input.GetKeyDown(KeyCode.Space) && canJump == true)
        {
            canJump = false;
            playerRB.velocity= new Vector3(playerRB.velocity.x, jumpPower);
            StartCoroutine(JumpCooldown());
        }

        float velX = Input.GetAxis("Horizontal") * movementSpeed;

        if(velX < 0)
        {
            SR.flipX = true;
        }
        else if(velX > 0)
        {
            SR.flipX = false;
        }

        playerRB.velocity = new Vector2(velX,playerRB.velocity.y); 
    }

    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }
}


