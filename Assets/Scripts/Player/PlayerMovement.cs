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
    [SerializeField] Animator animator; // Animator remains, but we won't use it
    [SerializeField] private SpriteRenderer SR;
    private bool canJump = true;

    void Update()
    {
        HandleMovement();
        HandleJump();
    }

    void HandleMovement()
    {
        // Get input from both keyboard and controller horizontal axis
        float velX = Input.GetAxis("Horizontal") * movementSpeed;

        // Also support D-pad input
        velX += Input.GetAxis("DPad_Horizontal") * movementSpeed;

        // animator.SetFloat("Speed", math.abs(playerRB.velocity.x)); // Disabled for debugging

        // Update sprite direction
        if (velX < 0)
        {
            SR.flipX = true;
        }
        else if (velX > 0)
        {
            SR.flipX = false;
        }

        playerRB.velocity = new Vector2(velX, playerRB.velocity.y);
    }

    void HandleJump()
    {
        // Support both keyboard space and controller button (usually A/X button)
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")) && canJump)
        {
            canJump = false;
            playerRB.velocity = new Vector3(playerRB.velocity.x, jumpPower);
            StartCoroutine(JumpCooldown());
        }
    }

    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }
}


