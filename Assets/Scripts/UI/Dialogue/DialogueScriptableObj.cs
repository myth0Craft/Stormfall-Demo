using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueEntry", menuName = "Game/Dialogue")]
public class DialogueScriptableObj : ScriptableObject
{
    public List<DialogueEntry> entries;
}
