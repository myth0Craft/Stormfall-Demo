using Unity.Cinemachine;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{

    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private PlayerMeleeAttack playerAttack;

    [SerializeField] private Animator attackAnimator;

    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void disableAttackQueued()
    {
        attackAnimator.SetBool("attackQueued", false);
    }

    public void EnableSword()
    {
        playerMovement.enableSword();
    }

    public void DisableSword()
    {
        playerMovement.disableSword();
    }

    public void setMidAttackFalse()
    {
        playerAttack.isMidAttack = false;
    }

    //called from 2nd sword swing animation to set player to mid attack state, preventing dashing during the sword swing
    public void setMidAttackTrue()
    {
        playerAttack.isMidAttack = true;
    }

    public void triggerAttackScreenShake()
    {
        float xForce = playerMovement.getFacingDirection() ? -0.008f : 0.008f;
        if (playerMovement.getLinearVelocity().x > 0.01f || playerMovement.getLinearVelocity().x < -0.01f)
        {
            xForce *= 3;
        }
        Vector3 force = new Vector3(xForce, 0.02f, 0);


        impulseSource.GenerateImpulse(force);

        playerAttack.ApplyDamage();
    }

}
