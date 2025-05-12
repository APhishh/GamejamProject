using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public Transform player; // Reference to the player's Transform
    public bool flipX; // Checkbox to determine if the sprite should flip on the X-axis

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer is not attached to the GameObject.");
        }
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player Transform is not assigned.");
            return;
        }

        // Determine the direction to the player
        float directionToPlayer = player.position.x - transform.position.x;

        // Flip the sprite based on the player's position
        if (spriteRenderer != null)
        {
            if (flipX)
            {
                spriteRenderer.flipX = directionToPlayer > 0; // Flip if player is to the right
            }
            else
            {
                spriteRenderer.flipX = directionToPlayer < 0; // Flip if player is to the left
            }
        }
    }
}
