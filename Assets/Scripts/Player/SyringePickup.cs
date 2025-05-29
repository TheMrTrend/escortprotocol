using Unity.VisualScripting;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IPickup
{ 
    public float idleFloatMagnitude = 0.2f;
    public float idleFloatFreq = 1.0f;
    public float idleRotationSpeed = 4.0f;
    float floatDelta;
    float startY;
    [SerializeField] Item itemToUnlock;
    public DialogueSequence pikcupSequence;

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
        Pickup(other);
    }
    public void Pickup(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                player.held.AddUnlock(itemToUnlock);
                if (pikcupSequence != null) DialogueManager.instance.Activate(pikcupSequence);
                player.keys.Add(itemToUnlock.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
