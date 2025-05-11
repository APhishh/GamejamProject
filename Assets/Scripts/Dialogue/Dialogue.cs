using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName; // Name of the speaker
        [TextArea(3, 5)] public string text; // Dialogue text
    }

    public DialogueLine[] lines; // Array of dialogue lines
}
