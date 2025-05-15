using UnityEngine;
using UnityEngine.Events;
public class Item : MonoBehaviour
{
    public string itemName;
    public Renderer model;
    public Animator animator;
    [SerializeField] public CrosshairSettings crosshairSettings;
    public int clipSize = 10;
    public int currentAmmo = 10;
    public int storedAmmo = 0;
    [System.NonSerialized] public UnityEvent<int> currentAmmoUpdated;
    [System.NonSerialized] public UnityEvent<int> storedAmmoUpdated;
    public ResourceType ammoType = ResourceType.NONE;

    private void Awake()
    {
        currentAmmoUpdated = new UnityEvent<int>();
        storedAmmoUpdated = new UnityEvent<int>();
    }

    public virtual void Primary() { }
    public virtual void Secondary() { }
    public virtual void PrimaryHeld() { }

    public virtual void SecondaryHeld() { }

    public virtual void PrimaryRelease() { }

    public virtual void SecondaryRelease() { }

    public virtual void Reload() { }
}
