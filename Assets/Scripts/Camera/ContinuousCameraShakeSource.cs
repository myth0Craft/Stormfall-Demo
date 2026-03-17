using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class ContinuousCameraShakeSource : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;

    public static ContinuousCameraShakeSource instance { get; private set; }

    public float currentForce = 0f;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        } else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    

    public void AddScreenShake(float amount)
    {
        float xForce = PlayerMovement.instance.getFacingDirection() ? -amount : amount;
        if (PlayerMovement.instance.getLinearVelocity().x > 0.01f || PlayerMovement.instance.getLinearVelocity().x < -0.01f)
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

    public void AddScreenShakeOverTime(float amount, float durationSeconds, float frequencySeconds)
    {
        StartCoroutine(AddScreenShakeOverTimeCoroutine(amount, durationSeconds, frequencySeconds));
    }

    

    private IEnumerator AddScreenShakeOverTimeCoroutine(float amount, float durationSeconds, float frequencySeconds)
    {
        currentForce = amount;

        float elapsedTime = 0f;
        float nextShakeTime = 0f;

        while (elapsedTime < durationSeconds)
        {
            if (elapsedTime >= nextShakeTime)
            {
                AddScreenShake(currentForce);
                nextShakeTime += frequencySeconds;
            }

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
