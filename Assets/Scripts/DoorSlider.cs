using DG.Tweening;
using UnityEngine;

public class DoorSlider : MonoBehaviour
{
    [SerializeField] GameObject doorL;
    [SerializeField] GameObject doorR;
    Vector3 origDoorR;
    Vector3 origDoorL;
    public float distance = 2.5f;
    public float openSpeed = 0.5f;

    private void Start()
    {
        origDoorR = doorR.transform.localPosition;
        origDoorL = doorL.transform.localPosition;
    }
    public void OpenDoor()
    {
        doorR.transform.DOLocalMoveZ(origDoorR.z - distance, openSpeed).SetEase(Ease.InQuart);
        doorL.transform.DOLocalMoveZ(origDoorL.z + distance, openSpeed).SetEase(Ease.InQuart);
    }

    public void CloseDoor()
    {
        doorR.transform.DOLocalMoveZ(origDoorR.z, openSpeed).SetEase(Ease.OutQuart);
        doorL.transform.DOLocalMoveZ(origDoorL.z, openSpeed).SetEase(Ease.OutQuart);
    }
}
