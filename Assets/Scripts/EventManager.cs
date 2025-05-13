using UnityEngine;
using UnityEngine.Events;
public class EventManager : MonoBehaviour
{
    public static EventManager instance;
    [System.NonSerialized] public UnityEvent<string> worldEvent;
    void Awake()
    {
        instance = this;
        worldEvent = new UnityEvent<string>();
    }

    public void FireEvent(string flagName)
    {
        worldEvent.Invoke(flagName);
    }


}
