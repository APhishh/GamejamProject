using UnityEngine;

public class ChargeEnemy : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private CapsuleCollider2D bodyCollider;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float detectionAngle = 60f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float hoverHeight = 3f;
    [SerializeField] private float chargeSpeed = 15f;
    [SerializeField] private float patrolRadius = 5f;
    private Vector2 startPosition;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 10f; // Force of the knockback
    [SerializeField] private float knockbackDuration = 0.2f; // Duration of the knockback effect


    [Header("Timers")]
    [SerializeField] private float chargePreparationTime = 2f;
    [SerializeField] private float stunDuration = 2f;
    [SerializeField] private float patrolWaitTime = 2f;

    [Header("Debug Colors")]
    [SerializeField] private Color idleColor = Color.green;
    [SerializeField] private Color alertColor = Color.yellow;
    [SerializeField] private Color stunnedColor = Color.red;

    private Transform player;
    private EnemyState currentState;
    private float stateTimer;
    private float chargeAngle;
    private Vector2 patrolTarget;
    private bool isPatrolWaiting;

    private enum EnemyState
    {
        Idle,
        Alert,
        Charging,
        Stunned
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPosition = transform.position;
        currentState = EnemyState.Idle;
        SetNewPatrolTarget();
        UpdateColor();
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState();
                break;
            case EnemyState.Alert:
                HandleAlertState();
                break;
            case EnemyState.Charging:
                HandleChargingState();
                break;
            case EnemyState.Stunned:
                HandleStunnedState();
                break;
        }

        // Always check if we're too far from patrol area
        if (Vector2.Distance(transform.position, startPosition) > patrolRadius && currentState != EnemyState.Charging)
        {
            ReturnToPatrolArea();
        }
    }

    private void UpdateColor()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                sr.color = idleColor;
                break;
            case EnemyState.Alert:
            case EnemyState.Charging:
                sr.color = alertColor;
                break;
            case EnemyState.Stunned:
                sr.color = stunnedColor;
                break;
        }
    }

    private bool IsPlayerInSight()
    {
        if (player == null) return false;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector2.Angle(transform.right, directionToPlayer);

        // Check if player is within the detection angle
        if (angleToPlayer <= detectionAngle / 2f)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Check if player is within detection range
            if (distanceToPlayer <= detectionRange)
            {
                // Raycast to check for obstacles between enemy and player
                RaycastHit2D hit = Physics2D.Raycast(
                    transform.position,
                    directionToPlayer,
                    distanceToPlayer,
                    obstacleLayer | playerLayer
                );

                // If we hit something and it's the player, they're in sight
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Draw the detection cone in the editor for debugging
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Draw detection range
        Gizmos.color = Color.yellow;

        // Draw the detection cone
        Vector2 leftDir = Quaternion.Euler(0, 0, detectionAngle / 2f) * transform.right;
        Vector2 rightDir = Quaternion.Euler(0, 0, -detectionAngle / 2f) * transform.right;

        Gizmos.DrawLine(transform.position, (Vector2)transform.position + leftDir * detectionRange);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + rightDir * detectionRange);

        // Draw arc
        int segments = 20;
        Vector2 previousPoint = (Vector2)transform.position + rightDir * detectionRange;

        for (int i = 1; i <= segments; i++)
        {
            float angle = -detectionAngle / 2f + ((float)i / segments) * detectionAngle;
            Vector2 direction = Quaternion.Euler(0, 0, angle) * transform.right;
            Vector2 point = (Vector2)transform.position + direction * detectionRange;

            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }
    }

    private void HandleIdleState()
    {
        // Check for player detection
        if (IsPlayerInSight())
        {
            currentState = EnemyState.Alert;
            stateTimer = chargePreparationTime;
            UpdateColor();
            return;
        }

        // Handle patrol behavior
        if (isPatrolWaiting)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                isPatrolWaiting = false;
                SetNewPatrolTarget();
            }
            return;
        }

        // Move towards patrol target
        Vector2 moveDirection = ((Vector2)patrolTarget - (Vector2)transform.position).normalized;
        rb.velocity = moveDirection * patrolSpeed;

        // Check if reached patrol target
        if (Vector2.Distance(transform.position, patrolTarget) < 0.1f)
        {
            isPatrolWaiting = true;
            stateTimer = patrolWaitTime;
            rb.velocity = Vector2.zero;
        }
    }

    private void HandleAlertState()
    {
        Debug.Log($"Enemy Rigidbody: {rb.name}");
        float currentHeight = transform.position.y - startPosition.y;
        if (currentHeight < hoverHeight)
        {
            rb.velocity = new Vector2(0, patrolSpeed);
            return;
        }

        rb.velocity = Vector2.zero;
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            chargeAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

            currentState = EnemyState.Charging;
            UpdateColor();
        }
    }

    private void HandleChargingState()
    {
        if (player != null)
        {
            Vector2 chargeDirection = new Vector2(Mathf.Cos(chargeAngle * Mathf.Deg2Rad), Mathf.Sin(chargeAngle * Mathf.Deg2Rad));

            // Move in the direction of the charge angle
            rb.velocity = chargeDirection * chargeSpeed;
        }
    }

    private void HandleStunnedState()
    {
        // Reset velocity before applying knockback
        rb.velocity = Vector2.zero;
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0)
        {
            // Check if player is nearby to determine next state
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= detectionRange && IsPlayerInSight())
            {
                currentState = EnemyState.Alert;
                stateTimer = chargePreparationTime;
            }
            else
            {
                currentState = EnemyState.Idle;
                SetNewPatrolTarget();
            }
            UpdateColor();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only process collision if we're in charging state
        if (currentState == EnemyState.Charging)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Enemy collided with Player!");

                // Apply knockback to both the player and the enemy
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

                // Knockback the player
                PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    Debug.Log($"Applying knockback to Player! Direction: {knockbackDirection}, Force: {knockbackDirection * knockbackForce}");
                    playerStats.ApplyKnockback(knockbackDirection * knockbackForce, knockbackDuration);
                    playerStats.TakeDamage(20f); // Deal 20 damage to the player
                }

                // Knockback the enemy
                Debug.Log($"Applying knockback to Enemy! Direction: {-knockbackDirection}, Force: {-knockbackDirection * knockbackForce}");
                // Apply knockback force
                rb.AddForce(-knockbackDirection * knockbackForce, ForceMode2D.Impulse);

                // Stun the enemy after the collision
                Stun();
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Debug.Log("Enemy collided with Ground!");
                // If the enemy hits a wall or floor, stun it
                Stun();
            }
        }
    }


    private void SetNewPatrolTarget()
    {
        // Generate a random point within patrol radius
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        patrolTarget = startPosition + (randomDirection * Random.Range(0, patrolRadius));
    }

    private void ReturnToPatrolArea()
    {
        Vector2 directionToStart = (startPosition - (Vector2)transform.position).normalized;
        rb.velocity = directionToStart * patrolSpeed;

        // Check if back in patrol area
        if (Vector2.Distance(transform.position, startPosition) <= patrolRadius)
        {
            SetNewPatrolTarget();
        }
    }


    public void Stun()
    {
        currentState = EnemyState.Stunned;
        stateTimer = stunDuration;
        rb.velocity = Vector2.zero;
        UpdateColor();
    }


    private void OnDamageTaken(float damage)
    {
        // Switch to Alert state if not already in Alert or Charging
        if (currentState != EnemyState.Alert && currentState != EnemyState.Charging)
        {
            currentState = EnemyState.Alert;
            stateTimer = chargePreparationTime;
            UpdateColor();
        }
    }

    // Called when the enemy dies (optional, if you want to handle death logic here)
    private void OnDeath()
    {
        // Example: Play a death animation or spawn effects
        Debug.Log("Enemy has died!");
    }
}
