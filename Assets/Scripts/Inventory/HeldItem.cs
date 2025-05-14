using System.Collections.Generic;
using UnityEngine;

public class HeldItem : MonoBehaviour
{
    public Item currentItem;
    List<Item> items = new List<Item>();
    [SerializeField] AmmoDisplay ammoDisplay;
    [SerializeField] Crosshair crosshair;

    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out Item item))
            {
                items.Add(item);
                item.gameObject.SetActive(false);
            }
        }
        SetCurrentItem(0);
    }

    public void SetCurrentItem(int slot)
    {
        Item item = items[slot];
        if (currentItem != null && item == currentItem) return;
        currentItem?.currentAmmoUpdated.RemoveListener(UpdateCurrentAmmo);
        currentItem?.storedAmmoUpdated.RemoveListener(UpdateStoredAmmo);
        currentItem?.gameObject.SetActive(false);
        currentItem = item;
        currentItem.gameObject.SetActive(true);
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
