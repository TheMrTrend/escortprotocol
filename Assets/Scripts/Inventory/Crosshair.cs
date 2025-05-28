using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public static Crosshair instance;
    [SerializeField] RectTransform centerDot;
    [SerializeField] RectTransform[] prongs;
    [SerializeField] float minDistance = 10f;
    [SerializeField] float maxDistance = 50f;
    [SerializeField] float lerpSeped = 6f;

    float currentDistance;
    float targetDistance;
    Vector2[] prongDirs;



    private void Awake()
    {
        instance = this;
        prongDirs = new Vector2[prongs.Length];
        for (int i= 0; i<prongs.Length; i++)
        {
            prongDirs[i] = prongs[i].anchoredPosition.normalized;
        }

        currentDistance = targetDistance = minDistance;

    }
    public void ApplySettings(CrosshairSettings settings)
    {
        centerDot.GetComponent<Image>().sprite = settings.centerDot;
        for(int i = 0; i < prongs.Length; i++) 
        {
            if (settings.prongSprites.Length <= i)
            {
                prongs[i].GetComponent<Image>().enabled = false;
            }else if (settings.prongSprites[i] != null)
            {
                prongs[i].GetComponent<Image>().enabled = true;
                prongs[i].GetComponent<Image>().sprite = settings.prongSprites[i];
            } else
            {
                prongs[i].GetComponent<Image>().enabled = false;
            }
            
        }
        minDistance = settings.minDistance;
        maxDistance = settings.maxDistance;
        lerpSeped = settings.lerpSpeed;
    }

    private void Update()
    {
        if (!Mathf.Approximately(currentDistance, targetDistance))
        {
            currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * lerpSeped);
            UpdateProngPositions();
        }
    }

    void UpdateProngPositions()
    {
        for (int i = 0; i<prongs.Length; i++)
        {
            prongs[i].anchoredPosition = prongDirs[i] * currentDistance;
        }
    }

    public void UpdateSpread(float amount)
    {
        targetDistance = Mathf.Lerp(minDistance, maxDistance, amount);
    }
}
