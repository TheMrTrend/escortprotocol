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
        DialogueManager.instance.Activate(sequence);
        shaking = true;
        StartCoroutine(ElevatorShake());
    }

    IEnumerator ElevatorShake()
    {
        while (shaking)
        {
            CameraShake.Shake(0.05f, 0.005f);
            yield return new WaitForSeconds(0.05f);
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
        shaking = false;
        StartCoroutine(DelayedDoorOpen());
    }
}
