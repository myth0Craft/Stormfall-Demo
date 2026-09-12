using UnityEngine;

[CreateAssetMenu(fileName = "PlayerHasAbilityCondition", menuName = "Conditions/PlayerHasAbilityCondition")]
public class PlayerHasAbilityCondition : ConditionBase
{
    public PlayerAbilities ability;
    
    public override bool IsMet()
    {
        switch (ability)
        {
            case PlayerAbilities.Dash:
                return PlayerData.dashUnlocked;

            case PlayerAbilities.Sprint:
                return PlayerData.sprintUnlocked;

            case PlayerAbilities.Sword:
                return PlayerData.swordUnlocked;

            case PlayerAbilities.DoubleJump:
                return PlayerData.doubleJumpUnlocked;

            case PlayerAbilities.WallJump:
                return PlayerData.wallJumpUnlocked;

            case PlayerAbilities.Shield:
                return PlayerData.shieldUnlocked;

            case PlayerAbilities.ShieldBounce:
                return PlayerData.shieldBounceUnlocked;
        }

        return false;
    }
}
