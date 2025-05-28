using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KnifeBehavior : Item
{
    private Image reticle;

    public float attackRate = 0.3f;       // Delay between individual stabs
    public float cooldownAfterCombo = 2f; // Delay after 3 stabs
    public int damage = 25;
    public float attackRange = 2f;
    public LayerMask hitLayers;
    public ParticleSystem impactEffect;

    private int stabCount = 0;
    private bool canAttack = true;
    private bool isCooldown = false;
    private float attackTimer = 0f;

    private void OnEnable()
    {
        ResetCombo();

        reticle = UIManager.instance.crosshairKnife;
        reticle.gameObject.SetActive(true);

        UIManager.instance.crosshairPistol.gameObject.SetActive(false);
        UIManager.instance.crosshairRifle.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (reticle != null)
            reticle.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!canAttack)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                canAttack = true;

                if (isCooldown)
                {
                    ResetCombo();
                }
            }
        }
    }

    public override void Primary()
    {
        if (!canAttack || isCooldown) return;

        PerformStab();
        stabCount++;

        if (stabCount >= 3)
        {
            isCooldown = true;
            attackTimer = cooldownAfterCombo;
        }
        else
        {
            attackTimer = attackRate;
        }

        canAttack = false;
    }

    void PerformStab()
    {
        animator.SetTrigger("Stab");

        RaycastHit hit;
        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;

        if (Physics.Raycast(origin, direction, out hit, attackRange, hitLayers))
        {
            if (hit.collider.TryGetComponent<IDamage>(out IDamage dmg))
            {
                dmg.TakeDamage(damage);
            }

            if (impactEffect != null)
            {
                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }

    void ResetCombo()
    {
        stabCount = 0;
        canAttack = true;
        isCooldown = false;
        attackTimer = 0f;
    }
}
