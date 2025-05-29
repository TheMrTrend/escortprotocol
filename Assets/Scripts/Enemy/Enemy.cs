using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamage
{
    [Header("References")]
    [SerializeField] public Renderer model;
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] protected Collider detectionField;
    [SerializeField] protected CapsuleCollider collisionField;
    [SerializeField] public Transform boneToFollow;
    [SerializeField] protected Transform escort;
    [SerializeField] private LayerMask viewMask;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Health")]
    [SerializeField] protected int health;
    private int maxHealth;
    protected bool isDying = false;
    public bool isKillable = false;
    public int essencePerKill = 2;
    [SerializeField] private ParticleSystem vanquishParticles;
    [SerializeField] private float regenTime = 7.5f;
    public int qteActions = 1;
    public float qteLength = 3f;
    Coroutine regenerationCoroutine;

    [Header("Targeting")]
    [SerializeField] private float faceTargetSpeed = 5f;
    [SerializeField] protected float fov = 90f;
    private Vector3 targetDir;
    private Vector3 colliderDefaultPosition;
    private int colliderDefaultDirection;
    protected Animator animator;
    protected bool playerInRange;
    public Transform currentTarget;
    private float timeSincePlayerHit = Mathf.Infinity;
    [SerializeField] private float escortVisionRange = 25f;
    public bool ignoreEscort = false;
    Vector3 roamTarget;
    Vector3 spawnPos;
    public bool navOverride = false;

    [Header("Combat")]
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] protected float attackCooldown = 1.5f; // Shared cooldown
    protected float timeSinceLastAttack = 0f;

    void Start()
    {
        GameManager.instance.UpdateGameGoal(1);
        animator = GetComponent<Animator>();
        colliderDefaultPosition = collisionField.center;
        colliderDefaultDirection = collisionField.direction;
        maxHealth = health;
        spawnPos = model.transform.position;
        if (escort == null)
        {
            Escort escortRef = FindObjectOfType<Escort>();
            if (escortRef != null)
                escort = escortRef.transform;
        }

        PickNewRoamTarget();
    }
    protected virtual void Update()
    {
        if (isKillable || isDying) return;
        timeSincePlayerHit += Time.deltaTime;
        if (navOverride)
        {
            if (currentTarget != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.SetDestination(currentTarget.position);
                Locomotion();
            }
            return;
        }
        //timeSinceLastAttack += Time.deltaTime;

        Transform player = GameManager.instance.player.transform;
        escort = escort.transform;

        bool playerVisible = CanSeeTarget("Player");
        bool escortVisible = CanSeeTarget("Escort");
        bool playerAggro = timeSincePlayerHit < 7f;
        bool escortFollowing = GameManager.instance.escort.isFollowing;

        Transform target = null;
        if (playerAggro && playerVisible)
        {
            target = player;
        }
        else if (escortVisible && escortFollowing)
        {
            target = escort;
        }
        else if (playerVisible) {
            target = player;
        }
        else
        {
            target = null;
        }

        currentTarget = target;
        if (currentTarget == null)
        {
            Roam();
        }
        // Switch to player if recently hit


        Locomotion();
        Behavior();

        /*if (timeSinceLastAttack >= attackCooldown)
        {
            Attack();
        }*/
    }

    void LateUpdate()
    {
        if (!isKillable) return;

        Vector3 boneLocation = boneToFollow.position;
        collisionField.center = transform.InverseTransformPoint(boneLocation);
        collisionField.direction = 2;
    }


    protected bool CanSeeTarget(string tag)
    {
        GameObject targetObj = GameObject.FindGameObjectWithTag(tag);
        
        if (targetObj == null) return false;
        Vector3 dir = new Vector3(targetObj.transform.position.x - model.transform.position.x, 0, targetObj.transform.position.z - model.transform.position.z);
        Vector3 forward = new Vector3(model.transform.forward.x, 0, model.transform.forward.z);
        float angleToTarget = Vector3.Angle(forward, dir);
        //Debug.Log("Angle to target " + tag + " is " + angleToTarget + " from " + name);
        if (angleToTarget > fov / 2f) return false;
        Vector3 rayDir = (targetObj.transform.position - boneToFollow.position).normalized;
        if (Physics.Raycast(boneToFollow.position, rayDir.normalized, out RaycastHit hit, Mathf.Infinity, viewMask, QueryTriggerInteraction.Ignore))
        {
            //Debug.Log("Ray hit: " + hit.collider.name + " from " + name);
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

    public void TakeDamage(int amount)
    {
        if (isKillable) return;

        timeSincePlayerHit = 0f;

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
        regenerationCoroutine = StartCoroutine(UnbecomeKillable());
    }

    IEnumerator UnbecomeKillable()
    {
        yield return new WaitForSeconds(regenTime);
        if (!isDying) {
            animator.SetTrigger("Unkillable");
            regenerationCoroutine = null;
        }
            
    }

    public void InstantRegeneration()
    {
        animator.SetTrigger("Unkillable");
        if (regenerationCoroutine != null)
        {
            StopCoroutine(regenerationCoroutine);
        }
        regenerationCoroutine = null;
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

    public void SetPlayerAsTarget()
    {
        agent.SetDestination(GameManager.instance.player.transform.position);
        currentTarget = GameManager.instance.player.transform;
    }

    protected void FaceTarget()
    {
        if (targetDir == Vector3.zero) return;
        Quaternion rot = Quaternion.LookRotation(new Vector3(targetDir.x, 0, targetDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    /*protected virtual void Attack()
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
                timeSinceLastAttack = 0f; // Reset cooldown on successful attack
            }
        }
    }*/

    protected Transform GetCurrentTarget()
    {
        return currentTarget;
    }

    protected void Locomotion()
    {
        float moveValue = animator.GetFloat("Move Speed");
        moveValue = Mathf.Lerp(moveValue, agent.velocity.magnitude / agent.speed, 0.1f);
        animator.SetFloat("Move Speed", moveValue);
    }

    protected void Roam()                                                                                                                 // WANDER AROUND RANDOMLY
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                PickNewRoamTarget();
            } 
                                                                                                      // GET A NEW DESTINATION WHEN CLOSE ENOUGH
        }

        agent.SetDestination(roamTarget);                                                                                       // MOVE TOWARD TARGET
    }

    void PickNewRoamTarget()
    {
        Vector3 randomDir = Random.insideUnitSphere * 8f;                                                                       // GET RANDOM DIRECTION
        randomDir += spawnPos;

        if (NavMesh.SamplePosition(new Vector3(randomDir.x, agent.transform.position.y, randomDir.z), out NavMeshHit hit, 8f, NavMesh.AllAreas))
        {
            roamTarget = hit.position;                                                                                          // SET A VALID NAVMESH TARGET
        }
    }
}