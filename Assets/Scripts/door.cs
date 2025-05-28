using UnityEngine;

public class door : MonoBehaviour
{
    [SerializeField] GameObject doorModel;
    [SerializeField] GameObject button;
    [SerializeField] string text;

    bool playerInTrigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerInTrigger)
        {
            if (Input.GetButtonDown("Interact"))
            {
                doorModel.SetActive(false);
                button.SetActive(false);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        IOpen openable = other.GetComponent<IOpen>();
        if (openable != null)
        {
            button.SetActive(true);
            playerInTrigger = true;
            GameManager.instance.textPopupDescription.text = text;
            GameManager.instance.textPopup.SetActive(true);

        }

    }
    private void OnTriggerExit(Collider other)
    {
        IOpen openable = other.GetComponent<IOpen>();
        if (openable != null)
        {
            button.SetActive(false);
            playerInTrigger = false;
            doorModel.SetActive(true);
            GameManager.instance.textPopup.SetActive(false);
        }

    }
}