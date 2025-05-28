using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class ScientistHealth : MonoBehaviour
{
    [SerializeField] float tweenSpeed = 0.15f;
    [SerializeField] Image fillRing;

    private void Start()
    {
        GameManager.instance.escort.escortHealthUpdated.AddListener(UpdateHealthbar);
    }

    private void OnDestroy()
    {
        GameManager.instance.escort.escortHealthUpdated.RemoveListener(UpdateHealthbar);
    }

    void UpdateHealthbar()
    {
        fillRing.DOFillAmount((float)GameManager.instance.escort.currentHealth / (float)GameManager.instance.escort.maxHealth, tweenSpeed);
    }
}
