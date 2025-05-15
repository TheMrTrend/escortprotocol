using UnityEngine;
using UnityEngine.AI;

public class Spitter : Enemy
{
    [SerializeField] Collider spitRange;
    bool isAttacking;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] LayerMask ignoreLayers;
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
        if (isAttacking)
        {
            Vector3 target = GameManager.instance.player.transform.position - transform.position;
            target.y = 0f;
            Quaternion targetRot = Quaternion.LookRotation(target);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 40f * Time.deltaTime);
        }
    }

    void StartAttack()
    {

        isAttacking = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        animator.SetTrigger("Attack");
    }

    bool PlayerInReach()
    {
        return spitRange.bounds.Intersects(GameManager.instance.playerController.GetComponent<Collider>().bounds) ? true : false;
    }

    public void Attack()
    {
        Vector3 target = (GameManager.instance.player.transform.position - shootPos.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(shootPos.position, target, out hit, float.MaxValue, ~ignoreLayers))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Quaternion rot = Quaternion.LookRotation(target);
                Instantiate(bullet, shootPos.transform.position, rot);
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
