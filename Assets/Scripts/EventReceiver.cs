using UnityEngine;
using UnityEngine.Events;

public class EventReceiver : MonoBehaviour
{
    public string flagToReceive;
    [SerializeField] UnityEvent functionToCall;

    private void Start()
    {
        EventManager.instance.worldEvent.AddListener(EventCalled);
    }

    private void OnDisable()
    {
        EventManager.instance.worldEvent.RemoveListener(EventCalled);
    }

    void EventCalled(string flag)
    {
        if (flag == flagToReceive)
        {
            functionToCall.Invoke();
        }
    }
}
