using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject dialoguePanel;

    [Header("References")]
    [SerializeField] private Rigidbody2D playerRigidbody; // Reference to player's Rigidbody2D

    private Dialogue currentDialogue;
    private int currentLineIndex;
    private bool isDialogueActive = false;
    private bool canProcessInput = true;
    private DialogueTrigger currentTrigger; // Cache the current trigger

    private void Awake()
    {
        // Find player's Rigidbody2D if not assigned
        if (playerRigidbody == null)
        {
            playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        }
    }

    public void StartDialogue(Dialogue dialogue, DialogueTrigger trigger)
    {
        if (dialogue == null || dialogue.lines == null || dialogue.lines.Length == 0)
        {
            return;
        }

        // Stop player movement
        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector2.zero;
        }

        currentDialogue = dialogue;
        currentTrigger = trigger; // Cache the trigger
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        isDialogueActive = true;
        canProcessInput = false;
        InputManager.Instance.DisableAllInputsExceptDialogue();

        DisplayCurrentLine();
        Invoke(nameof(EnableInput), 0.2f);
    }

    public void DisplayNextLine()
    {
        if (!isDialogueActive || !canProcessInput)
        {
            return;
        }

        if (currentLineIndex >= currentDialogue.lines.Length - 1)
        {
            EndDialogue();
            return;
        }

        currentLineIndex++;
        DisplayCurrentLine();
    }

    private void DisplayCurrentLine()
    {
        if (currentDialogue == null || currentLineIndex >= currentDialogue.lines.Length)
        {
            return;
        }

        var line = currentDialogue.lines[currentLineIndex];
        speakerNameText.text = line.speakerName;
        dialogueText.text = line.text;
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false;
        InputManager.Instance.EnableAllInputs();

        // Notify the cached trigger
        if (currentTrigger != null)
        {
            currentTrigger.EndDialogue();
        }

        currentDialogue = null;
        currentTrigger = null;
    }

    private void Update()
    {
        if (isDialogueActive && canProcessInput)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Submit"))
            {
                DisplayNextLine();
            }
        }
    }

    private void EnableInput()
    {
        canProcessInput = true;
    }
}
