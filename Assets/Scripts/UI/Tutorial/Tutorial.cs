using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial", menuName = "Scriptable Objects/Tutorial")]
public class Tutorial : ScriptableObject
{
    [Header("Tutorial")]
    public string tutorialID;

    [Header("Completion")]
    public TutorialCondition condition;

    [Header("UI")]

    public string text;

    public bool pauseGame = false;

    public bool focusedOnStamina = false;
}