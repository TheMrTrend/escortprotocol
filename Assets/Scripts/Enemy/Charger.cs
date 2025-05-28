using UnityEngine;
using UnityEngine.AI;

public class TankCharger : Enemy
{
    [Header("Charge Settings")]
    [SerializeField] private float chargeRange = 15f;
    [SerializeField] private float chargeSpeedMultiplier = 3f;
    [SerializeField] private float chargeDuration = 2f;
    [SerializeField] private float chargeCooldown = 6f;
    [SerializeField] private int chargeDamage = 20;
    [SerializeField] private Collider chargeHitbox;

    private bool isCharging = false;
    private float chargeTimer = 0f;
    private float timeSinceLastCharge = Mathf.Infinity;

    public override void Behavior()
    {
        if (isKillable || isCharging) return;

        timeSinceLastCharge += Time.deltaTime;

        Transform target = GetCurrentTarget();
        if (target == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        // Prioritize charging the player
        if (distanceToPlayer <= chargeRange && timeSinceLastCharge >= chargeCooldown)
        {
            StartCharge(GameManager.instance.player.transform);
        }
    }

    private void StartCharge(Transform target)
    {
        isCharging = true;
        timeSinceLastCharge = 0f;
        chargeTimer = chargeDuration;

        animator.SetTrigger("Charge");
        agent.speed *= chargeSpeedMultiplier;
        agent.SetDestination(target.position);
    }

    protected override void Update()
    {
        base.Update();

        if (isCharging)
        {
            chargeTimer -= Time.deltaTime;

            if (chargeTimer <= 0f)
            {
                EndCharge();
            }

            CheckChargeHit();
        }
    }

    private void EndCharge()
    {
        isCharging = false;
        agent.speed /= chargeSpeedMultiplier;
        animator.ResetTrigger("Charge");
    }

    private void CheckChargeHit()
    {
        if (chargeHitbox == null) return;

        Collider[] hits = Physics.OverlapBox(chargeHitbox.bounds.center, chargeHitbox.bounds.extents, chargeHitbox.transform.rotation);

        foreach (Collider col in hits)
        {
            IDamage target = col.GetComponent<IDamage>();
            if (target != null && col.transform != transform)
            {
                target.TakeDamage(chargeDamage);
                Debug.Log($"{gameObject.name} charges and hits {col.name} for {chargeDamage} damage!");
                EndCharge();
                break;
            }
        }
    }
}