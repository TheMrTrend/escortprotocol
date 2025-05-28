using TMPro;
using UnityEngine;

public class FloatingTextUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float lifetime = 1.2f;
    public float moveSpeed = 30f;

    RectTransform rect;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        rect.anchoredPosition += new Vector2(0, moveSpeed * Time.deltaTime);
    }

    public void Init(string message, Color? colorOverride = null, TMP_FontAsset fontOverride = null, int? sizeOverride = null)
    {
        text.text = message;
        text.color = colorOverride ?? UIManager.instance.floatingTextColor;
        text.font = fontOverride ?? UIManager.instance.floatingFont;
        text.fontSize = sizeOverride ?? UIManager.instance.floatingFontSize;
    }
}
