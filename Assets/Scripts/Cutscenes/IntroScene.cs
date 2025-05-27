using System.Collections;
using UnityEngine;

public class IntroScene : MonoBehaviour
{
    [SerializeField] DialogueSequence sequence;
    bool shaking = false;
    [SerializeField] DoorSlider door;
    public void StartScene()
    {
        GameManager.instance.playerController.movementLocked = true;
        Camera.main.GetComponent<CameraController>().isMovable = false;
        DialogueManager.instance.Activate(sequence);
        shaking = true;
        StartCoroutine(ElevatorShake());
    }

    IEnumerator ElevatorShake()
    {
        while (shaking)
        {
            //shake logic
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator DelayedDoorOpen()
    {
        yield return new WaitForSeconds(3);
        door.OpenDoor();
    }

    public void SceneFinished()
    {
        GameManager.instance.playerController.movementLocked = false;
        Camera.main.GetComponent<CameraController>().isMovable = true;
        shaking = false;
        StartCoroutine(DelayedDoorOpen());
    }
}
