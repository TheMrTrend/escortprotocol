using UnityEngine;

public class KnifeDisplay : MonoBehaviour
{
    public GameObject knifeNoAmmoBox;

    public void Show()
    {
        if (knifeNoAmmoBox != null)
            knifeNoAmmoBox.SetActive(true);
    }

    public void Hide()
    {
        if (knifeNoAmmoBox != null)
            knifeNoAmmoBox.SetActive(false);
    }
}
