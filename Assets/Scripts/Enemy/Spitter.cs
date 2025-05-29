using UnityEngine;
using UnityEngine.AI;

public class Spitter : Enemy
{
    [Header("Spitter Settings")]
    [SerializeField] private Collider spitRange;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform shootPos;
    [SerializeField] private LayerMask hitLayers;
 
    private bool isAttacking;

    public override void Behavior()
    {

        Transform target = GetCurrentTarget();
        if (target == null) return;

        // Retarget player if visible
        if (currentTarget == GameManager.instance.player.transform)
        {
            agent.SetDestination(currentTarget.position);
        }
        // Otherwise, fall back to escort if visible
        else if (currentTarget == GameManager.instance.escort.transform)
        {
            agent.SetDestination(currentTarget.position);
        }

        if (!isAttacking && TargetInReach(target))
        {
            Quaternion desiredRot = Quaternion.LookRotation((target.position - transform.position).normalized);
            float angle = Quaternion.Angle(transform.rotation, desiredRot);

            if (angle < 10f) 
            {
                StartAttack();
            }
            
        }

        if (isAttacking && target != null)
        {
            Vector3 flatDir = target.position - transform.position;
            flatDir.y = 0f;
            Quaternion lookRot = Quaternion.LookRotation(flatDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, 40f * Time.deltaTime);
        }
    }

    private bool TargetInReach(Transform target)
    {
        if (target == null) return false;

        float dist = Vector3.Distance(transform.position, target.position);
        return dist <= 35.0f;
    }

    private void StartAttack()
    {
        isAttacking = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        animator.SetTrigger("Attack");
    }

    protected void Attack()
    {
        if (isKillable) return;

        Transform target = GetCurrentTarget();
        if (target == null) return;

        timeSinceLastAttack = 0f;

        Vector3 dir = (target.position - shootPos.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(shootPos.position, dir, out hit, Mathf.Infinity, hitLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Escort"))
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                Instantiate(bullet, shootPos.position, rot);
                Debug.Log($"{gameObject.name} spit at {hit.collider.name}");
            }
        }
    }

    public void FinishAttack()
    {
        if (agent.isActiveAndEnabled)
        {
            agent.isStopped = false;
        }

        isAttacking = false;
    }
}