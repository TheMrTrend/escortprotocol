using UnityEngine;

public class HeldItem : MonoBehaviour
{
    public Item currentItem;
    [SerializeField] AmmoDisplay ammoDisplay;
    [SerializeField] Crosshair crosshair;

    private void Start()
    {
        SetCurrentItem(currentItem);
    }

    public void SetCurrentItem(Item item)
    {
        currentItem.currentAmmoUpdated.RemoveListener(UpdateCurrentAmmo);
        currentItem.storedAmmoUpdated.RemoveListener(UpdateStoredAmmo);
        currentItem = item;

        if (item.clipSize != 0)
        {
            ammoDisplay.EnableAmmos();
            ammoDisplay.UpdateCurrentAmmo(item.currentAmmo);
            ammoDisplay.UpdateStoredAmmo(item.storedAmmo);
        } else
        {
            ammoDisplay.DisableAmmos();
        }
        currentItem.currentAmmoUpdated.AddListener(UpdateCurrentAmmo);
        currentItem.storedAmmoUpdated.AddListener(UpdateStoredAmmo);
        crosshair.ApplySettings(currentItem.crosshairSettings);
    }

    void UpdateCurrentAmmo(int amount)
    {
        ammoDisplay.UpdateCurrentAmmo(amount);
    }

    void UpdateStoredAmmo(int amount)
    {
        ammoDisplay.UpdateStoredAmmo(amount);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            currentItem.Primary();
        } else if (Input.GetButton("Fire1"))
        {
            currentItem.PrimaryHeld();
        }
        if (Input.GetButtonUp("Fire1"))
        {
            currentItem.PrimaryRelease();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            currentItem.Secondary();
        }
        else if (Input.GetButton("Fire2"))
        {
            currentItem.SecondaryHeld();
        }
        if (Input.GetButtonUp("Fire2"))
        {
            currentItem.SecondaryRelease();
        }
        if (Input.GetButtonDown("Reload"))
        {
            currentItem.Reload();
        }
    }
}
