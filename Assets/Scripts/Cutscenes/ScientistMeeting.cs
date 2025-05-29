using UnityEngine;

public class ScientistMeeting : MonoBehaviour
{
    [SerializeField] GameObject syringe;
    [SerializeField] Transform syringeSpawnLocation;
    [SerializeField] DoorSlider door;
    [SerializeField] DialogueSequence dialogueSequence;
    [SerializeField] DoorSlider outsideDoor;
    [SerializeField] DoorSlider outsideDoor2;

    public void FinishInteraction()
    {
        Instantiate(syringe, syringeSpawnLocation.position, Quaternion.identity);
        GameObject.FindGameObjectWithTag("Escort").GetComponent<Escort>().StartFollow();
        GameManager.instance.playerController.movementLocked = false;
        UIManager.instance.EnableScientistHealth();
        outsideDoor.OpenDoor();
        outsideDoor2.OpenDoor();
    }

    public void StartInteraction()
    {
        GameManager.instance.playerController.movementLocked = true;
        door.CloseDoor();
        DialogueManager.instance.Activate(dialogueSequence);
    }
}
