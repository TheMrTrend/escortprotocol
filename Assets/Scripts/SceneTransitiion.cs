using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTransition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Level 2");
        }
    }
}