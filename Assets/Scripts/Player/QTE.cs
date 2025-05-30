using DG.Tweening;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QTE : MonoBehaviour
{
    [SerializeField] Sprite[] arrowGraphics;
    [SerializeField] Image[] arrows;
    [SerializeField] ParticleSystem[] particleSystems;
    [SerializeField] ParticleSystem[] arrowGhosts;
    [SerializeField] ParticleSystem particleTimer;
    [SerializeField] Vector2[] timerRadiusAndCount;
    int currentQTEIndex;
    int currentQTEActionCount;
    bool eventOccuring = false;
    UnityEvent<bool> passedQTE;
    Coroutine autoFailRoutine;
    Coroutine disableRoutine;
    Coroutine timerCoroutine;
    [SerializeField] Color defaultColor;
    [SerializeField] Color successColor;
    [SerializeField] Color failureColor;
    float timeMult;
    float particleDefaultRadius;
    Color particleDefaultColor;
    Color arrowDefaultColor;
    ParticleSystem.Particle[] timerParticles;
    List<Tween> tweens = new List<Tween>();

    private void Start()
    {
        particleDefaultRadius = particleSystems[0].shape.radius;
        particleDefaultColor = particleSystems[0].main.startColor.color;
        arrowDefaultColor = arrows[0].color;
        ResetAll();
        StartCoroutine(DisableAll(0));
    }

    public UnityEvent<bool> StartQTE(int numberOfActions, float duration, float timeScale)
    {
        if (disableRoutine != null)
        {
            StopCoroutine(disableRoutine);
        }
        UIManager.instance.crosshair.gameObject.SetActive(false);
        gameObject.SetActive(true);
        currentQTEIndex = 0;
        currentQTEActionCount = numberOfActions;
        int i = 0;

        for (; i < numberOfActions; i++)
        {
            SetRandomActionState(i);
            EnableAction(i);
        }
        for (; i < arrows.Length; i++)
        {
            DisableAction(i);
        }
        eventOccuring = true;
        passedQTE = new UnityEvent<bool>();
        timeMult = timeScale;
        Time.timeScale *= timeMult;
        Time.fixedDeltaTime = Time.timeScale * .02f;
        foreach (ParticleSystem ps in particleSystems)
        {
            ParticleSystem.MainModule m = ps.main;
            m.simulationSpeed /= timeMult;
        }
        autoFailRoutine = StartCoroutine(AutofailQTE(duration * timeMult));
        StartTimer(currentQTEActionCount, duration * timeMult);
        
        return passedQTE;
    }

    void StartTimer(int count, float duration)
    {
        ParticleSystem.ShapeModule s = particleTimer.shape;
        s.scale = new Vector3(timerRadiusAndCount[count-1].x, 1f, 1f);
        particleTimer.Clear();
        particleTimer.Play();
        particleTimer.Emit((int)timerRadiusAndCount[count-1].y);
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(DeleteFromTimerRing(duration));
    }

    IEnumerator DeleteFromTimerRing(float duration)
    {
        int maxParticles = particleTimer.main.maxParticles;
        if (timerParticles == null || timerParticles.Length < maxParticles)
        {
            timerParticles = new ParticleSystem.Particle[maxParticles];
        }
        int aliveCount = particleTimer.GetParticles(timerParticles);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float cutoff = Mathf.Lerp(0f, Mathf.PI * 2f, elapsed / duration);
            Color color = Color.Lerp(successColor, failureColor, elapsed / duration);
            for (int i = 0; i < aliveCount; i++)
            {
                Vector3 pos = timerParticles[i].position.normalized;
                float angle = Mathf.Atan2(pos.y, pos.x);
                angle -= Mathf.PI / 2f;
                if (angle < 0) angle += Mathf.PI * 2f;

                if (angle <= cutoff)
                {
                    timerParticles[i].remainingLifetime = -1f;
                } else
                {
                    timerParticles[i].startColor = color;
                }
            }

            particleTimer.SetParticles(timerParticles, aliveCount);
            elapsed += Time.deltaTime;
            yield return null;
        }

        DisableTimer();
    }

    void DisableTimer()
    {
        int aliveCount = particleTimer.GetParticles(timerParticles);
        for (int i = 0; i < aliveCount; i++)
        {
            timerParticles[i].remainingLifetime = -1f;
        }

        particleTimer.SetParticles(timerParticles, aliveCount);
        timerCoroutine = null;
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
        if (Input.GetButtonDown("Up"))
        {
            HandleInput(0);
        } 
        if (Input.GetButtonDown("Down"))
        {
            HandleInput(3);
        }
        if (Input.GetButtonDown("Right"))
        {
            HandleInput(1);
        } 
        if (Input.GetButtonDown("Left"))
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
            for (int i = currentQTEIndex; i < currentQTEActionCount; i++)
            {
                FailEffect(i);
            }
            FinishEvent(false);
        }
    }

    IEnumerator DisableAll(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (ParticleSystem p in particleSystems)
        {
            p.gameObject.SetActive(false);
            
        }
        foreach (Image i in arrows)
        {
            i.gameObject.SetActive(false);
        }
        ResetAll();
        disableRoutine = null;
    }

    void ResetAll()
    {
        for (int i = 0; i <  arrows.Length; i++)
        {
            ResetAction(i);
        }
    }

    void FinishEvent(bool success)
    {
        Time.timeScale /= timeMult;
        Time.fixedDeltaTime = Time.timeScale * .02f;
        timeMult = 1f;
        AdjustTweens();
        tweens.Clear();
        if (autoFailRoutine != null)
        {
            StopCoroutine(autoFailRoutine);
            autoFailRoutine = null;
        }
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            DisableTimer();
        }
        UIManager.instance.crosshair.gameObject.SetActive(true);
        //Debug.Log("Finished QTE index is " + currentQTEIndex);
        passedQTE.Invoke(success);
        eventOccuring = false;
        disableRoutine = StartCoroutine(DisableAll(0.6f));
        //gameObject.SetActive(false);
    }

    IEnumerator InstantiateAfterFrame(ParticleSystem p, int rand)
    {
        yield return null;
        ParticleSystem arrowGhost = Instantiate(arrowGhosts[rand], p.gameObject.transform);
        ParticleSystem.MainModule m = arrowGhost.main;
        m.simulationSpeed /= timeMult;
    }

    void SetRandomActionState(int index)
    {
        int graphicIndex = Random.Range(0, arrowGraphics.Length);
        arrows[index].sprite = arrowGraphics[graphicIndex];
        foreach (Transform child in particleSystems[index].gameObject.transform)
        {
            if (child.gameObject.GetComponent<ParticleSystem>() != null)
            {
                Destroy(child.gameObject);
            }
        }
        StartCoroutine(InstantiateAfterFrame(particleSystems[index], graphicIndex));
        //m.simulationSpeed /= timeMult;
    }

    IEnumerator PlayAfterFrame(ParticleSystem p)
    {
        yield return null;
        p.Clear(true);
        p.Play(true);
    }

    void DisableAction(int index)
    {
        arrows[index].gameObject.SetActive(false);
        particleSystems[index].gameObject.SetActive(false);
        
        ResetAction(index);
    }

    void EnableAction(int index)
    {
        arrows[index].gameObject.SetActive(true);
        particleSystems[index].gameObject.SetActive(true);
        StartCoroutine(PlayAfterFrame(particleSystems[index]));
    }

    void AddTween(Tween t)
    {
        t.timeScale /= timeMult;
        tweens.Add(t);
    }
    void AdjustTweens()
    {
        foreach (Tween t in tweens)
        {
            if (t != null)
            {
                t.timeScale = timeMult;
            }
        }
        foreach (ParticleSystem ps in  particleSystems)
        {
            ParticleSystem.MainModule m = ps.main;
            m.simulationSpeed = timeMult;
        }
    }

    void SuccessEffect(int index)
    {
        float effectDuration = 0.6f;
        Tween t;
        t = arrows[index].DOColor(successColor, effectDuration / 4f).OnComplete(() =>
        {
            AddTween(arrows[index].DOColor(new Color(0, 0, 0, 0), effectDuration / 2f));
        });
        AddTween(t);

        t = arrows[index].rectTransform.DOScale(new Vector3(5, 5), effectDuration / 2f).SetDelay(effectDuration / 6f);
        AddTween(t);

        float radius = particleDefaultRadius;
        t = DOTween.To(() => radius, x =>
        {
            radius = x;
            var s = particleSystems[index].shape;
            s.radius = radius;
            particleSystems[index].Clear();
            particleSystems[index].Play();
        }, 1.5f, effectDuration).SetEase(Ease.InExpo);
        AddTween(t);

        Color color = particleDefaultColor;
        t = DOTween.To(() => color, x =>
        {
            color = x;
            var main = particleSystems[index].main;
            main.startColor = color;
        }, new Color(1, 1, 1, 0), effectDuration).SetEase(Ease.InExpo);
        AddTween(t);
        foreach (Transform child in particleSystems[index].gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        
    }

    void FailEffect(int index)
    {
        float effectDuration = 0.6f;
        Tween t;
        t = arrows[index].DOColor(failureColor, effectDuration / 4f).OnComplete(() =>
        {
            AddTween(arrows[index].DOColor(new Color(failureColor.r, failureColor.g, failureColor.b, 0), effectDuration / 2f).SetEase(Ease.InExpo));
        });
        AddTween(t);

        float radius = particleDefaultRadius;
        t = DOTween.To(() => radius, x =>
       {
           radius = x;
           var s = particleSystems[index].shape;
           s.radius = radius;
           particleSystems[index].Clear();
           particleSystems[index].Play();
       }, 0.0f, effectDuration / 2f).SetEase(Ease.InExpo);
        AddTween(t);

        Color color = particleDefaultColor;
        t = DOTween.To(() => color, x =>
        {
            color = x;
            var main = particleSystems[index].main;
            main.startColor = color;
        }, new Color(1, 1, 1, 0), effectDuration).SetEase(Ease.InExpo);
        AddTween(t);
        foreach (Transform child in particleSystems[index].gameObject.transform)
        {
            Destroy(child.gameObject);
        }
    }



    void ResetAction(int index)
    {
        ParticleSystem.MainModule m = particleSystems[index].main;
        ParticleSystem.ShapeModule s = particleSystems[index].shape;
        s.radius = particleDefaultRadius;
        m.startColor = particleDefaultColor;
        particleSystems[index].Clear();
        particleSystems[index].Play();
        arrows[index].color = arrowDefaultColor;
        arrows[index].rectTransform.localScale = Vector3.one;
    }

    
}
