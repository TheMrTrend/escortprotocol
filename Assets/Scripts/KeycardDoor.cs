using DG.Tweening;
using UnityEngine;

public class KeyCardDoorSlider : MonoBehaviour
{
    [SerializeField] GameObject doorL;
    [SerializeField] GameObject doorR;
    Vector3 origDoorR;
    Vector3 origDoorL;
    public float distance = 2.5f;
    public float openSpeed = 0.5f;
    [SerializeField] DialogueSequence dialogueSequence;

    private void Start()
    {
        origDoorR = doorR.transform.localPosition;
        origDoorL = doorL.transform.localPosition;
    }
    public void OpenDoor()
    {
        if (!GameManager.instance.playerController.hasKeyCard)
        {
            DialogueManager.instance.Activate(dialogueSequence);
        } else
        {
            doorR.transform.DOLocalMoveZ(origDoorR.z - distance, openSpeed).SetEase(Ease.InQuart);
            doorL.transform.DOLocalMoveZ(origDoorL.z + distance, openSpeed).SetEase(Ease.InQuart);
        }
    }

    public void CloseDoor()
    {
        doorR.transform.DOLocalMoveZ(origDoorR.z, openSpeed).SetEase(Ease.OutQuart);
        doorL.transform.DOLocalMoveZ(origDoorL.z, openSpeed).SetEase(Ease.OutQuart);
    }
}
