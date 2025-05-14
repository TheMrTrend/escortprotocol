using UnityEngine;
using UnityEngine.AI;

public class ZombieScientist : Enemy
{
    [SerializeField] Collider attackBox;
    bool isAttacking;
    int damagePerHit = 5;
    public override void Behavior()
    {
        if (playerInRange)
        {
            SetPlayerAsTarget();
        }
        if (!isAttacking && PlayerInReach())
        {
            StartAttack();
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        animator.SetBool("Follow Up", false);
        animator.SetTrigger("Attack");
    }

    bool PlayerInReach()
    {
        return attackBox.bounds.Intersects(GameManager.instance.playerController.GetComponent<Collider>().bounds) ? true : false;
    }

    public void Attack()
    {
        bool fU = animator.GetBool("Follow Up");
        animator.SetBool("Follow Up", !fU);
        if (PlayerInReach())
        {
            GameManager.instance.playerController.TakeDamage(damagePerHit);
        }
    }

    public void FollowUpCheck()
    {
        if (PlayerInReach())
        {
            animator.SetTrigger("Attack");
        }
    }

    public void FinishAttack()
    {
        isAttacking = false;
        agent.isStopped = false;
    }
}
