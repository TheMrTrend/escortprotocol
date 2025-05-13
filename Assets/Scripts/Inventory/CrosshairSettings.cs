using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Objects/Crosshair Settings")]
public class CrosshairSettings : ScriptableObject
{
    public Sprite centerDot;
    public Sprite[] prongSprites;
    public float minDistance;
    public float maxDistance;
    public float lerpSpeed;
}

