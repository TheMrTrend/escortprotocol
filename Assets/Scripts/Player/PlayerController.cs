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

    [SerializeField] string objective;

    [System.NonSerialized] public UnityEvent healthUpdatedEvent;
    [System.NonSerialized] public UnityEvent essenceUpdated;
    [System.NonSerialized] public UnityEvent objectiveUpdated;
    [System.NonSerialized] public UnityEvent interact;
    [System.NonSerialized] public UnityEvent dialogue;


    [SerializeField] HeldItem held;

    public bool movementLocked = false;

    [SerializeField] float viewBobFrequency = 1.0f;
    [SerializeField] float viewBobAmplitude = 1.0f;
    float bobDelta;
    Vector3 cameraOrigin;
    Vector3 heldItemOrigin;

    public bool hasKeyCard = false;

    private void Awake()
    {
        healthUpdatedEvent = new UnityEvent();
        essenceUpdated = new UnityEvent();
        objectiveUpdated = new UnityEvent();
        interact = new UnityEvent();
        dialogue = new UnityEvent();
        cameraOrigin = cameraController.gameObject.transform.localPosition;
        heldItemOrigin = held.gameObject.transform.localPosition;
    }

    private void OnEnable()
    {
        maxHealth = health;
        
    }

    public void AddHealth(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        healthUpdatedEvent.Invoke();
    }
    private void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);
        SlotSelection();
        Sprint();
        Movement();
        ViewBobbing();
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
        if (controller.isGrounded && jumpCount != 0)
        {
            jumpCount = 0;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        playerVel.y -= gravity * Time.deltaTime;
        playerVel = new Vector3(moveDir.x * speed, playerVel.y, moveDir.z * speed);
        if (!movementLocked)
        {
            controller.Move(playerVel * Time.deltaTime);
            Jump();
        }
        

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
        Vector3 raisedEnemyPosition = enemy.boneToFollow.position;
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

    public void AddAmmo(int amount, ResourceType type)
    {
        for (int i = 0; i < held.items.Count; i++)
        {
            if (held.items[i].ammoType == type)
            {
                held.items[i].storedAmmo += amount;
                if (held.items[i] == held.currentItem)
                {
                    held.items[i].storedAmmoUpdated.Invoke(held.items[i].storedAmmo);
                }
            }
        }
    }

    void ViewBobbing()
    {
        if ( (controller.velocity.x != 0 || controller.velocity.z != 0) && controller.isGrounded)
        {
            bobDelta += Time.deltaTime * controller.velocity.magnitude;
            Camera.main.transform.localPosition = cameraOrigin + HeadViewBob(bobDelta);
            held.transform.localPosition = heldItemOrigin + ItemViewBob(bobDelta);
        } else
        {
            Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, cameraOrigin, 0.1f * Time.deltaTime);
            held.transform.localPosition = Vector3.Lerp(held.transform.localPosition, heldItemOrigin, 0.1f * Time.deltaTime);
        }
    }

    Vector3 HeadViewBob(float t)
    {
        Vector3 pos = Vector3.zero;
        pos.y = Mathf.Sin(t * viewBobFrequency) * viewBobAmplitude;
        pos.x = Mathf.Cos(t * viewBobFrequency / 2.1f) * viewBobAmplitude;
        return pos;
    }

    Vector3 ItemViewBob(float t)
    {
        Vector3 pos = Vector3.zero;
        pos.y = Mathf.Sin(t * viewBobFrequency) * viewBobAmplitude/5f;
        pos.x = -Mathf.Cos(t * viewBobFrequency / 2.1f) * viewBobAmplitude/5f;
        return pos;
    }
}
