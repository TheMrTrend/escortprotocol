using UnityEngine;

public class ScientistMeeting : MonoBehaviour
{
    [SerializeField] GameObject syringe;
    [SerializeField] Transform syringeSpawnLocation;
    [SerializeField] DoorSlider door;
    [SerializeField] DialogueSequence dialogueSequence;
    
    public void FinishInteraction()
    {
        Instantiate(syringe, syringeSpawnLocation.position, Quaternion.identity);
        GameObject.FindGameObjectWithTag("Escort").GetComponent<Escort>().StartFollow();
        GameManager.instance.playerController.movementLocked = false;
        UIManager.instance.EnableScientistHealth();
    }

    public void StartInteraction()
    {
        GameManager.instance.playerController.movementLocked = true;
        door.CloseDoor();
        DialogueManager.instance.Activate(dialogueSequence);
    }
}
