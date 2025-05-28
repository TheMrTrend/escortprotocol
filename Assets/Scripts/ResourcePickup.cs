using UnityEngine;
using TMPro;

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
            string message = "";

            switch (resourceType)
            {
                case ResourceType.HEALTH:
                    player.AddHealth(amount);
                    message = $"+{amount} Health";
                    break;
                case ResourceType.ESSENCE:
                    player.AddEssence(amount);
                    message = $"+{amount} Essence";
                    break;
                default:
                    player.AddAmmo(amount, resourceType);
                    message = $"+{amount} {resourceType.ToString().Replace("_", " ")}";
                    break;
            }
        }
    }
}
