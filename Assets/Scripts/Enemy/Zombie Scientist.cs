using UnityEngine;
using UnityEngine.AI;

public class ZombieScientist : Enemy
{
    [SerializeField] private Collider attackBox;
    [SerializeField] private int damagePerHit = 5;
    private bool isAttacking = false;

    public override void Behavior()
    {
        timeSinceLastAttack += Time.deltaTime;

        /*Transform target = GetCurrentTarget();
        if (target == null) return;*/

        // Retarget player only if they're visible or in range
        if (currentTarget == GameManager.instance.player.transform)
        {
            agent.SetDestination(currentTarget.position);
        }
        // Otherwise, fall back to escort if visible
        else if (currentTarget == GameManager.instance.escort.transform)
        {
            agent.SetDestination(currentTarget.position);
        } 
        
        if (!isAttacking && TargetInReach(currentTarget))
        {
            StartAttack();
        }
    }

    private bool TargetInReach(Transform target)
    {
        if (target == null || attackBox == null) return false;

        Collider targetCol = target.GetComponent<Collider>();
        if (targetCol == null || targetCol.isTrigger) return false;

        return attackBox.bounds.Intersects(targetCol.bounds);
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

    protected void Attack()
    {
        if (isKillable) return;

        Transform target = GetCurrentTarget();
        if (target == null) return;


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