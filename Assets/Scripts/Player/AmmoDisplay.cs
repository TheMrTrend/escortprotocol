using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentAmmo;
    [SerializeField] TextMeshProUGUI storedAmmo;
    [SerializeField] private GameObject ammoWarningBox;

    public void UpdateCurrentAmmo(int amount)
    {
        currentAmmo.text = amount.ToString();
        UpdateWarningBox(amount);
    }

    public void UpdateStoredAmmo(int amount)
    {
        storedAmmo.text = amount.ToString();
    }

    public void DisableAmmos()
    {
        currentAmmo.enabled = false;
        storedAmmo.enabled = false;
        if (ammoWarningBox != null)
            ammoWarningBox.SetActive(false);
    }
    public void EnableAmmos()
    {
        currentAmmo.enabled = true;
        storedAmmo.enabled = true;
    }

    private void UpdateWarningBox(int current)
    {
        if (ammoWarningBox != null)
            ammoWarningBox.SetActive(current <= 0);
    }
}
