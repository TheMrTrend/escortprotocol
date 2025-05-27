using UnityEngine;
using UnityEngine.AI;

public class ZombieScientist : Enemy
{
    [Header("Zombie Scientist Settings")]
    [SerializeField] private Collider attackBox;
    [SerializeField] private int damagePerHit = 5;

    private bool isAttacking = false;

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
        else if (!ignoreEscort && CanSeeTarget("Escort"))
        {
            currentTarget = escort;
        }

        if (!isAttacking && TargetInReach(target) && timeSinceLastAttack >= attackCooldown)
        {
            StartAttack();
        }
    }

    private bool TargetInReach(Transform target)
    {
        if (target == null || attackBox == null) return false;

        Collider targetCol = target.GetComponent<Collider>();
        return targetCol != null && attackBox.bounds.Intersects(targetCol.bounds);
    }

    private void StartAttack()
    {
        isAttacking = true;

        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        animator.SetBool("Follow Up", false);
        animator.SetTrigger("Attack");
    }

    public override void Attack()
    {
        if (isKillable || timeSinceLastAttack < attackCooldown) return;

        Transform target = GetCurrentTarget();
        if (target == null) return;

        timeSinceLastAttack = 0f;

        bool fU = animator.GetBool("Follow Up");
        animator.SetBool("Follow Up", !fU);

        if (TargetInReach(target))
        {
            IDamage damageable = target.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.TakeDamage(damagePerHit);
                Debug.Log($"{gameObject.name} attacks {target.name} for {damagePerHit}");
            }
        }
    }

    public void FollowUpCheck()
    {
        if (isKillable) return;

        Transform target = GetCurrentTarget();
        if (target != null && TargetInReach(target))
        {
            animator.SetTrigger("Attack");
        }
    }

    public void FinishAttack()
    {
        isAttacking = false;

        if (agent.enabled)
        {
            agent.isStopped = false;
        }
    }
}
