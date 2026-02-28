using Unity.Cinemachine;
using UnityEngine;

public class CamShakeSource : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void AddScreenShake(float amount)
    {
        float xForce = playerMovement.getFacingDirection() ? -amount : amount;
        if (playerMovement.getLinearVelocity().x > 0.01f || playerMovement.getLinearVelocity().x < -0.01f)
        {
            xForce *= 3;
        }
        Vector3 force = new Vector3(xForce, amount, 0);


        impulseSource.GenerateImpulse(force);
    }

    public void AddVerticalScreenShake(float amount)
    {
        Vector3 force = new Vector3(0, amount, 0);
        impulseSource.GenerateImpulse(force);
    }
}
