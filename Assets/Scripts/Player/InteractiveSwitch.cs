using UnityEngine;

public class InteractableSwitch : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactRange = 3f;          // Distance the player can be from the switch to use it
    [SerializeField] private KeyCode interactKey = KeyCode.E;   // Key used to interact
    [SerializeField] private GameObject targetObject;           // The object this switch will toggle

    private bool isPlayerNearby = false;                        // Tracks if the player is close enough to interact

    void Update()
    {
        // If the player is nearby and presses the interaction key
        if (isPlayerNearby && Input.GetKeyDown(interactKey))
        {
            ToggleTarget(); // Toggle the target object
        }
    }

    // Turns the target object on or off
    private void ToggleTarget()
    {
        if (targetObject != null)
        {
            bool currentState = targetObject.activeSelf; // Get current active state
            targetObject.SetActive(!currentState);       // Toggle it
            Debug.Log($"{gameObject.name} toggled {targetObject.name} to {!currentState}");
        }
    }

    // When something enters trigger zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true; // Allow interaction
        }
    }

    // When something exits trigger zone
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false; // Disallow interaction
        }
    }
}