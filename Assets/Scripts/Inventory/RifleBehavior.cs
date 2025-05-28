using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RifleBehavior : Item
{
    private Image reticle;

    bool canAttack = true;
    public LayerMask ignoreLayers;
    public int shootDamage = 10;

    public float maxSpread = 5f;
    public float spreadPerShot = 1f;
    public float spreadRecovery = 0.7f;

    float currentSpread;

    public Transform shootPos;
    public TrailRenderer bulletTrail;
    public ParticleSystem impactParticles;
    
    public float fireRate = 0.1f; 
    private float lastShotTime;

    public override void PrimaryHeld()
    {
        if (Time.time - lastShotTime >= fireRate && canAttack && currentAmmo > 0)
        {
            animator.SetBool("Is Shooting", true);
            Attack();
            lastShotTime = Time.time;
        }
        else if (currentAmmo == 0)
        {
            FinishAttack();
        }
    }

    public override void PrimaryRelease()
    {
        FinishAttack();
    }

    void FinishAttack()
    {
        animator.SetBool("Is Shooting", false);
        canAttack = true;
    }

    public override void Reload()
    {
        int ammoToGet = clipSize - currentAmmo;
        if (ammoToGet > storedAmmo)
        {
            currentAmmo += storedAmmo;
            storedAmmo = 0;
        }
        else
        {
            currentAmmo = clipSize;
            storedAmmo -= ammoToGet;
        }

        currentAmmoUpdated.Invoke(currentAmmo);
        storedAmmoUpdated.Invoke(storedAmmo);
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => UIManager.instance != null);

        currentSpread = maxSpread / 2f;
        FinishAttack();

        reticle = UIManager.instance.crosshairPistol;
        reticle.gameObject.SetActive(true);
        UIManager.instance.crosshairRifle.gameObject.SetActive(false);
        UIManager.instance.crosshairKnife.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (reticle != null)
            reticle.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (reticle != null && currentSpread > 0)
        {
            currentSpread = Mathf.Max(0, currentSpread - spreadRecovery * Time.deltaTime);
            float scale = Mathf.Lerp(1f, 1.4f, currentSpread / maxSpread);
            reticle.rectTransform.localScale = new Vector3(scale, scale, 1f);
        }
    }

    void Attack()
    {
        if (currentAmmo == 0) return;

        currentAmmo--;
        currentAmmoUpdated.Invoke(currentAmmo);
        currentSpread = Mathf.Min(maxSpread, currentSpread + spreadPerShot);

        float spreadRad = currentSpread * Mathf.Deg2Rad;
        float coneRad = Mathf.Tan(spreadRad);
        Vector2 rand = Random.insideUnitCircle * coneRad;

        Vector3 dir = (Camera.main.transform.forward + Camera.main.transform.right * rand.x + Camera.main.transform.up * rand.y).normalized;
        Debug.DrawRay(Camera.main.transform.position, dir * 10, Color.red, 1f);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, dir, out hit, float.MaxValue, ~ignoreLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject.TryGetComponent<IDamage>(out IDamage dmg))
            {
                dmg.TakeDamage(shootDamage);
            }

            TrailRenderer trail = Instantiate(bulletTrail, shootPos.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit));
        }

    }

    IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPos = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPos, hit.point, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }

        trail.transform.position = hit.point;
        Instantiate(impactParticles, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(trail.gameObject, trail.time);
    }

    public void AmmoCheck()
    {
        if (currentAmmo == 0)
        {
            FinishAttack();
        }
    }
}
