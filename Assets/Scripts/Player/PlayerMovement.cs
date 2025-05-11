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
    [SerializeField] private float dashSpeed = 20f; // Speed of the dash
    [SerializeField] private float dashDistance = 5f; // Distance of the dash
    [SerializeField] private float dashCooldown = 2f; // Cooldown for the dash
    [SerializeField] private GameObject dashParticlePrefab; // Prefab for the dash particle effect
    [SerializeField] private TrailRenderer dashTrail; // Reference to the Trail Renderer
    [SerializeField] Animator animator;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private SpriteRenderer SR;

    private bool canJump = true;
    private bool canDash = true;
    private bool isDashing = false; // Flag to indicate if the player is currently dashing

    void Update()
    {
        if (!isDashing) // Skip movement and jump logic while dashing
        {
            HandleMovement();
            HandleJump();
        }
        HandleDash();
    }

    void HandleMovement()
    {
        // Get input from both keyboard and controller horizontal axis
        float velX = Input.GetAxis("Horizontal") * movementSpeed;

        // Also support D-pad input
        velX += Input.GetAxis("LeftJoystickHorizontal") * movementSpeed;

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
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0)) && canJump)
        {
            canJump = false;
            playerRB.velocity = new Vector3(playerRB.velocity.x, jumpPower);
            StartCoroutine(JumpCooldown());
        }
    }

    void HandleDash()
    {
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton1))
            && canDash && playerStats.UseStamina(25f)) // Check if enough stamina
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true; // Set dashing flag to true

        // Trigger the dash particle effect
        if (dashParticlePrefab != null)
        {
            Instantiate(dashParticlePrefab, transform.position, Quaternion.identity);
        }

        // Enable the dash trail
        if (dashTrail != null)
        {
            dashTrail.emitting = true;
        }

        // Determine the dash direction based on the player's current movement direction
        float dashDirection = SR.flipX ? -1f : 1f; // Dash left if sprite is flipped, otherwise dash right

        // Calculate the dash velocity
        Vector2 dashVelocity = new Vector2(dashDirection * dashSpeed, 0);

        // Temporarily set the player's velocity to the dash velocity
        playerRB.velocity = dashVelocity;

        // Wait for the dash duration (calculated based on distance and speed)
        float dashDuration = dashDistance / dashSpeed;
        yield return new WaitForSeconds(dashDuration);

        // Reset the player's velocity to zero (or maintain horizontal movement if needed)
        playerRB.velocity = new Vector2(0, playerRB.velocity.y);

        // Disable the dash trail
        if (dashTrail != null)
        {
            dashTrail.emitting = false;
        }

        isDashing = false; // Reset dashing flag

        // Wait for the cooldown before allowing another dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }
}


