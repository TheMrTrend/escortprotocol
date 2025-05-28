using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeldItem : MonoBehaviour
{
    public Item currentItem;
    public List<Item> items = new List<Item>();
    [SerializeField] AmmoDisplay ammoDisplay;
    private Image reticle;

    private void Start()
    {
        InitializeHeldItems();
    }

    public void InitializeHeldItems()
    {
        items.Clear(); // Just in case it's already populated
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out Item item))
            {
                items.Add(item);
                item.gameObject.SetActive(false);
            }
        }

        int pistolIndex = items.FindIndex(i => i.ammoType == ResourceType.PISTOL_AMMO || i.name.Contains("Pistol"));
        if (pistolIndex < 0) pistolIndex = 0;

        SetCurrentItem(pistolIndex);
    }

    public void SetCurrentItem(int slot)
    {
        if (slot < 0 || slot >= items.Count) return;

        Item item = items[slot];
        if (currentItem != null && item == currentItem) return;

        // Disable current
        currentItem?.currentAmmoUpdated.RemoveListener(UpdateCurrentAmmo);
        currentItem?.storedAmmoUpdated.RemoveListener(UpdateStoredAmmo);
        currentItem?.gameObject.SetActive(false);

        currentItem = item;
        currentItem.gameObject.SetActive(true);

        // Hide all ammo displays first
        UIManager.instance.ammoDisplay_Pistol.gameObject.SetActive(false);
        UIManager.instance.ammoDisplay_Rifle.gameObject.SetActive(false);
        UIManager.instance.ammoDisplay_Knife.gameObject.SetActive(false);

        // Handle UI display
        if (item.name.Contains("Knife"))
        {
            UIManager.instance.ShowKnifeDisplay();
            ammoDisplay = null;
        }
        else if (item.name.Contains("Pistol"))
        {
            UIManager.instance.ShowPistolDisplay();
            ammoDisplay = UIManager.instance.ammoDisplay_Pistol;
        }
        else if (item.name.Contains("Rifle"))
        {
            UIManager.instance.ShowRifleDisplay();
            ammoDisplay = UIManager.instance.ammoDisplay_Rifle;
        }

        // Set the ammo display UI as active
        ammoDisplay.gameObject.SetActive(true);
        ammoDisplay.EnableAmmos();
        ammoDisplay.UpdateCurrentAmmo(item.currentAmmo);
        ammoDisplay.UpdateStoredAmmo(item.storedAmmo);

        // Register listeners
        currentItem.ammoDisplay = ammoDisplay;
        currentItem.currentAmmoUpdated.AddListener(UpdateCurrentAmmo);
        currentItem.storedAmmoUpdated.AddListener(UpdateStoredAmmo);
    }

    void UpdateCurrentAmmo(int amount)
    {
        ammoDisplay.UpdateCurrentAmmo(amount);
    }

    public void UpdateStoredAmmo(int amount)
    {
        ammoDisplay.UpdateStoredAmmo(amount);
    }

    void CycleWeapon(int direction)
    {
        int nextSlot = (items.IndexOf(currentItem) + direction + items.Count) % items.Count;
        SetCurrentItem(nextSlot);
    }

    void Update()
    {
        int scroll = (int)Input.GetAxisRaw("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            CycleWeapon(1);
        }
        else if (scroll < 0f)
        {
            CycleWeapon(-1);
        }


        if (Input.GetButtonDown("Fire1"))
        {
            currentItem.Primary();
        }
        else if (Input.GetButton("Fire1"))
        {
            currentItem.PrimaryHeld();
        }
        else if (Input.GetButtonUp("Fire1"))
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
        else if (Input.GetButtonUp("Fire2"))
        {
            currentItem.SecondaryRelease();
        }

        if (Input.GetButtonDown("Reload"))
        {
            currentItem.Reload();
        }
    }
}
