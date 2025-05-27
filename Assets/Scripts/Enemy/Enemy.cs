using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamage
{
    [Header("References")]
    [SerializeField] private Renderer model; // Reference to the enemy's model for VFX
    [SerializeField] protected NavMeshAgent agent; // Controls pathfinding
    [SerializeField] protected Collider detectionField; // Trigger for basic range detection
    [SerializeField] protected CapsuleCollider collisionField; // Adjusted based on killable state
    [SerializeField] public Transform boneToFollow; // Used to adjust collision during killable state
    [SerializeField] protected Transform escort; // Escort target reference
    [SerializeField] private LayerMask viewMask; // Layer mask for vision raycasts
    [SerializeField] private LayerMask obstacleMask; // Layer mask to check line of sight

    [Header("Health")]
    [SerializeField] protected int health; // Current health
    protected int maxHealth; // Maximum health
    private bool isDying = false; // Flag to prevent further action while dying
    public bool isKillable = false; // Flag when enemy is in killable state
    public int essencePerKill = 2; // Award for killing
    [SerializeField] private ParticleSystem vanquishParticles; // Death effect
    [SerializeField] private float regenTime = 7.5f; // Time to regenerate out of killable state

    [Header("Targeting")]
    [SerializeField] private float faceTargetSpeed = 5f; // Speed to rotate toward target
    [SerializeField] private float fov = 90f; // Field of view for detection
    private Vector3 targetDir; // Direction to current target
    private Vector3 colliderDefaultPosition; // Stored collider position
    private int colliderDefaultDirection; // Stored collider direction
    protected Animator animator; // Animator controller
    protected bool playerInRange; // Simple range check
    protected Transform currentTarget; // Current chase/attack target
    private float timeSincePlayerHit = Mathf.Infinity; // Timer for switching between targets
    [SerializeField] private float escortVisionRange = 25f; // Escort detection range
    public bool ignoreEscort = false; // Optional override

    [Header("Combat")]
    [SerializeField] private float attackRange = 2.5f; // Melee attack distance
    [SerializeField] private int attackDamage = 10; // Damage per hit
    [SerializeField] protected float attackCooldown = 1.5f; // Time between attacks
    protected float timeSinceLastAttack = 0f; // Attack timer

    protected virtual void Start()
    {
        GameManager.instance.UpdateGameGoal(1);
        animator = GetComponent<Animator>();
        colliderDefaultPosition = collisionField.center;
        colliderDefaultDirection = collisionField.direction;
        maxHealth = health;

        if (escort == null)
        {
        //    Escort escortRef = Object.FindFirstObjectByType<SomeType>()
;
        //    if (escortRef != null)
        //        escort = escortRef.transform;
        }

        currentTarget = escort;
    }

    protected virtual void Update()
    {
        if (isKillable || isDying) return;

        timeSincePlayerHit += Time.deltaTime;
        timeSinceLastAttack += Time.deltaTime;

        Transform player = GameManager.instance.player.transform;

        if (timeSincePlayerHit < 7f)
        {
            if (currentTarget != player)
            {
                currentTarget = player;
                Debug.Log($"{gameObject.name} switches to PLAYER due to damage.");
            }
        }
        else if (currentTarget == player)
        {
            currentTarget = escort;
            Debug.Log($"{gameObject.name} switches back to ESCORT after cooldown.");
        }

        if (currentTarget == null) currentTarget = escort;

        if (agent != null && currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
            targetDir = agent.nextPosition - transform.position;
        }

        Locomotion();
        Behavior();

        if (timeSinceLastAttack >= attackCooldown)
        {
            Attack();
        }
    }

    void LateUpdate()
    {
        if (!isKillable) return;

        Vector3 boneLocation = boneToFollow.position;
        collisionField.center = transform.InverseTransformPoint(boneLocation);
        collisionField.direction = 2; // Y-axis
    }

    void CanSeePlayer()
    {
        playerInRange = detectionField.bounds.Contains(GameManager.instance.player.transform.position);
    }

    protected bool CanSeeTarget(string tag)
    {
        GameObject targetObj = GameObject.FindGameObjectWithTag(tag);
        if (targetObj == null) return false;

        Vector3 dir = targetObj.transform.position - transform.position;
        float angleToTarget = Vector3.Angle(new Vector3(dir.x, 0, dir.z), transform.forward);

        if (angleToTarget > fov) return false;

        if (Physics.Raycast(boneToFollow.position, dir.normalized, out RaycastHit hit, Mathf.Infinity, ~viewMask))
        {
            return hit.collider.CompareTag(tag);
        }

        return false;
    }

    private bool EscortVisible()
    {
        if (escort == null) return false;

        Vector3 dirToEscort = escort.position - transform.position;
        float dist = dirToEscort.magnitude;

        if (dist > escortVisionRange) return false;

        Vector3 rayOrigin = boneToFollow.position + Vector3.up * 0.5f;
        Debug.DrawRay(rayOrigin, dirToEscort.normalized * dist, Color.red, 0.1f);

        if (Physics.Raycast(rayOrigin, dirToEscort.normalized, out RaycastHit hit, dist, ~obstacleMask))
        {
            return hit.transform.CompareTag("Escort");
        }

        return false;
    }

    public virtual void TakeDamage(int amount)
    {
        if (isKillable) return;

        timeSincePlayerHit = 0f;

        if (currentTarget != GameManager.instance.player.transform)
        {
            currentTarget = GameManager.instance.player.transform;
            Debug.Log($"{gameObject.name} was hit and now targets PLAYER.");
        }

        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        if (agent.enabled)
            agent.SetDestination(currentTarget.position);

        if (health == 0)
        {
            BecomeKillable();
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    IEnumerator FlashRed()
    {
        yield return new WaitForSeconds(0.1f);
    }

    void BecomeKillable()
    {
        isKillable = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.enabled = false;
        animator.SetTrigger("Killable");
        StartCoroutine(UnbecomeKillable());
    }

    IEnumerator UnbecomeKillable()
    {
        yield return new WaitForSeconds(regenTime);
        if (!isDying)
            animator.SetTrigger("Unkillable");
    }

    public void UnkillableFinish()
    {
        ResetCollisionField();
        isKillable = false;
        agent.enabled = true;
        agent.isStopped = false;
        health = maxHealth / 2;
    }

    public void StartDeath()
    {
        isDying = true;
    }

    public IEnumerator Vanquish()
    {
        vanquishParticles.Play();
        animator.speed = 0;
        model.enabled = false;
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(vanquishParticles.main.duration);
        Destroy(gameObject);
    }

    void ResetCollisionField()
    {
        collisionField.direction = colliderDefaultDirection;
        collisionField.center = colliderDefaultPosition;
    }

    public virtual void Behavior() { }

    protected void SetPlayerAsTarget()
    {
        agent.SetDestination(GameManager.instance.player.transform.position);
    }

    protected void FaceTarget()
    {
        if (targetDir == Vector3.zero) return;
        Quaternion rot = Quaternion.LookRotation(new Vector3(targetDir.x, 0, targetDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public virtual void Attack()
    {
        if (currentTarget == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
        if (distanceToTarget <= attackRange)
        {
            IDamage damageable = currentTarget.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
                Debug.Log($"{gameObject.name} attacks {currentTarget.name}");
                timeSinceLastAttack = 0f; // Reset cooldown
            }
        }
    }

    protected Transform GetCurrentTarget()
    {
        return currentTarget;
    }

    void Locomotion()
    {
        float moveValue = animator.GetFloat("Move Speed");
        moveValue = Mathf.Lerp(moveValue, agent.velocity.magnitude / agent.speed, 0.1f);
        animator.SetFloat("Move Speed", moveValue);
    }
}