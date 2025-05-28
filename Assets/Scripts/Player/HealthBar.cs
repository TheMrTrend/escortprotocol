using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image[] segments;
    [SerializeField] float tweenSpeed = 0.15f;
    Color startColor;
    [SerializeField] Color lowColor;

    private void Start()
    {
        GameManager.instance.playerController.healthUpdatedEvent.AddListener(SetHealthSlider);
        startColor = segments[0].color;
    }

    private void OnDisable()
    {
        GameManager.instance.playerController.healthUpdatedEvent.RemoveListener(SetHealthSlider);
    }

    /*void SetHealthSlider()
    {
        slider.DOValue((float)GameManager.instance.playerController.health / (float)GameManager.instance.playerController.maxHealth, tweenSpeed);
    }*/
    void SetHealthSlider()
    {
        float percent = (float)GameManager.instance.playerController.health / (float)GameManager.instance.playerController.maxHealth;
        for (int i = 0; i < segments.Length; i++)
        {
            Image segment = segments[i];
            float bound = i * (1f / segments.Length);
            segment.DOFillAmount((percent - bound) / (1f / segments.Length), tweenSpeed / segments.Length).SetDelay(i * (tweenSpeed/segments.Length));
            if (percent <= 0.2f && segment.color != lowColor)
            {
                segment.DOColor(lowColor, tweenSpeed).SetDelay(i * (tweenSpeed / segments.Length));
            } else if (percent > 0.2f && segment.color == lowColor) {
                segment.DOColor(startColor, tweenSpeed).SetDelay(i * (tweenSpeed / segments.Length));
            }
        }
    }

}
