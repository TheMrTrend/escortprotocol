using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamage
{
   
    // Component and target references
    

    [SerializeField] private Renderer model;                      // Enemy model for visual feedback
    [SerializeField] protected NavMeshAgent agent;                // Navigation/pathfinding
    [SerializeField] protected Collider detectionField;           // Proximity detection without needing raycast
    [SerializeField] protected CapsuleCollider collisionField;    // Collider used during killable state
    [SerializeField] protected Transform boneToFollow;            // Used to reposition collider on animations
    [SerializeField] private Transform escort;                    // Target Escort reference

    [SerializeField] private LayerMask viewMask;                  // Define layers to include/exclude in vision checks
    [SerializeField] private LayerMask obstacleMask;              // Mask used for LOS checks to Escort

    //Health and Damage

    [SerializeField] private int health;                          // Current health
    private int maxHealth;                                        // Max health value
    private bool isDying = false;                                 // Set when enemy starts death animation
    private bool isKillable = false;                              // Determines if the enemy can be killed after downed
    public int essencePerKill = 2;                                // Reward on kill

    [SerializeField] private ParticleSystem vanquishParticles;    // Death FX
    [SerializeField] private float regenTime = 7.5f;              // Time before respawn

    //Targeting and AI

    [SerializeField] private float faceTargetSpeed = 5f;          // Turn speed to face target
    [SerializeField] private float fov = 90f;                     // Field of view for visibility check

    private Vector3 targetDir;                                    // Direction to target
    private Vector3 colliderDefaultPosition;                      // Original position of killable collider
    private int colliderDefaultDirection;                         // Original collider direction

    protected Animator animator;                                  // Animation controller
    protected bool playerInRange;                                 // Is player in detection bounds

    private Transform currentTarget;                              // Who enemy currently chasing
    private float timeSincePlayerHit = Mathf.Infinity;            // Time since last hit by player,

    [SerializeField] private float switchToEscortAfter = 7f;      // Time delay to switch target to escort
    [SerializeField] private float escortVisionRange = 25f;       // Max range to "see" escort

    public bool ignoreEscort = false;                             // Phase toggle to ignore escort

    
    // Unity methods
    

    void Start()
    {
        // Tell gamemanager new enemy has spawned
        GameManager.instance.UpdateGameGoal(1);

        //Reference for movement/kill animations
        animator = GetComponent<Animator>();

        // Save position of killable collider
        colliderDefaultPosition = collisionField.center;
        colliderDefaultDirection = collisionField.direction;

        //set starting health
        maxHealth = health;

        // Auto-assign Escort if not set
        if (escort == null)
        {
            Escort escortRef = FindObjectOfType<Escort>();
            if (escortRef != null)
                escort = escortRef.transform;
        }

        // Default to targeting the player
        currentTarget = GameManager.instance.player.transform;
    }

    void Update()
    {
        //if enemy is killable skip AI logic
        if (isKillable || isDying) return;

        // check if player is inside detection collider
        CanSeePlayer();

        // Increase timer since last time playe hurt this enemy
        timeSincePlayerHit += Time.deltaTime;

        // Change target to escort if pllayer hasnt damaged enemy for set time
        if (!ignoreEscort && timeSincePlayerHit >= switchToEscortAfter && EscortVisible())
        {
            currentTarget = escort;
        }
        else
        {
            currentTarget = GameManager.instance.player.transform;
        }

        // Set destination toward current target
        if (agent != null && currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
            targetDir = agent.nextPosition - transform.position;
        }

        // Animate movement speed
        Locomotion();

        // Optional facing logic (disabled for now)
        // FaceTarget();

        // Run behavior override hook (used in subclasses)
        Behavior();
    }

    void LateUpdate()
    {
        //reposition collider only if enemy is in kilable state
        if (!isKillable) return;

        // Move collider to follow animation "bone" while killable
        Vector3 boneLocation = boneToFollow.position;
        collisionField.center = transform.InverseTransformPoint(boneLocation);
        collisionField.direction = 2; // Vertical
    }

    // VISIBILITY & DETECTION
    void CanSeePlayer()
    {
        // If player is in detection collider, flag them
        playerInRange = detectionField.bounds.Contains(GameManager.instance.player.transform.position);
    }

    protected bool CanSeeTarget(string tag)
    {
        // Get direction to target
        Vector3 dir = GameObject.FindWithTag(tag).transform.position - transform.position;
        
        // Get angle between forward direction and target
        float angleToTarget = Vector3.Angle(new Vector3(dir.x, 0, dir.z), transform.forward);
        
        //Raycast toward target and check for obstructions
        if (Physics.Raycast(boneToFollow.position, dir, out RaycastHit hit, Mathf.Infinity, ~viewMask))
        {
            //Confirm raycast hit correct target and is inside field of vision
            if (hit.collider.CompareTag(tag) && angleToTarget <= fov)
                return true;
        }

        return false;
    }

    private bool EscortVisible()
    {
        if (escort == null) return false;

        //Get direction and distance to escort
        Vector3 dirToEscort = escort.position - transform.position;
        float dist = dirToEscort.magnitude;

        //If escort is too far, dont check
        if (dist > escortVisionRange)
            return false;

        // Raycast to check for obstacles
        if (Physics.Raycast(boneToFollow.position, dirToEscort.normalized, out RaycastHit hit, dist, ~obstacleMask))
        {
            return hit.transform == escort;
        }

        return false;
    }

   // Combat and Damage

    public void TakeDamage(int amount)
    {
        if (isKillable) return;

        // Reset target priority back to player
        timeSincePlayerHit = 0f;

        //apply damage
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        // Immediately chase the player
        agent.SetDestination(GameManager.instance.player.transform.position);

        if (health == 0)
        {
            //enter killable state
            BecomeKillable();
        }
        else
        {
            //flash red if still alive
            StartCoroutine(FlashRed());
        }
    }

    IEnumerator FlashRed()
    {
        // Flash enemy red temporarily (disabled for now)
        // model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        // model.material.color = originalColor;
    }

    void BecomeKillable()
    {
        isKillable = true;

        //stop moving
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.enabled = false;

        //play killable animation
        animator.SetTrigger("Killable");

        // Start timer to revive if not killed
        StartCoroutine(UnbecomeKillable());
    }

    //called if the player doesn't finish the enemy in time
    IEnumerator UnbecomeKillable()
    {
        yield return new WaitForSeconds(regenTime);

        if (!isDying)
        {
            animator.SetTrigger("Unkillable");
        }
    }

   //enemy revives with partial health and resumes AI
    public void UnkillableFinish()
    {
        // Reset collider to original setup
        ResetCollisionField();
        isKillable = false;

        //Re-enable movement
        agent.enabled = true;
        agent.isStopped = false;
        // Revive with half health
        health = maxHealth / 2;
    }

    //handles enemy death and removal from the scene
    public void StartDeath()
    {
        isDying = true;
    }

    public IEnumerator Vanquish()
    {
        //play particles, freeze animation, and disable visuals
        vanquishParticles.Play();
        animator.speed = 0;
        model.enabled = false;
        GetComponent<Collider>().enabled = false;

        //Wait for effect tro finish before destroying
        yield return new WaitForSeconds(vanquishParticles.main.duration);
        Destroy(gameObject);
    }

    //Restore colider defaults after killable phase
    void ResetCollisionField()
    {
        collisionField.direction = colliderDefaultDirection;
        collisionField.center = colliderDefaultPosition;
    }

    // EXTENSION HOOKS

    public virtual void Behavior()
    {
        // Optional behavior override for subclasses (rankged, melee etc.)
    }

    protected void SetPlayerAsTarget()
    {
        // Retarget player manually (can be called from cutscenes or triggers)
        agent.SetDestination(GameManager.instance.player.transform.position);
    }

    protected void FaceTarget()
    {
        if (targetDir == Vector3.zero) return;

        //Rotate enemy to face current direction smoothly
        Quaternion rot = Quaternion.LookRotation(new Vector3(targetDir.x, 0, targetDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    //mixes movement speed into animations
    void Locomotion()
    {
        float moveValue = animator.GetFloat("Move Speed");
        
        //interpolate to new movement speed
        moveValue = Mathf.Lerp(moveValue, agent.velocity.magnitude / agent.speed, 0.1f);
        animator.SetFloat("Move Speed", moveValue);
    }
}