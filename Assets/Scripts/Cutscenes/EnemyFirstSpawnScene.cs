using System.Collections;
using UnityEngine;

public class EnemyFirstSpawnScene : MonoBehaviour
{
    [SerializeField] Enemy cutsceneEnemy;
    Coroutine playerLockOn;
    [SerializeField] DoorSlider door;
    [SerializeField] Transform enemyNavigatePosition;
    public void StartScene()
    {
        GameManager.instance.playerController.movementLocked = true;
        Camera.main.GetComponent<CameraController>().isMovable = false;
        playerLockOn = StartCoroutine(LockOnEnemy());
        cutsceneEnemy.currentTarget = enemyNavigatePosition;
        door.OpenDoor();
        
    }

    public void FinishScene()
    {
        door.CloseDoor();
        GameManager.instance.playerController.movementLocked = false;
        Camera.main.GetComponent<CameraController>().isMovable = true;
        cutsceneEnemy.SetPlayerAsTarget();
        StopCoroutine(playerLockOn);
    }

    IEnumerator LockOnEnemy()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            GameManager.instance.playerController.EnemyLockOn(cutsceneEnemy);
        }
    }
}
