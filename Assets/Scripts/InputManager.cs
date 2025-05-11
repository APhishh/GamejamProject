using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private bool isDialogueInputOnly = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void DisableAllInputsExceptDialogue()
    {
        isDialogueInputOnly = true;
        Debug.Log("All inputs disabled except dialogue.");
    }

    public void EnableAllInputs()
    {
        isDialogueInputOnly = false;
        Debug.Log("All inputs enabled.");
    }

    public bool CanProcessInput(string inputName)
    {
        // Allow only dialogue-related inputs when in dialogue mode
        if (isDialogueInputOnly)
        {
            return inputName == "Submit" || inputName == "E";
        }

        // Allow all inputs otherwise
        return true;
    }
}
