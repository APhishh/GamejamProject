using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private float triggerRadius = 3f; // Radius within which the player can trigger dialogue
    [SerializeField] private LayerMask playerLayer; // Layer for detecting the player

    private bool isPlayerInRange;
    private bool isDialogueActive; // Flag to track if dialogue is active

    private void Update()
    {
        // Check if the player is in range and presses the E key or Submit button
        if (isPlayerInRange && (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Submit")))
        {
            if (!isDialogueActive)
            {
                // Start the dialogue if it's not already active
                TriggerDialogue();
            }
            else
            {
                // Progress to the next line if dialogue is already active
                FindObjectOfType<DialogueManager>().DisplayNextLine();
            }
        }
    }

    private void FixedUpdate()
    {
        // Check if the player is within the trigger radius
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, triggerRadius, playerLayer);
        isPlayerInRange = playerCollider != null;

        // Debugging: Log whether the player is in range
        Debug.Log($"Player in range: {isPlayerInRange}");
    }

    public void TriggerDialogue()
    {
        isDialogueActive = true;
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, this);
    }

    public void EndDialogue()
    {
        Debug.Log("Dialogue ended!");
        isDialogueActive = false; // Reset the dialogue active flag
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the trigger radius in the Scene view for debugging
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}

