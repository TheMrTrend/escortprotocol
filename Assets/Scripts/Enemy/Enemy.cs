using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int health;
    int maxHealth;
    [SerializeField] int faceTargetSpeed;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float fireRate;

    Animator animator;

    Color originalColor;
    float shootTimer;
    bool playerInRange;
    Vector3 playerDir;

    [SerializeField] float regenTime = 7.5f;
    [SerializeField] ParticleSystem vanquishParticles;
    [System.NonSerialized] public bool isKillable = false;
    public int essencePerKill = 2;

    bool isDying = false;
    void Start()
    {
        originalColor = model.material.color;
        GameManager.instance.UpdateGameGoal(1);
        animator = GetComponent<Animator>();
        maxHealth = health;
    }

    void Update()
    {
        
        shootTimer += Time.deltaTime;
        if (isKillable || isDying) return;
        if (playerInRange)
        {
            playerDir = GameManager.instance.player.transform.position - transform.position;
            agent.SetDestination(GameManager.instance.player.transform.position);

            if (shootTimer >= fireRate)
            {
                Shoot();
            }

            if (agent.remainingDistance < agent.stoppingDistance)
            {
                FaceTarget();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void Shoot()
    {
        shootTimer = 0;
        animator.SetTrigger("Attack");
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        agent.SetDestination(GameManager.instance.player.transform.position);
        if (health < 0)
        {
            BecomeKillable();
        } else
        {
            StartCoroutine(FlashRed());
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = originalColor;
    }

    void BecomeKillable()
    {
        isKillable = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        model.material.color = Color.red;
        StartCoroutine(UnbecomeKillable());
        animator.SetTrigger("Killable");
    }

    IEnumerator UnbecomeKillable()
    {
        yield return new WaitForSeconds(regenTime);
        isKillable = false;
        agent.isStopped = false;
        model.material.color = originalColor;
        animator.SetTrigger("Back");
        health = maxHealth / 2;
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
    public void StartDeath()
    {
        isDying = true;
    }


}
