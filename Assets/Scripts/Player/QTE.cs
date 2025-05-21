using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QTE : MonoBehaviour
{
    [SerializeField] Sprite[] arrowGraphics;
    [SerializeField] Image[] arrows;
    [SerializeField] ParticleSystem[] particleSystems;
    int currentQTEIndex;
    int currentQTEActionCount;
    bool eventOccuring = false;
    UnityEvent<bool> passedQTE;
    Coroutine autoFailRoutine;

    public UnityEvent<bool> StartQTE(int numberOfActions, float duration)
    {
        gameObject.SetActive(true);
        currentQTEIndex = 0;
        currentQTEActionCount = numberOfActions;
        int deadActions = arrows.Length - numberOfActions;
        for (int i = 0; i < numberOfActions; i++)
        {
            EnableAction(i);
            SetRandomActionState(i);
        }
        for (int i = deadActions; i < arrows.Length; i++)
        {
            DisableAction(i);
        }
        eventOccuring = true;
        passedQTE = new UnityEvent<bool>();
        autoFailRoutine = StartCoroutine(AutofailQTE(duration));
        return passedQTE;
    }

    IEnumerator AutofailQTE(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (eventOccuring)
        {
            autoFailRoutine = null;
            FinishEvent(false);
        }
    }

    private void Update()
    {
        if (!eventOccuring) { return; }
        if (Input.GetAxis("Vertical") == 1)
        {
            HandleInput(0);
        } 
        if (Input.GetAxis("Vertical") == -1)
        {
            HandleInput(3);
        }
        if (Input.GetAxis("Horizontal") == 1)
        {
            HandleInput(1);
        } 
        if (Input.GetAxis("Horizontal") == -1)
        {
            HandleInput(2);
        }
    }

    void HandleInput(int direction)
    {
        Sprite compSprite = arrowGraphics[direction];
        if (compSprite != null && compSprite == arrows[currentQTEIndex].sprite)
        {
            SuccessEffect(currentQTEIndex);
            currentQTEIndex++;
            if (currentQTEActionCount == currentQTEIndex)
            {
                FinishEvent(true);
            }
        } else
        {
            FailEffect(currentQTEIndex);
            FinishEvent(false);
        }
    }

    void FinishEvent(bool success)
    {
        if (autoFailRoutine != null)
        {
            StopCoroutine(autoFailRoutine);
            autoFailRoutine = null;
        }
        passedQTE.Invoke(success);
        eventOccuring = false;
        gameObject.SetActive(false);
    }

    void SetRandomActionState(int index)
    {
        int graphicIndex = Random.Range(0, arrowGraphics.Length - 1);
        arrows[index].sprite = arrowGraphics[graphicIndex];
    }

    void DisableAction(int index)
    {
        arrows[index].gameObject.SetActive(false);
        particleSystems[index].gameObject.SetActive(false);
    }

    void EnableAction(int index)
    {
        arrows[index].gameObject.SetActive(true);
        particleSystems[index].gameObject.SetActive(true);
    }

    void SuccessEffect(int index)
    {
        arrows[index].color = Color.green;
        ParticleSystem.MainModule p = particleSystems[index].main;
        p.startSpeed = 0.3f;
        DOTween.To(() => p.startColor.color, x => p.startColor = x, new Color(1, 1, 1, 0), 0.3f).OnComplete(() =>
        {
            DisableAction(index);
        });
    }

    void FailEffect(int index)
    {
        arrows[index].color = Color.red;
        ParticleSystem.MainModule p = particleSystems[index].main;
        p.startSpeed = -0.1f;
        DOTween.To(() => p.startColor.color, x => p.startColor = x, new Color(1, 1, 1, 0), 0.3f).OnComplete(() =>
        {
            DisableAction(index);
        });
    }
}
