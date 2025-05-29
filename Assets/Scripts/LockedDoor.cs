using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [SerializeField] GameObject key;
    [SerializeField] DoorSlider door;
    public void OpenDoor()
    {
        if (GameManager.instance.playerController.keys.Contains(key))
        {
            door.OpenDoor();
        }
    }
}
