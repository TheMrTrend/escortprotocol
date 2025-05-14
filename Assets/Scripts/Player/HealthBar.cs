using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Slider slider;
    [SerializeField] float tweenSpeed = 0.15f;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        GameManager.instance.playerController.healthUpdatedEvent.AddListener(SetHealthSlider);
    }

    private void OnDisable()
    {
        GameManager.instance.playerController.healthUpdatedEvent.RemoveListener(SetHealthSlider);
    }

    void SetHealthSlider()
    {
        slider.DOValue((float)GameManager.instance.playerController.health / (float)GameManager.instance.playerController.maxHealth, tweenSpeed);
    }
}
