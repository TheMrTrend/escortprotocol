using UnityEngine;

public enum ResourceType{
    HEALTH,
    ESSENCE,
    PISTOL_AMMO,
    RIFLE_AMMO,
    NONE
}
public class ResourcePickup : MonoBehaviour, IPickup
{
    public ResourceType resourceType;
    public int amount;
    public float idleFloatMagnitude = 0.2f;
    public float idleFloatFreq = 1.0f;
    public float idleRotationSpeed = 4.0f;
    float floatDelta;
    float startY;

    void Start()
    {
        startY = transform.position.y;
    }
    void Update()
    {
        floatDelta += Time.deltaTime;
        transform.position = new Vector3(transform.position.x, startY + Mathf.Sin(floatDelta * idleFloatFreq) * idleFloatMagnitude, transform.position.z);
        transform.Rotate(Vector3.up, idleRotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Pickup(other);
            Destroy(gameObject);
        }
    }
    public void Pickup(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerController player))
        {
            if (resourceType == ResourceType.HEALTH)
            {
                player.AddHealth(amount);
            } else if (resourceType == ResourceType.ESSENCE)
            {
                player.AddEssence(amount);
            } else
            {
                player.AddAmmo(amount, resourceType);
            }
        }
    }
}
