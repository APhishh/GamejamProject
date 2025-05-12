using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private float triggerRadius = 1f; // Radius within which the player can trigger dialogue
    [SerializeField] private LayerMask playerLayer; // Layer for detecting the player
    [SerializeField] private bool triggerOnce = false; // If true, dialogue can only be triggered once
    [SerializeField] private bool triggeredByRange = false; // If true, dialogue triggers automatically when in range

    private bool isPlayerInRange;
    private bool isDialogueActive; // Flag to track if dialogue is active
    private bool hasBeenTriggered = false; // Tracks if the dialogue has already been triggered

    private void Update()
    {
        if (triggeredByRange)
        {
            // Automatically trigger dialogue when the player enters the range
            if (isPlayerInRange && !isDialogueActive && !hasBeenTriggered)
            {
                TriggerDialogue();
            }
        }
        else
        {
            // Check if the player is in range and presses the E key or Submit button
            if (isPlayerInRange && InputManager.Instance.CanProcessInput("E") && InputManager.Instance.CanProcessInput("Submit")
                && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2)))
            {
                if (!isDialogueActive && !hasBeenTriggered)
                {
                    // Start the dialogue if it's not already active
                    TriggerDialogue();
                }
                else if (isDialogueActive)
                {
                    // Progress to the next line if dialogue is already active
                    FindObjectOfType<DialogueManager>().DisplayNextLine();
                }
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
        hasBeenTriggered = triggerOnce; // Mark as triggered if "triggerOnce" is true
        InputManager.Instance.DisableAllInputsExceptDialogue(); // Disable non-dialogue inputs
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, this);
    }

    public void EndDialogue()
    {
        Debug.Log("Dialogue ended!");
        isDialogueActive = true; // Temporarily keep it true to block immediate re-trigger
        Invoke(nameof(ResetDialogueState), 0.2f); // Delay before allowing new dialogue
        InputManager.Instance.EnableAllInputs(); // Re-enable all inputs
    }

    private void ResetDialogueState()
    {
        isDialogueActive = false; // Allow dialogue to be triggered again
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the trigger radius in the Scene view for debugging
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}

