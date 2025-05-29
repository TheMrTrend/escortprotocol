using UnityEngine;

public class ScientistMeeting : MonoBehaviour
{
    GameObject syringe;
    Transform syringeSpawnLocation;
    DoorSlider door;
    
    public void FinishInteraction()
    {
        Instantiate(syringe, syringeSpawnLocation.position, Quaternion.identity);
        GameObject.FindGameObjectWithTag("Escort").GetComponent<Escort>().StartFollow();
        GameManager.instance.playerController.movementLocked = false;
    }

    public void StartInteraction()
    {
        GameManager.instance.playerController.movementLocked = true;
        door.CloseDoor();
    }
}
