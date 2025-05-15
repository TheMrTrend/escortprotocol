using UnityEditor.Rendering;
using UnityEngine;

public class CardPickup : MonoBehaviour, IPickup
{ 
    public float idleFloatMagnitude = 0.2f;
    public float idleFloatFreq = 1.0f;
    public float idleRotationSpeed = 4.0f;
    float floatDelta;
    float startY;
    public DialogueSequence cardPickupSequence;

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
                player.hasKeyCard = true;
                DialogueManager.instance.Activate(cardPickupSequence);
                Destroy(gameObject);
            }
        }
    }
}
