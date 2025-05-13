using UnityEngine;
using UnityEngine.Events;
public class TerribleSyringeCode : MonoBehaviour
{
    public UnityEvent syringeHit;
    public UnityEvent syringeFinish;
    private void Hit()
    {
        syringeHit.Invoke();
    }

    void Finish()
    {
        syringeFinish.Invoke();
    }
}
