using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EssenceBar : MonoBehaviour
{
    Slider slider;
    [SerializeField] float tweenSpeed = 0.15f;
    [SerializeField] Gradient colorGradient;
    [SerializeField] Image fillBar;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        GameManager.instance.playerController.essenceUpdated.AddListener(SetEssenceSlider);
    }

    private void OnDisable()
    {
        GameManager.instance.playerController.essenceUpdated.RemoveListener(SetEssenceSlider);
    }

    void SetEssenceSlider()
    {

        float value = (float)GameManager.instance.playerController.essence / (float)GameManager.instance.playerController.maxEssence;
        Debug.Log("Value is " + value + " essence is " + GameManager.instance.playerController.essence + " max essence is " + GameManager.instance.playerController.maxEssence);
        slider.DOValue(value, tweenSpeed).SetEase(Ease.InExpo).SetDelay(0.25f);
        fillBar.DOColor(colorGradient.Evaluate(value), tweenSpeed).SetEase(Ease.InExpo).SetDelay(0.25f);
    }
}
