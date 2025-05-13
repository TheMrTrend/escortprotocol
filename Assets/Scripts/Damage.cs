using System.Collections;
using UnityEngine;

enum DamageType
{
    MOVING, STATIONARY, DOT, HOMING
}

public class Damage : MonoBehaviour
{
    [SerializeField] DamageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int amount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    bool isDamaging;
    void Start()
    {
        if (type == DamageType.MOVING || type == DamageType.HOMING)
        {
            Destroy(gameObject, destroyTime);

            if (type == DamageType.MOVING)
            {
                rb.linearVelocity = transform.forward * speed;
            }
        }
    }

    void Update()
    {
        if (type == DamageType.HOMING)
        {
            rb.linearVelocity = (GameManager.instance.player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && (type == DamageType.MOVING || type == DamageType.STATIONARY || type == DamageType.HOMING))
        {
            dmg.TakeDamage(amount);
        }
        if (type == DamageType.MOVING || type == DamageType.HOMING)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger) {
            return;
        }
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && type == DamageType.DOT)
        {
            if (!isDamaging)
            {
                StartCoroutine(DamageOther(dmg));
            }
        }
    }

    IEnumerator DamageOther(IDamage dmg)
    {
        isDamaging = true;
        dmg.TakeDamage(amount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}
