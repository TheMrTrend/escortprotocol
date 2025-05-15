using DG.Tweening;
using NUnit.Framework.Internal.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public struct DialogueSpeaker {
    public string speakerName;
    public Sprite speakerIcon;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    public DialogueSpeaker[] speakerLibrary;
    Dictionary<string, Sprite> potentialSpeakers = new Dictionary<string, Sprite>();
    [SerializeField] Image frame;
    float defaultFrameOpacity;
    float defaultIconOpacity;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image icon;
    [SerializeField] Sprite errorSprite;
    [SerializeField] ParticleSystem frameParticles;
    [SerializeField] ParticleSystem iconParticles;
    [SerializeField] TextMeshProUGUI promptText;
    public string fastForwardText = "Skip [T]";
    public string dismissText = "Dismiss [T]";
    public string nextText = "Next [T]";
    public float typeSpeed;
    
    private string currentWrite;
    private Coroutine typingRoutine;
    private Coroutine autocompleteRoutine;
    DialogueSequence currentSequence;
    int currentSequenceIndex;

    List<DialogueSequence> queue = new List<DialogueSequence>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameManager.instance.playerController.dialogue.AddListener(Interaction);
        defaultFrameOpacity = frame.color.a;
        defaultIconOpacity = icon.color.a;
        frame.color = new Color(frame.color.r, frame.color.g, frame.color.b, 0);
        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0);
        LayoutElement e = frame.GetComponent<LayoutElement>();
        e.minHeight = 0;
        icon.rectTransform.localScale = new Vector3(icon.rectTransform.localScale.x, 0, icon.rectTransform.localScale.z);
        promptText.text = "";
        foreach (DialogueSpeaker d in speakerLibrary)
        {
            potentialSpeakers.Add(d.speakerName, d.speakerIcon);
        }
    }

    private void OnDisable()
    {
        GameManager.instance.playerController.interact.RemoveListener(Interaction);
    }

    public void Activate(DialogueSequence sequence)
    {
        if (currentSequence != null && sequence != currentSequence && !currentSequence.overwritable)
        {
            queue.Add(sequence);
        } else
        {
            StartDialogueSequence(sequence);
        }
        StartCoroutine(FadeInText(sequence));
    }  

    void StartDialogueSequence(DialogueSequence sequence)
    {
        currentSequence = sequence;
        currentSequenceIndex = 0;
        if (sequence.flagOnStart != null)
        {
            EventManager.instance.FireEvent(sequence.flagOnStart);
        }
        SetIconAndWrite(currentSequence.lines[currentSequenceIndex]);
    }

    void SetIconAndWrite(string line)
    {
        string[] s = line.Split('|');
        promptText.text = fastForwardText;
        if (potentialSpeakers.ContainsKey(s[0]))
        {
            icon.sprite = potentialSpeakers[s[0]];
        }
        else
        {
            icon.sprite = errorSprite;
        }
        StartTypewriter(s[1]);
    }

    void StartTypewriter(string input)
    {
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
        }

        typingRoutine = StartCoroutine(TypeText(input));
    }

    IEnumerator TypeText(string input)
    {
        text.text = "";
        
        currentWrite = input;
        foreach (char c in input)
        {
            text.text += c;
            if (c == '.' || c == '!' || c == '?' || c == '-')
            {
                yield return new WaitForSeconds(typeSpeed * 2.5f);
            } else if (c == ',')
            {
                yield return new WaitForSeconds(typeSpeed * 2f);
            } else {
                yield return new WaitForSeconds(typeSpeed);
            }
            
        }
        currentSequenceIndex++;
        typingRoutine = null;
        currentWrite = null;
        promptText.text = currentSequenceIndex >= currentSequence.lines.Length ? dismissText : nextText;
        if (currentSequence.autoTime != 0)
        {
            StartCoroutine(AutotimeRoutine());
        } 
    }


    void FadeOutText()
    {
        DisappearFrame(0.3f);
    }

    IEnumerator FadeInText(DialogueSequence sequence)
    {

        AppearFrame(0.4f);
        
        
        yield return new WaitForSeconds(0.25f);
        StartDialogueSequence(sequence);
        
    }

    void AppearFrame(float totalDuration)
    {
        text.text = "";
        StartCoroutine(RunParticles(frameParticles, frameParticles.main.duration));
        frame.DOColor(new Color(frame.color.r, frame.color.b, frame.color.g, defaultFrameOpacity), totalDuration * (2f/3f)).SetEase(Ease.OutExpo);
        LayoutElement e = frame.GetComponent<LayoutElement>();
        e.DOMinSize(new Vector2(e.minWidth, 64), totalDuration/3f).SetDelay(totalDuration/3f).OnComplete( () =>
        {
            StartCoroutine(RunParticles(iconParticles, iconParticles.main.duration));
            icon.DOColor(new Color(icon.color.r, icon.color.g, icon.color.b, defaultIconOpacity), iconParticles.main.duration).SetEase(Ease.OutCubic);
            icon.rectTransform.DOScaleY(1, iconParticles.main.duration);
        });
    }

    void DisappearFrame(float totalDuration)
    {
        text.text = "";
        promptText.text = "";
        icon.rectTransform.DOScaleY(0, iconParticles.main.duration);
        icon.DOColor(new Color(icon.color.r, icon.color.g, icon.color.b, 0), iconParticles.main.duration).SetEase(Ease.OutCubic);
        StartCoroutine(RunParticles(iconParticles, iconParticles.main.duration));
        LayoutElement e = frame.GetComponent<LayoutElement>();
        frame.DOColor(new Color(frame.color.r, frame.color.b, frame.color.g, 0), totalDuration * (2f / 3f)).SetEase(Ease.OutExpo);
        e.DOMinSize(new Vector2(e.minWidth, 0), totalDuration / 3f).SetDelay(totalDuration / 3f).OnComplete(() =>
        {
            StartCoroutine(RunParticles(frameParticles, frameParticles.main.duration));
        });
    }
    IEnumerator RunParticles(ParticleSystem system, float duration)
    {
        system.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        system.gameObject.SetActive(false);
    }

    void InstaComplete()
    {
        if (typingRoutine != null && currentWrite != null)
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
            currentSequenceIndex++;
            text.text = currentWrite;
            currentWrite = null;
            promptText.text = currentSequenceIndex >= currentSequence.lines.Length ? dismissText : nextText;

            if (currentSequence.autoTime != 0)
            {
                if (autocompleteRoutine != null)
                {
                    StopCoroutine(autocompleteRoutine);
                }
                autocompleteRoutine = StartCoroutine(AutotimeRoutine());
            } 

        }
    }

    IEnumerator AutotimeRoutine()
    {
        yield return new WaitForSeconds(currentSequence.autoTime);
        if (currentSequenceIndex >= currentSequence.lines.Length)
        {
            if (currentSequence.flagOnEnd != null)
            {
                EventManager.instance.FireEvent(currentSequence.flagOnEnd);
            }
            currentSequence = null;
            currentSequenceIndex = 0;
            if (queue.Count > 0)
            {
                StartDialogueSequence(queue[0]);
                queue.RemoveAt(0);
            }
            else
            {
                FadeOutText();
            }
        }
        else
        {
            currentSequenceIndex++;
            SetIconAndWrite(currentSequence.lines[currentSequenceIndex]);
        }
    }

    void Interaction()
    {
        if (currentSequence == null) return;
        if (typingRoutine != null)
        {
            InstaComplete();
        } else
        {
            if (currentSequenceIndex >= currentSequence.lines.Length)
            {
                if (currentSequence.flagOnEnd != null)
                {
                    EventManager.instance.FireEvent(currentSequence.flagOnEnd);
                }
                currentSequence = null;
                if (autocompleteRoutine != null)
                {
                    StopCoroutine(autocompleteRoutine);
                }
                currentSequenceIndex = 0;
                if (queue.Count > 0)
                {
                    StartDialogueSequence(queue[0]);
                    queue.RemoveAt(0);
                }
                else
                {
                    FadeOutText();
                }

            } else
            {
                currentSequenceIndex++;
                SetIconAndWrite(currentSequence.lines[currentSequenceIndex]);
            }
        }
    }
}
