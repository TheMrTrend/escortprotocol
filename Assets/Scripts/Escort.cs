using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Escort : MonoBehaviour, IDamage
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Follow Settings")]
    [SerializeField] private float followdistance = 7f;
    [SerializeField] private float stopDistance = 6f;

    [Header("Health Settings")]
    public int maxHealth = 100;
    [System.NonSerialized] public int currentHealth;

    private bool isDead = false;

    [System.NonSerialized] public UnityEvent escortHealthUpdated;

   public  bool isFollowing = false;

    void Awake()
    {
        escortHealthUpdated = new UnityEvent();
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null) player = foundPlayer.transform;
        }

        if (agent == null) agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (!isFollowing) return;
        if (isDead || player == null) return;

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

    public void StartFollow()
    {
        isFollowing = true;
    }

    public void StopFollow()
    {
        isFollowing = false;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        escortHealthUpdated.Invoke();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        agent.isStopped = true;

        if (animator != null)
            animator.SetTrigger("Death");
        GameManager.instance.LoseState();
    }

    public bool IsDead()
    {
        return isDead;
    }
}