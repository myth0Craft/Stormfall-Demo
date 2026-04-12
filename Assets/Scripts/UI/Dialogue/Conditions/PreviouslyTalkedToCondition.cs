using UnityEngine;

[CreateAssetMenu(menuName = "Game/Conditions/PreviouslyTalkedTo")]
public class PreviouslyTalkedToCondition : BaseDialogueCondition
{
    public override bool IsMet(string npcId)
    {
        return PlayerData.HasTalkedTo(npcId);
    }
}
