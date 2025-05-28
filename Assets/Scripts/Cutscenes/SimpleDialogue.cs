using UnityEngine;

public class SimpleDialogue : MonoBehaviour
{
    [SerializeField] DialogueSequence sequence;

    public void PlayDialogue()
    {
        DialogueManager.instance.Activate(sequence);
    }
}
