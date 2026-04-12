using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueEntry
{
    [TextArea]
    public List<string> text;

    public BaseDialogueCondition condition;

    public int priority;
}
