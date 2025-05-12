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
    [SerializeField] private float dashSpeed = 20f; // Speed of the dash
    [SerializeField] private float dashDistance = 5f; // Distance of the dash
    [SerializeField] private float dashCooldown = 2f; // Cooldown for the dash
    [SerializeField] private GameObject dashParticlePrefab; // Prefab for the dash particle effect
    [SerializeField] private TrailRenderer dashTrail; // Reference to the Trail Renderer
    [SerializeField] Animator animator;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private SpriteRenderer SR;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool grounded;
    [SerializeField] private bool canDoubleJump = true; // Flag to enable/disable double jump

    private bool canDash = true;
    private bool isDashing = false; // Flag to indicate if the player is currently dashing
    private bool isClimbing = false;
    private bool hasDoubleJumped = false; // Tracks if the player has already double jumped

    void Update()
    {
        if (!isDashing) // Skip movement and jump logic while dashing
        {
            HandleFalling();
            HandleMovement();
            HandleJump();
        }
        HandleDash();
    }

    void HandleFalling()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1, groundLayer);
        if (hit.collider != null)
        {
            grounded = true;
            hasDoubleJumped = false; // Reset double jump when grounded
            animator.SetBool("Grounded", true);
        }
        else
        {
            animator.SetBool("Grounded", false);
            grounded = false;
        }
    }

    void HandleMovement()
    {
        // Check if movement input is allowed
        if (!InputManager.Instance.CanProcessInput("Horizontal") || !InputManager.Instance.CanProcessInput("Vertical"))
        {
            playerRB.velocity = new Vector2(0, playerRB.velocity.y); // Stop movement if input is disabled
            return;
        }

        // Get input from both keyboard and controller horizontal axis
        float velX = Input.GetAxis("Horizontal") * movementSpeed;
        float velY = Input.GetAxis("Vertical") * movementSpeed;
        // Also support D-pad input
        velX += Input.GetAxis("LeftJoystickHorizontal") * movementSpeed;

        animator.SetFloat("Speed", math.abs(playerRB.velocity.x)); // Disabled for debugging

        // Update sprite direction
        if (velX < 0)
        {
            SR.flipX = true;
        }
        else if (velX > 0)
        {
            SR.flipX = false;
        }

        if (isClimbing)
        {
            playerRB.velocity = new Vector2(0, velY);
        }
        else
        {
            playerRB.velocity = new Vector2(velX, playerRB.velocity.y);
        }
    }

    void HandleJump()
    {
        // Check if jump input is allowed
        if (!InputManager.Instance.CanProcessInput("Jump"))
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            if (grounded)
            {
                // Perform normal jump
                isClimbing = false;
                playerRB.velocity = new Vector3(playerRB.velocity.x, jumpPower);
            }
            else if (canDoubleJump && !hasDoubleJumped)
            {
                // Perform double jump
                isClimbing = false;
                playerRB.velocity = new Vector3(playerRB.velocity.x, jumpPower);
                hasDoubleJumped = true; // Mark double jump as used
                animator.SetTrigger("DoubleJump"); // Optional: Trigger double jump animation
            }
        }
    }

    void HandleDash()
    {
        // Check if dash input is allowed
        if (!InputManager.Instance.CanProcessInput("Dash"))
        {
            return;
        }

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton1)) && canDash && playerStats.UseStamina(25f)) // Check if enough stamina
        {
            StartCoroutine(Dash());
        }
    }

    public void setSpeed(float newspeed)
    {
        movementSpeed = newspeed;
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true; // Set dashing flag to true
        isClimbing = false;
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

    public bool getClimbing()
    {
        return isClimbing;
    }

    public void setClimbing(bool val)
    {
        isClimbing = val;
    }

    public void SetJumpPower(float val)
    {
        jumpPower = val;
    }
}


