using UnityEngine;

public abstract class TutorialCondition : ScriptableObject
{
    public abstract bool IsComplete();

    public abstract void StartListening();

    public abstract void StopListening();
}
