using UnityEngine;
using UnityEngine.AI;

public class Spitter : Enemy
{
    [Header("Spitter Settings")]
    [SerializeField] private Collider spitRange;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform shootPos;
    [SerializeField] private LayerMask ignoreLayers;
 
    private bool isAttacking;

    public override void Behavior()
    {
        timeSinceLastAttack += Time.deltaTime;

        Transform target = GetCurrentTarget();
        if (target == null) return;

        // Retarget player if visible
        if (playerInRange || CanSeeTarget("Player"))
        {
            SetPlayerAsTarget();
        }

        if (!isAttacking && TargetInReach(target) && timeSinceLastAttack >= attackCooldown)
        {
            StartAttack();
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
        if (target == null || spitRange == null) return false;

        Collider col = target.GetComponent<Collider>();
        return col != null && spitRange.bounds.Intersects(col.bounds);
    }

    private void StartAttack()
    {
        isAttacking = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        animator.SetTrigger("Attack");
    }

    protected override void Attack()
    {
        if (isKillable) return;

        Transform target = GetCurrentTarget();
        if (target == null) return;

        timeSinceLastAttack = 0f;

        Vector3 dir = (target.position - shootPos.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(shootPos.position, dir, out hit, Mathf.Infinity, ~ignoreLayers))
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