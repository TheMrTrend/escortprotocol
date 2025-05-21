using UnityEngine;
using UnityEngine.AI;

// Controls the Escort Character
// Escort follows player and can take damage
public class Escort : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player; // The target escort will follow
    [SerializeField] private NavMeshAgent agent; // NavMeshAgent for navigation
    [SerializeField] private Animator animator; // Animator for walk/death animations 

    // Following
    [Header("Follow Settings")]
    [SerializeField] private float followdistance = 6f; // Start following at this range or farther
    [SerializeField] private float stopDistance = 3f; // Stop approaching when this close
    [SerializeField] private float toggleRange = 3f; // Max distance to toggle follow/stay

    // Health
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100; // Escort max health
    private int currentHealth; // Escort current health

    private bool isDead = false; // Is the escort dead?
    private bool isFollowing = true; // Toggle wait/follow function

    // Start
    private void Start()
    {
        // Find player by tag if not assigned
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null) player = foundPlayer.transform;
        }

        // Ensure the NavMeshAgent is assigned
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        // Start at full health
        currentHealth = maxHealth;
    }

    // Following
    private void Update()
    {
        // Don't do anything if dead or missing player reference
        if (isDead || player == null) return;

        // Check for follow/stay key press and proximity
        if (Input.GetKeyDown(KeyCode.E))
        {
            float playerDistance = Vector3.Distance(transform.position, player.position);
            if (playerDistance <= toggleRange)
            {
                ToggleFollow();
            }
        }

        if (isFollowing)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance > followdistance)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else if (distance <= stopDistance)
            {
                agent.isStopped = true;
            }

            if (animator != null)
                animator.SetFloat("Speed", agent.velocity.magnitude);
        }
        else
        {
            // Stop and idle
            agent.isStopped = true;
            if (animator != null)
                animator.SetFloat("Speed", 0f);
        }
    }

    // Damage
    // Called to apply damage to escort
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Death behavior
    private void Die()
    {
        isDead = true;
        agent.isStopped = true;

        if (animator != null)
            animator.SetTrigger("Die");

        // Optional addition: disable components or notify game manager
    }

    // Toggle follow/wait mode when called via key press
    public void ToggleFollow()
    {
        isFollowing = !isFollowing;
        Debug.Log("Escort is now " + (isFollowing ? "following." : "waiting."));
    }

    // Public check if escort is dead
    public bool IsDead()
    {
        return isDead;
    }
}