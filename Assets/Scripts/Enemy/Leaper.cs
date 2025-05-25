using UnityEngine;
using UnityEngine.AI;

public class Leaper : Enemy
{
    [SerializeField] float leapSpeed = 10f;
    [SerializeField] float leapHeight = 5f;
    [SerializeField] float leapCooldown = 5f;
    [SerializeField] float damageRadius = 3f;
    [SerializeField] float damageAmount = 25f;
    [SerializeField] Transform leapOrigin;
    [SerializeField] LayerMask playerLayer;

    [Header("FX")]
    [SerializeField] GameObject landingDustPrefab;
    [SerializeField] AudioClip landingSound;
    [SerializeField] AudioSource audioSource;

    bool isLeaping;
    bool canLeap = true;

    public override void Behavior()
    {
        if (playerInRange)
        {
            SetPlayerAsTarget();
        }

        if (!isLeaping && canLeap && PlayerInLeapRange())
        {
            StartCoroutine(PerformLeap());
        }
    }

    bool PlayerInLeapRange()
    {
        float distance = Vector3.Distance(GameManager.instance.player.transform.position, transform.position);
        return distance > 5f && distance < 15f;
    }

    IEnumerator PerformLeap()
    {
        isLeaping = true;
        canLeap = false;

        agent.enabled = false;
        animator.SetTrigger("Leap");

        Vector3 start = transform.position;
        Vector3 end = GameManager.instance.player.transform.position;
        float duration = Vector3.Distance(start, end) / leapSpeed;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Arc motion
            Vector3 current = Vector3.Lerp(start, end, t);
            current.y += Mathf.Sin(t * Mathf.PI) * leapHeight;

            transform.position = current;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;

        // Apply damage if player is nearby
        ApplyLeapDamage();

        // Play dust FX
        if (landingDustPrefab)
            Instantiate(landingDustPrefab, leapOrigin.position, Quaternion.identity);

        // Play sound
        if (audioSource && landingSound)
            audioSource.PlayOneShot(landingSound);

        // Cooldown and re-enable nav agent
        yield return new WaitForSeconds(0.2f);
        agent.enabled = true;
        isLeaping = false;
        yield return new WaitForSeconds(leapCooldown);
        canLeap = true;
    }

    void ApplyLeapDamage()
    {
        Collider[] hits = Physics.OverlapSphere(leapOrigin.position, damageRadius, playerLayer);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                GameManager.instance.playerController.TakeDamage(damageAmount);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (leapOrigin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(leapOrigin.position, damageRadius);
        }
    }
}

