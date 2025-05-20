using UnityEngine;
using UnityEngine.AI;


//Controls the Escort Character
//Escort follows player and can take damage
public class Escort : MonoBehaviour
{
    [Header("references")]
    [SerializeField] private Transform player; //The target escort will follow
    [SerializeField] private NavMeshAgent agent; // NavMeshAgent for navigation
    [SerializeField] private Animator animator; //Animator for walk/death animations 

    //Following
    [Header("Follow Settings")]
    [SerializeField] private float followdistance = 6f; //Start following at this range of farther
    [SerializeField] private float stopDistance = 3f; //Stop approaching when this close
    [SerializeField] private float toggleRange = 3f; //max distance to toggle follow/stay
    //health
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100; //Escort max health
    private int currentHealth; //escort current health

    private bool isDead = false; //Is the escort dead?

    private bool isFollowing = true; //toggle wait/follow function
    //start
    private void Start()
    {
        //Find player by tag if notassigned
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null) player = foundPlayer.transform;
        }
        //Ensure the NavMeshAgent is assigned
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        //Start at full health
        currentHealth = maxHealth;
    }

    //following
    void Update()
    {
        //Dont do anything if dead or missing player reference
        if (isDead || player == null) return;
        //check for follow/stay key press and proximty
        if (Input.GetKeyDown(KeyCode.E))
        {
            float playerDistance = Vector3.Distance(transform.position, player.position);
            if (playerDistance <= toggleRange)
            {
                ToggleFollow();
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
                //stop and idle
                agent.isStopped = true;
                if (animator != null)
                    animator.SetFloat("Speed", 0f);
            }
        }
        //damage
        //called to apply famage to escort
        public void TakeDamage(int amount)
        {
            if (isDead) return;

            currentHealth = amount;

            if (currentHealth <= 0)
            {
                Die();
            }
        }
        //death behavior
        private void Die()
        {
            isDead = true;
            agent.isStopped = true;

            if (animator != null)
                animator.SetTrigger("Die");

            //optional addition: disable components or notify game manager
        }
        //toggle follow/wait mode when called via key press
        public void ToggleFollow()
        {
            isFollowing = !isFollowing;
            Debug.Log("Escort is now " + (isFollowing ? "following." : "waiting."));
        }
        //public check if escort is dead.
        public bool IsDead()
        {
            return isDead;
        }
    }
