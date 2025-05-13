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
    [Header("Flag Config")]
    public string enterFlagToEmit;
    public string exitFlagToEmit;


    private void OnTriggerEnter(Collider other)
    {
        if (enterFlagToEmit == null) return;
        if (isOneShot && fired) return;
        if (!isOneShot && cooldown > cdTimer) return;
        if (other.CompareTag(triggerTag))
        {
            EventManager.instance.FireEvent(enterFlagToEmit);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (exitFlagToEmit == null) return;
        if (isOneShot && fired) return;
        if (!isOneShot && cooldown > cdTimer) return;
        if (other.CompareTag(triggerTag))
        {
            EventManager.instance.FireEvent(exitFlagToEmit);
        }
    }

    private void Update()
    {
        cdTimer += Time.deltaTime;
    }


}
