using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    Slider healthSlider;
    TextMeshProUGUI objectiveText;

    void OnAwake()
    {
        healthSlider = GameObject.FindWithTag("Health Bar").GetComponent<Slider>();
        objectiveText = GameObject.FindWithTag("Objective").GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        GameManager.instance.playerController.healthUpdatedEvent.AddListener(UpdateHealthSlider);
    }
    private void OnDisable()
    {
        GameManager.instance.playerController.healthUpdatedEvent.RemoveListener(UpdateHealthSlider);
    }
    void UpdateHealthSlider()
    {
        healthSlider.DOValue((float)GameManager.instance.playerController.health / (float)GameManager.instance.playerController.maxHealth, 0.25f).SetEase(Ease.OutQuint);
    }

    void UpdateObjective()
    {

    }
}
