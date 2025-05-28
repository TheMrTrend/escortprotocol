using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class HeldItem : MonoBehaviour
{
    public Item currentItem;
    public List<Item> items = new List<Item>();
    [SerializeField] AmmoDisplay ammoDisplay;
    private Image reticle;

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

        
        int pistolIndex = items.FindIndex(i => i.ammoType == ResourceType.PISTOL_AMMO || i.name.Contains("Pistol"));

        
        if (pistolIndex < 0) pistolIndex = 0;

        
        ammoDisplay = UIManager.instance.ammoDisplay_Pistol;
        reticle = UIManager.instance.crosshairPistol;

        SetCurrentItem(pistolIndex);
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
        if (item.name.Contains("Knife"))
        {
            UIManager.instance.ShowKnifeDisplay();
        }
        else if (item.name.Contains("Pistol"))
        {
            ammoDisplay = UIManager.instance.ammoDisplay_Pistol;
            UIManager.instance.ShowPistolDisplay();
            ammoDisplay.EnableAmmos();
            ammoDisplay.UpdateCurrentAmmo(item.currentAmmo);
            ammoDisplay.UpdateStoredAmmo(item.storedAmmo);
        }
        else if (item.name.Contains("Rifle"))
        {
            ammoDisplay = UIManager.instance.ammoDisplay_Rifle;
            UIManager.instance.ShowRifleDisplay();
            ammoDisplay.EnableAmmos();
            ammoDisplay.UpdateCurrentAmmo(item.currentAmmo);
            ammoDisplay.UpdateStoredAmmo(item.storedAmmo);
        }
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
