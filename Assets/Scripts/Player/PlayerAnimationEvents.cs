using Unity.Cinemachine;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    [SerializeField] private Animator swordAnim;

    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private PlayerMeleeAttack playerAttack;

    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void EnableSword()
    {
        swordAnim.SetBool("hasSword", true);
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
