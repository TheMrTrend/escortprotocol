using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SyringeBehavior : Item
{
    bool canAttack = true;
    public LayerMask ignoreLayers;
    public float stabDistance = 5f;
    [SerializeField] ParticleSystem vanquishParticles;
    float currentSpread;
    Enemy enemyBeingKilled;
    [SerializeField] Volume darkening;
    Dictionary<GameObject, string> nonDarkenedObjects = new Dictionary<GameObject, string>();
    string darkenLayer = "Darkened";
    [SerializeField] AnimationClip useClip;


    public override void Primary()
    {
        if (canAttack)
        {
            Attack();
        }
        
    }

    public override void Secondary()
    {
        if (GameManager.instance.playerController.essence > 0)
        {
            GameManager.instance.playerController.AddHealth(Mathf.RoundToInt(GameManager.instance.playerController.maxHealth * ((float)GameManager.instance.playerController.essence / (float)GameManager.instance.playerController.maxEssence)));
            GameManager.instance.playerController.essence = 0;

            GameManager.instance.playerController.essenceUpdated.Invoke();
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
                    enemyBeingKilled = enemy;
                    Camera.main.GetComponent<CameraController>().isMovable = false;
                    
                    animator.SetTrigger("Kill");
                    SyringeQuickTime();
                }
            }
        }
    }

    public void SyringeQuickTime()
    {
        float timeMult = useClip.length / enemyBeingKilled.qteLength;
        UIManager.instance.quickTimeEvent.StartQTE(enemyBeingKilled.qteActions, enemyBeingKilled.qteLength, timeMult).AddListener(QuickTimeResult);
        EnableDarkness(timeMult);
        
    }

    void QuickTimeResult(bool success)
    {
        if (!success)
        {
            animator.SetTrigger("Event Fail");
            enemyBeingKilled.InstantRegeneration();
            enemyBeingKilled = null;
        } else
        {
            animator.SetTrigger("Event Success");
            enemyBeingKilled.StartDeath();
        }
        DisableDarkness();
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


    void EnableDarkness(float speed)
    {
        nonDarkenedObjects.Add(gameObject, LayerMask.LayerToName(gameObject.layer));
        gameObject.layer = LayerMask.NameToLayer(darkenLayer);
        nonDarkenedObjects.Add(enemyBeingKilled.model.gameObject, LayerMask.LayerToName(enemyBeingKilled.model.gameObject.layer));
        enemyBeingKilled.model.gameObject.layer = LayerMask.NameToLayer(darkenLayer);
        DOTween.To(() => darkening.weight, x => darkening.weight = x, 1f, 0.2f).timeScale /= speed;
    }

    void DisableDarkness()
    {
        foreach (GameObject obj in nonDarkenedObjects.Keys)
        {
            if (obj != null)
            {
                obj.layer = LayerMask.NameToLayer(nonDarkenedObjects[obj]);
                
            }
        }
        nonDarkenedObjects.Clear();

        DOTween.To(() => darkening.weight, x => darkening.weight = x, 0f, 0.2f);
    }
}
