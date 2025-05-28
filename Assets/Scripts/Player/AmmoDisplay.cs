using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentAmmo;
    [SerializeField] TextMeshProUGUI storedAmmo;
    
    public void UpdateCurrentAmmo(int amount)
    {
        currentAmmo.text = amount.ToString();
    }

    public void UpdateStoredAmmo(int amount)
    {
        storedAmmo.text = amount.ToString();
    }

    public void DisableAmmos()
    {
        currentAmmo.enabled = false;
        storedAmmo.enabled = false;
    }
    public void EnableAmmos()
    {
        currentAmmo.enabled = true;
        storedAmmo.enabled = true;
    }
}
