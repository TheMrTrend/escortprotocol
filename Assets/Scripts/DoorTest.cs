using UnityEngine;

public class DoorTest : MonoBehaviour
{
    public DialogueSequence test;
    public void OpenDoor()
    {
        DialogueManager.instance.Activate(test);
        transform.position = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);
    }

    public void CloseDoor()
    {
        //gameObject.SetActive(true);
        transform.position = new Vector3(transform.position.x, transform.position.y - 5, transform.position.z);
    }
}
