using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRegenerationScene : MonoBehaviour
{
    [SerializeField] Enemy cutsceneEnemy;
    [SerializeField] DoorSlider door;
    [SerializeField] DialogueSequence dialogueSequence;
    Coroutine playerLockOn;
    public void StartScene()
    {
        playerLockOn = StartCoroutine(LockOnEnemy());
        cutsceneEnemy.InstantRegeneration();
        GameManager.instance.playerController.movementLocked = true;
        Camera.main.GetComponent<CameraController>().isMovable = false;
        DialogueManager.instance.Activate(dialogueSequence);
    }

    public void FinishScene()
    {
        StopCoroutine(playerLockOn);
        cutsceneEnemy.agent.isStopped = false;
        cutsceneEnemy.SetPlayerAsTarget();
        GameManager.instance.playerController.movementLocked = false;
        Camera.main.GetComponent<CameraController>().isMovable = true;
        door.OpenDoor();
    }

    IEnumerator LockOnEnemy()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            GameManager.instance.playerController.EnemyLockOn(cutsceneEnemy);
            if (cutsceneEnemy.agent.enabled && cutsceneEnemy.agent.isOnNavMesh && !cutsceneEnemy.agent.isStopped)
            {
                cutsceneEnemy.agent.isStopped = true;
            }
        }
    }
}
