using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class EventEmitter : MonoBehaviour
{
    [Header("Trigger Config")]
    public bool isOneShot = true;
    public float cooldown = 0.0f;
    float cdTimer;
    bool fired = false;
    public string triggerTag = "Player";
    public List<string> triggers = new List<string>();
    [Header("Flag Config")]
    public string enterFlagToEmit;
    public string exitFlagToEmit;


    private void Start()
    {
        string[] triggerTags = triggerTag.Split(',');
        foreach (string triggerTag in triggerTags)
        {
            triggers.Add(triggerTag);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (enterFlagToEmit == null) {  return; }
        
        if (isOneShot && fired) { ; return; }
        if (!isOneShot && cooldown > cdTimer) {  return; }

        if (triggers.Contains(other.tag))
        {
            EventManager.instance.FireEvent(enterFlagToEmit);
            fired = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (exitFlagToEmit == null) return;
        if (isOneShot && fired) return;
        if (!isOneShot && cooldown > cdTimer) return;
        if (triggers.Contains(other.tag))
        {
            EventManager.instance.FireEvent(exitFlagToEmit);
            fired = true;
        }
    }

    private void Update()
    {
        cdTimer += Time.deltaTime;
    }


}
