using NUnit.Framework;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Objects/Dialogue Sequence")]
public class DialogueSequence : ScriptableObject
{
    public string flagOnStart;
    public string[] lines;
    public string flagOnEnd;
    public int autoTime = 0;
    public bool overwritable = true;
}
