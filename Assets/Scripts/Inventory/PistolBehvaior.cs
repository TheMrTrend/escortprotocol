using System.Collections;
using UnityEngine;

public class PistolBehvaior : Item
{
    bool canAttack = true;
    public LayerMask ignoreLayers;
    public int shootDamage = 10;

    public float maxSpread = 3f;
    public float spreadPerShot = 0.7f;
    public float spreadRecovery = 2f;

    float currentSpread;

    public Transform shootPos;
    public TrailRenderer bulletTrail;
    public ParticleSystem impactParticles;
    public override void Primary()
    {
        if (canAttack && currentAmmo > 0)
        {
            canAttack = false;
            animator.SetTrigger("Fire");
        }
    }

    public override void Reload()
    {
        int ammoToGet = clipSize - currentAmmo;
        if (ammoToGet > storedAmmo)
        {
            currentAmmo += storedAmmo;
            storedAmmo = 0;

        } else
        {
            currentAmmo = clipSize;
            storedAmmo -= ammoToGet;
        }
        currentAmmoUpdated.Invoke(currentAmmo);
        storedAmmoUpdated.Invoke(storedAmmo);
    }

    private void OnEnable()
    {
        currentSpread = maxSpread;
    }

    private void Update()
    {
        if (currentSpread > 0)
        {
            currentSpread = Mathf.Max(0, currentSpread - spreadRecovery * Time.deltaTime);
            Crosshair.instance.UpdateSpread(currentSpread / maxSpread);
        }
    }

    void Attack()
    {
        currentAmmo--;
        currentAmmoUpdated.Invoke(currentAmmo);
        currentSpread = Mathf.Min(maxSpread, currentSpread + spreadPerShot);
        Vector2 rand = Random.insideUnitCircle * currentSpread;
        Quaternion spreadRotation = Quaternion.Euler(-rand.y, rand.x, 0f);
        Vector3 dir = spreadRotation * Camera.main.transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, dir, out hit, float.MaxValue, ~ignoreLayers))
        {
            if (hit.collider.gameObject.TryGetComponent<IDamage>(out IDamage dmg))
            {
                dmg.TakeDamage(shootDamage);
            }
            TrailRenderer trail = Instantiate(bulletTrail, shootPos.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit));
        }
    }

    void FinishAttack()
    {
        canAttack = true;
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
}
