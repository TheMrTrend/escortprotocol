using System.Collections;
using UnityEngine;

public class SyringeBehavior : Item
{
    bool canAttack = true;
    public LayerMask ignoreLayers;
    public float stabDistance = 5f;
    [SerializeField] ParticleSystem vanquishParticles;
    float currentSpread;
    Enemy enemyBeingKilled;

    public override void Primary()
    {
        if (canAttack)
        {
            Attack();
        }
        
    }

    private void Update()
    {
        if (enemyBeingKilled != null)
        {
            GameManager.instance.playerController.EnemyLockOn(enemyBeingKilled);
        }
    }

    void Attack()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, stabDistance, ~ignoreLayers))
        {
            if (hit.collider.gameObject.TryGetComponent(out Enemy enemy))
            {
                if (enemy.isKillable)
                {
                    canAttack = false;
                    GameManager.instance.playerController.movementLocked = true;
                    enemy.StartDeath();
                    enemyBeingKilled = enemy;
                    Camera.main.GetComponent<CameraController>().isMovable = false;
                    animator.SetTrigger("Kill");
                }
            }
        }
    }

    public void SyringeConnect()
    {
        enemyBeingKilled.StartCoroutine(enemyBeingKilled.Vanquish());
        SpawnEssenceParticles();
        GameManager.instance.playerController.AddEssence(enemyBeingKilled.essencePerKill);
    }

    public void SpawnEssenceParticles()
    {
        Vector3 raisedEnemyPosition = enemyBeingKilled.boneToFollow.position;
        Vector3 playerDir = (Camera.main.transform.position - (raisedEnemyPosition));
        float distance = playerDir.magnitude;
        playerDir = playerDir.normalized;

        ParticleSystem p = Instantiate(vanquishParticles, raisedEnemyPosition, Quaternion.LookRotation(playerDir, Vector3.up));
        ParticleSystem.MainModule main = p.main;
        main.startSpeed = distance;

        p.Play();
    }

    void FinishAttack()
    {
        canAttack = true;
        GameManager.instance.playerController.movementLocked = false;
        enemyBeingKilled = null;
        Camera.main.GetComponent<CameraController>().isMovable = true;
    }

}
