using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] GameObject hitbox;
    [SerializeField] GameObject hitParticle;
    [SerializeField] GameObject attackIndicatorPrefab; // Prefab for the attack indicator
    [SerializeField] Animator animator; // Animator remains, but we won't use it
    [SerializeField] private Rigidbody2D playerRB; // Reference to the player's Rigidbody2D
    [SerializeField] private bool attacking;
    [SerializeField] private float swingDelay;
    [SerializeField] private float indicatorOrbitSpeed = 5f; // Speed of the attack indicator orbiting the player
    [SerializeField] private float indicatorDistance = 3f; // Distance of the attack indicator from the player

    private GameObject attackIndicator; // Instance of the attack indicator
    private SpriteRenderer attackIndicatorSprite; // SpriteRenderer for the attack indicator
    private Vector3 lastAttackDirection = Vector3.right; // Default direction (facing right)

    void Start()
    {
        // Instantiate the attack indicator at the player's position + default direction
        attackIndicator = Instantiate(attackIndicatorPrefab, transform.position + lastAttackDirection * indicatorDistance, Quaternion.identity);

        // Get the SpriteRenderer component of the attack indicator
        attackIndicatorSprite = attackIndicator.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Check if inputs are allowed
        // if (!InputManager.Instance.CanProcessInput("PlayerAttack"))
        // {
        //     return; // Skip all input handling if inputs are disabled
        // }

        HandleAttackInput();
        UpdateAttackIndicator();
    }

    void HandleAttackInput()
    {
        // Mouse input
        if (Input.GetMouseButtonDown(0) && attacking == false)
        {
            attacking = true;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; // Ensure the z-coordinate is 0
            StartCoroutine(Attacking(mousePosition));
        }

        // Joystick input
        float joystickX = Input.GetAxis("RightJoystickHorizontal"); // Right stick horizontal axis
        float joystickY = -Input.GetAxis("RightJoystickVertical"); // Right stick vertical axis

        if (math.abs(joystickX) > 0.1f || math.abs(joystickY) > 0.1f) // Deadzone check
        {
            Vector3 joystickInput = new Vector3(joystickX, joystickY, 0).normalized;

            // Update the last valid attack direction
            lastAttackDirection = joystickInput;

            // Show the attack indicator sprite
            if (!attackIndicatorSprite.enabled)
            {
                attackIndicatorSprite.enabled = true;
            }
        }
        else
        {
            // Hide the attack indicator sprite if no joystick input is detected
            if (attackIndicatorSprite.enabled)
            {
                attackIndicatorSprite.enabled = false;
            }
        }

        // Attack when R1 is pressed
        if (Input.GetKeyDown(KeyCode.JoystickButton5) && attacking == false) // R1 is JoystickButton5
        {
            attacking = true;
            StartCoroutine(Attacking(attackIndicator.transform.position));
        }
    }

    void UpdateAttackIndicator()
    {
        if (attackIndicator != null)
        {
            // Calculate the orbit position based on the player's position and the last attack direction
            Vector3 orbitPosition = transform.position + lastAttackDirection * indicatorDistance;

            // Add the player's velocity to make the indicator follow the player's speed
            orbitPosition += (Vector3)playerRB.velocity * Time.deltaTime;

            // Update the attack indicator's position
            attackIndicator.transform.position = orbitPosition;
        }
    }

    IEnumerator Attacking(Vector3 targetPosition)
    {
        yield return new WaitForSeconds(0.1f);

        Vector3 dir = (targetPosition - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject newObj = Instantiate(hitbox, transform.position + dir, Quaternion.Euler(0, 0, angle));
        HitboxFollowCharacter followScript = newObj.GetComponent<HitboxFollowCharacter>();
        CollisionDetection colScript = newObj.GetComponent<CollisionDetection>();
        Animator hitboxAnimator = newObj.GetComponent<Animator>();
        colScript.setAttacker(gameObject);
        followScript.Set(transform, dir);
        StartCoroutine(Despawn(newObj));
    }

    IEnumerator Despawn(GameObject obj)
    {
        yield return new WaitForSeconds(swingDelay);
        Destroy(obj);
        attacking = false;
    }
}
