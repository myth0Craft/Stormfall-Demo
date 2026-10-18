using UnityEngine;

public class LightTransition : MonoBehaviour
{
    public float exitIntensity;
    public float enterIntensity;

    public float duration;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GlobalLightManager.instance.ChangeIntensity(enterIntensity, duration);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GlobalLightManager.instance.ChangeIntensity(exitIntensity, duration);
        }
    }
}
