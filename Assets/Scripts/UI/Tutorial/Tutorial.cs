using UnityEngine;

public enum TutorialCompletionAction
{
    None,
    Jump,
    Attack,
    AddBlockEffects
}

[CreateAssetMenu(fileName = "Tutorial", menuName = "Scriptable Objects/Tutorial")]
public class Tutorial : ScriptableObject
{
    [Header("Tutorial")]
    public string tutorialID;

    [Header("Completion")]
    public TutorialCondition condition;
    public TutorialCompletionAction completionAction;

    [Header("UI")]

    public string text;

    public bool pauseGame = false;

    public bool focusedOnStamina = false;
}