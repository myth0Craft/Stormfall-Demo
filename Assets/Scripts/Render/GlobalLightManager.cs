using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightManager : MonoBehaviour
{
    private Light2D globalLight;

    public static GlobalLightManager instance;

    private readonly float defaultIntensity = 1.0f;

    private Coroutine intensityCoroutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        globalLight = GetComponent<Light2D>();

        globalLight.intensity = defaultIntensity;
    }

    public void ChangeIntensity(float newIntensity, float duration)
    {
        if (intensityCoroutine != null)
        {
            StopCoroutine(intensityCoroutine);
        }

        intensityCoroutine = StartCoroutine(ChangeIntensityCoroutine(newIntensity, duration));
    }

    private IEnumerator ChangeIntensityCoroutine(float newIntensity, float duration)
    {
        float startIntensity = globalLight.intensity;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            float percent = elapsedTime / duration;

            globalLight.intensity = Mathf.Lerp(startIntensity, newIntensity, percent);

            yield return null;
        }

        globalLight.intensity = newIntensity;

        intensityCoroutine = null;
    }
}
