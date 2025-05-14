using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

public class Enemy : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] protected NavMeshAgent agent;

    [SerializeField] int health;
    int maxHealth;
    [SerializeField] int faceTargetSpeed;

    protected Animator animator;

    Color originalColor;
    protected bool playerInRange;
    Vector3 targetDir;

    [SerializeField] float regenTime = 7.5f;
    [SerializeField] ParticleSystem vanquishParticles;
    [System.NonSerialized] public bool isKillable = false;
    public int essencePerKill = 2;

    bool isDying = false;
    void Start()
    {
        //originalColor = model.material.color;
        GameManager.instance.UpdateGameGoal(1);
        animator = GetComponent<Animator>();
        maxHealth = health;
    }

    void Update()
    {
        
        if (isKillable || isDying) return;
        if (agent != null && agent.destination != null)
        {
            targetDir = agent.nextPosition - transform.position;
        }
        Locomotion();
        //FaceTarget();
        Behavior();
    }
    protected void SetPlayerAsTarget()
    {
        agent.SetDestination(GameManager.instance.player.transform.position);
    }

    void Locomotion()
    {
        float value = animator.GetFloat("Move Speed");
        value = Mathf.Lerp(value, agent.velocity.magnitude / agent.speed, 0.1f);
        animator.SetFloat("Move Speed", value);
    }
    public virtual void Behavior() { }

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

    protected void FaceTarget()
    {
        if (targetDir == null) return;
        Quaternion rot = Quaternion.LookRotation(new Vector3(targetDir.x, transform.position.y, targetDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void TakeDamage(int amount)
    {
        if (isKillable) return;
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        Debug.Log("Heal is now " + health);
        agent.SetDestination(GameManager.instance.player.transform.position);
        if (health == 0)
        {
            BecomeKillable();
        } else
        {
            StartCoroutine(FlashRed());
        }
    }

    IEnumerator FlashRed()
    {
        //model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        //model.material.color = originalColor;
    }

    void BecomeKillable()
    {
        isKillable = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        //model.material.color = Color.red;
        StartCoroutine(UnbecomeKillable());
        animator.SetTrigger("Killable");
    }

    IEnumerator UnbecomeKillable()
    {
        yield return new WaitForSeconds(regenTime);
        animator.SetTrigger("Unkillable");
    }

    public void UnkillableFinish()
    {
        isKillable = false;
        agent.isStopped = false;
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
