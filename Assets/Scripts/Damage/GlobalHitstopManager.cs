using System.Collections;
using UnityEngine;

public class GlobalHitstopManager : MonoBehaviour
{
    private static GlobalHitstopManager instance;

    private Coroutine active;

    private void Awake()
    {
        instance = this;
    }

    public static void DoHitstop(float duration)
    {
        
        if (instance.active != null)
            instance.StopCoroutine(instance.active);

        instance.active = instance.StartCoroutine(instance.Hitstop(duration));
    }

    private IEnumerator Hitstop(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    public static void DoHitslow(float duration, float timescale)
    {

        if (instance.active != null)
            instance.StopCoroutine(instance.active);

        instance.active = instance.StartCoroutine(instance.Hitslow(duration, timescale));
    }

    private IEnumerator Hitslow(float duration, float timescale)
    {
        Time.timeScale = timescale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}
