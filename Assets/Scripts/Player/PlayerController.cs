using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] CameraController cameraController;
    [SerializeField] LayerMask ignoreLayers;

    [SerializeField] float speed;
    [SerializeField] float sprintModifier;

    [SerializeField] float gravity;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpForce;

    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDistance;

    [SerializeField] int stabDistance;

    public int health;
    [System.NonSerialized] public int maxHealth;
    public int essence = 0;
    public int maxEssence = 50;


    Vector3 moveDir;
    Vector3 playerVel;

    bool isSprinting;
    int jumpCount;

    float shootTimer;

    [SerializeField] string objective;

    [System.NonSerialized] public UnityEvent healthUpdatedEvent;
    [System.NonSerialized] public UnityEvent essenceUpdated;
    [System.NonSerialized] public UnityEvent objectiveUpdated;
    [System.NonSerialized] public UnityEvent interact;
    [System.NonSerialized] public UnityEvent dialogue;


    [SerializeField] HeldItem held;

    public bool movementLocked = false;


    private void Awake()
    {
        healthUpdatedEvent = new UnityEvent();
        essenceUpdated = new UnityEvent();
        objectiveUpdated = new UnityEvent();
        interact = new UnityEvent();
        dialogue = new UnityEvent();
    }

    private void OnEnable()
    {
        maxHealth = health;
        
    }

    private void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);
        SlotSelection();
        Sprint();
        Movement();
    }

    void SlotSelection()
    {
        if (Input.GetButtonDown("Slot 1"))
        {
            held.SetCurrentItem(0);
        } else if (Input.GetButtonDown("Slot 2"))
        {
            held.SetCurrentItem(1);
        } else if (Input.GetButton("Slot 3"))
        {
            held.SetCurrentItem(2);
        }
    }

    void Movement()
    {
        shootTimer += Time.deltaTime;

        if (controller.isGrounded && jumpCount != 0)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        if (!movementLocked)
        {
            controller.Move(moveDir * speed * Time.deltaTime);
            Jump();
        }
        

        

        playerVel.y -= gravity * Time.deltaTime;

        if (Input.GetButtonDown("Interact"))
        {
            interact.Invoke();
        }

        if (Input.GetButtonDown("Dialogue"))
        {
            dialogue.Invoke();
        }
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintModifier;
            isSprinting = true;
        } else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintModifier;
            isSprinting = false;
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpForce;
        } 
    }

    public void TakeDamage(int amount)
    {
        if (movementLocked) { return; }
        health -= amount;
        healthUpdatedEvent.Invoke();
        if (health <= 0 )
        {
            GameManager.instance.LoseState();
        }
    }

    public void UpdateObjective(string newObjective)
    {
        objective = newObjective;
        objectiveUpdated.Invoke();
    }

    public void EnemyLockOn(Enemy enemy)
    {
        Vector3 camPos = cameraController.transform.position;
        Vector3 raisedEnemyPosition = new Vector3(enemy.transform.position.x, enemy.transform.position.y + (enemy.GetComponent<CapsuleCollider>().height/1.7f), enemy.transform.position.z);
        Vector3 enemyDir = (raisedEnemyPosition - camPos).normalized;

        float yawDegrees = Mathf.Atan2(enemyDir.x, enemyDir.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, yawDegrees, 0f), 0.2f);

        float pitchDegrees = -Mathf.Asin(enemyDir.y) * Mathf.Rad2Deg;

        Vector3 cameraEuler = cameraController.transform.localEulerAngles;
        cameraEuler.x = Mathf.Lerp(cameraEuler.x, pitchDegrees, 0.2f);
        cameraController.transform.localEulerAngles = cameraEuler;
    }

    public void AddEssence(int amount)
    {
        essence += amount;
        essence = Mathf.Clamp(essence, 0, maxEssence);
        essenceUpdated.Invoke();
    }
}
