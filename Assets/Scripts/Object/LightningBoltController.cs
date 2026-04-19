
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightningBoltController : MonoBehaviour
{

    private SpriteRenderer sprite;
    private Light2D light;
    private System.Random rand = new System.Random();

    private float minSecondsBetweenStrikes = 1;
    private float maxSecondsBetweenStrikes = 5;
    private float lightningFadeSeconds = 1;

    private float secondsUntilActive;

    private float lightIntensity;
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        light = GetComponent<Light2D>();
        if (light == null)
        {
            light = GetComponentInChildren<Light2D>();
        }
        lightIntensity = light.intensity;
        if (sprite != null)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);
        }
        StartCoroutine(LightningBoltCoroutine());
    }

    private IEnumerator LightningBoltCoroutine()
    {

        while (true)
        {
            secondsUntilActive = (float)rand.NextDouble() * (maxSecondsBetweenStrikes - minSecondsBetweenStrikes) + minSecondsBetweenStrikes;
            yield return new WaitForSeconds(secondsUntilActive);

            yield return ActivateLightningBoltCoroutine();
        }


    }

    private IEnumerator ActivateLightningBoltCoroutine()
    {
        float elapsedTime = 0f;
        if (sprite != null)
        {
            sprite.color = new Color (sprite.color.r, sprite.color.g, sprite.color.b, 1f);
        }
        if (light != null)
        {
            light.intensity = lightIntensity;
        }

        while (elapsedTime < lightningFadeSeconds)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / lightningFadeSeconds);
            if (sprite != null)
            {
                sprite.color = Color.Lerp(new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f), new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f), t);
            }

            light.intensity = Mathf.Lerp(lightIntensity, 0, t);
            yield return null;
        }

    }
}
