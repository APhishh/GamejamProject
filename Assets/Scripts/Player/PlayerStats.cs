using UnityEngine;
using System.Collections;
using System;


public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float currentStamina;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private float dashStaminaCost = 25f;

    [Header("Weapon")]
    [SerializeField] private WeaponData currentWeapon;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] Animator animator;
    [SerializeField] PlayerMovement PM;
    

    private void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    private void Update()
    {
        RegenerateStamina();
    }

    private void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            return true;
        }
        return false;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            // Handle player death
            Debug.Log("Player died!");
           // gameOverScreen.active = true;
            gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
            animator.SetBool("Dead", true);
            PM.setSpeed(0);
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            
            StartCoroutine(FadeOut(sr,0.16f));
            // Add death logic here (e.g., respawn, game over screen)
        }
    }

    public void ApplyKnockback(Vector2 force, float duration)
    {
        StartCoroutine(KnockbackCoroutine(force, duration));
    }

    private IEnumerator KnockbackCoroutine(Vector2 force, float duration)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Reset velocity before applying knockback
            rb.velocity = Vector2.zero;

            // Apply the knockback force
            rb.AddForce(force, ForceMode2D.Impulse);

            // Wait for the knockback duration
            yield return new WaitForSeconds(duration);

            // Reset the player's velocity
            rb.velocity = Vector2.zero;
        }
    }

    IEnumerator FadeOut(SpriteRenderer sr, float fadeSpeed)
{
    Color color = sr.color;

    while (color.a > 0f)
    {
        color.a -= fadeSpeed * Time.deltaTime;
        sr.color = color;
        yield return null; 
    }

    color.a = 0f;
    sr.color = color;
    Destroy(gameObject);
}

    // Getters for UI
    public float GetHealthPercentage() => currentHealth / maxHealth;
    public float GetStaminaPercentage() => currentStamina / maxStamina;
    public WeaponData GetCurrentWeapon() => currentWeapon;
}
