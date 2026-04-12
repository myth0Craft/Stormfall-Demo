using UnityEngine;

public abstract class BaseDialogueCondition : ScriptableObject
{
    public abstract bool IsMet(string npcId);
}
