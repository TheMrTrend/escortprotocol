using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] float speed;
    [SerializeField] float sprintMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpForce;
    [SerializeField] int gravity;

    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    float shootTimer;

    int jumpCount;

    Vector3 moveDir;
    Vector3 playerVel;

    bool isSprinting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.darkOrange);
        movement();
        sprint();
    }

    void movement()
    {
        shootTimer += Time.deltaTime;

        if ((controller.isGrounded))
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }
        //Movement on character axis.
        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);

        //transform.position += moveDir * speed * Time.deltaTime; 
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Fire1") && shootTimer > shootRate)
            shoot();
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }
    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpForce;
        }
    }
    void shoot()
    {
        shootTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }
    }
}
