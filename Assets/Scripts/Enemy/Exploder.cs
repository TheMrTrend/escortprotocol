using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Exploder : Enemy
{
    [Header("Detection Settings")]
    [SerializeField] float detectionRange = 10f;                                                                                // RANGE AT WHICH PLAYER IS SPOTTED

    [Header("Charge & Explosion Settings")]
    [SerializeField] float chargeSpeed = 8f;                                                                                    // SPEED WHEN CHARGING
    [SerializeField] float explosionDelay = 2f;                                                                                 // TIME TO WAIT BEFORE EXPLODING
    [SerializeField] float explosionRadius = 4f;                                                                                // AREA OF EXPLOSION
    [SerializeField] float explosionDamage = 50f;                                                                               // DAMAGE TO PLAYER
    [SerializeField] LayerMask playerLayer;                                                                                     // LAYER FOR DETECTING PLAYER
    [SerializeField] LayerMask knockbackLayers;                                                                                 // LAYERS FOR KNOCKBACK (LIKE ZOMBIES)

    [Header("FX")]
    [SerializeField] GameObject explosionVFX;                                                                                   // PARTICLE EFFECT TO SPAWN
    [SerializeField] AudioClip explosionSFX;                                                                                    // SOUND TO PLAY ON EXPLOSION
    [SerializeField] AudioSource audioSource;                                                                                   // AUDIO SOURCE ON THE PREFAB
    [SerializeField] AudioClip footstepClip;                                                                                    // AUDIO CLIP FOR FOOTSTEPS
    [SerializeField] AudioClip screamClip;                                                                                      // AUDIO CLIP FOR SCREAMING


    private Vector3 roamTarget;                                                                                                 // CURRENT ROAM DESTINATION
    private Vector3 lockedPlayerPosition;                                                                                       // STORED PLAYER LOCATION WHEN DETECTED
    private bool playerDetected = false;                                                                                        // TRUE IF PLAYER HAS BEEN SEEN
    private bool isExploding = false;                                                                                           // TRUE IF CURRENTLY EXPLODING

    protected void Start()
    {                                                                                  
        PickNewRoamTarget();                                                                                                    // PICK INITIAL ROAM POINT
    }

    public override void Behavior()
    {
        if (isExploding) return;                                                                                                // IGNORE BEHAVIOR WHILE EXPLODING

        float distToPlayer = Vector3.Distance(GameManager.instance.player.transform.position, transform.position);

                                                                                                                                // PLAYER ENTERED DETECTION RANGE
        if (!playerDetected && distToPlayer <= detectionRange)
        {
            playerDetected = true;
            lockedPlayerPosition = GameManager.instance.player.transform.position;                                              // LOCK PLAYER LOCATION
            StartCoroutine(ChargeSequence());                                                                                   // BEGIN CHARGE SEQUENCE
            return;
        }

        if (!playerDetected)
        {
            Roam();                                                                                                             // WANDER AROUND IF PLAYER NOT SEEN
        }

        animator.SetFloat("Move Speed", agent.velocity.magnitude);                                                              // UPDATE SPEED PARAMETER FOR BLEND TREE
    }

    public void PlayFootstep()                                                                                                  // PLAY FOOTSTEP SOUND
    {
        if (audioSource && footstepClip)
            audioSource.PlayOneShot(footstepClip);
    }

    public void PlayScream()                                                                                                    // PLAY SCREAM SOUND
    {
        if (audioSource && screamClip)
            audioSource.PlayOneShot(screamClip);
    }

    void Roam()                                                                                                                 // WANDER AROUND RANDOMLY
    {
        if (Vector3.Distance(transform.position, roamTarget) < 1f)
        {
            PickNewRoamTarget();                                                                                                // GET A NEW DESTINATION WHEN CLOSE ENOUGH
        }

        agent.SetDestination(roamTarget);                                                                                       // MOVE TOWARD TARGET
    }

    void PickNewRoamTarget()
    {
        Vector3 randomDir = Random.insideUnitSphere * 8f;                                                                       // GET RANDOM DIRECTION
        randomDir += transform.position;

        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, 8f, NavMesh.AllAreas))
        {
            roamTarget = hit.position;                                                                                          // SET A VALID NAVMESH TARGET
        }
    }

    IEnumerator ChargeSequence()
    {
        agent.isStopped = true;                                                                                                 // STOP TO SCREAM
        animator.SetTrigger("Scream");                                                                                          // TRIGGER SCREAM ANIMATION
        PlayScream();                                                                                                           // PLAY SCREAM SOUND

        yield return new WaitForSeconds(0.8f);                                                                                  // WAIT FOR ANIMATION TO PLAY

        agent.isStopped = false;
        agent.speed = chargeSpeed;                                                                                              // INCREASE SPEED
        agent.SetDestination(lockedPlayerPosition);                                                                             // MOVE TO WHERE PLAYER WAS
        animator.SetFloat("Move Speed", 1f);                                                                                         // ENTER RUN STATE

                                                                                                                                // WAIT UNTIL NEAR TARGET POSITION
        while (Vector3.Distance(transform.position, lockedPlayerPosition) > 1.5f)
        {
            yield return null;
        }

        StartCoroutine(Explode());                                                                                              // BEGIN EXPLOSION SEQUENCE
    }

    IEnumerator Explode()
    {
        isExploding = true;
        agent.isStopped = true;
        animator.SetFloat("Move Speed", 0f);
        animator.SetTrigger("Explode");                                                                                         // PLAY EXPLOSION ANIMATION

        yield return new WaitForSeconds(explosionDelay);                                                                        // SHORT DELAY BEFORE BOOM

        if (explosionVFX)
            Instantiate(explosionVFX, transform.position, Quaternion.identity);                                                 // SPAWN VFX

        if (audioSource && explosionSFX)
            audioSource.PlayOneShot(explosionSFX);                                                                              // PLAY SOUND

        // DAMAGE PLAYER
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, playerLayer);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                GameManager.instance.playerController.TakeDamage((int)explosionDamage);                                         // APPLY DAMAGE
            }
        }

                                                                                                                                // KNOCK BACK OTHER OBJECTS (ZOMBIES, ETC.)
        Collider[] knockTargets = Physics.OverlapSphere(transform.position, explosionRadius, knockbackLayers);
        foreach (Collider other in knockTargets)
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                Vector3 force = (other.transform.position - transform.position).normalized * 10f;
                rb.AddForce(force, ForceMode.Impulse);                                                                          // APPLY FORCE OUTWARD
            }
        }

        Destroy(gameObject);                                                                                                    // DESTROY SELF
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);                                                             // VISUALIZE BLAST RADIUS
    }

}
