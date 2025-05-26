using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Exploder : Enemy
{
    [SerializeField] float explosionRadius = 4f;
    [SerializeField] float explosionDamage = 50f;
    [SerializeField] float chargeSpeed = 8f;
    [SerializeField] float detectionRange = 12f;
    [SerializeField] float timeBeforeExplosion = 2f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask knockbackLayers;

    [Header("FX")]
    [SerializeField] GameObject explosionVFX;
    [SerializeField] AudioClip explosionSFX;
    [SerializeField] AudioSource audioSource;

    private Animator animator;
    private NavMeshAgent agent;
    private Vector3 targetPosition;
    private bool isExploding = false;
    private bool hasLockedOn = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    public override void Behavior()
    {
        if (isExploding) return;

        float distance = Vector3.Distance(GameManager.instance.player.transform.position, transform.position);

        if (distance <= detectionRange && !hasLockedOn)
        {
            hasLockedOn = true;
            targetPosition = GameManager.instance.player.transform.position;
            agent.speed = chargeSpeed;
            agent.SetDestination(targetPosition);

            // Start charging animation
            animator.SetFloat("Speed", 1f);
            animator.SetTrigger("Scream");
        }

        if (hasLockedOn && Vector3.Distance(transform.position, targetPosition) <= 1.5f)
        {
            StartCoroutine(ExplodeSequence());
        }
    }

    IEnumerator ExplodeSequence()
    {
        isExploding = true;
        agent.isStopped = true;
        animator.SetFloat("Speed", 0f);

        yield return new WaitForSeconds(timeBeforeExplosion);

        animator.SetTrigger("Explode");

        // Delay slightly for animation effect
        yield return new WaitForSeconds(0.15f);

        if (explosionVFX)
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        if (audioSource && explosionSFX)
            audioSource.PlayOneShot(explosionSFX);

        // Damage Player
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, playerLayer);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                GameManager.instance.playerController.TakeDamage((int)explosionDamage);
            }
        }

        // Knockback other enemies (like zombies)
        Collider[] others = Physics.OverlapSphere(transform.position, explosionRadius, knockbackLayers);
        foreach (Collider other in others)
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                Vector3 forceDir = (other.transform.position - transform.position).normalized;
                rb.AddForce(forceDir * 10f, ForceMode.Impulse);                                                                                     // Tweak force as needed
            }
        }

        Destroy(gameObject);                                                                                                                        // Destroys self
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}