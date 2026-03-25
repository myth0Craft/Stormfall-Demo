using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using Unity.VisualScripting;

public class MusicManager : MonoBehaviour
{
    public AudioSource music;

    public static MusicManager instance;

    public bool currentlyPlaying = false;
    private Coroutine currentFade;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this.gameObject);
        }


        if (music != null)
        {
            if (currentlyPlaying)
            {
                music.Play();
            } else
            {
                music.Stop();
            }
            
        }
    }

    public void FadeIn(float duration, float endVolume)
    {
        if (currentlyPlaying) return;

        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeInCoroutine(duration, endVolume));
    }

    public void FadeOut(float duration)
    {
        if (!currentlyPlaying) return;
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float durationSeconds)
    {
        float startVolume = music.volume;

        float endVolume = 0f;
        float elapsedTime = 0f;


        while (elapsedTime < durationSeconds)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / durationSeconds;
            music.volume = Mathf.Lerp(startVolume, endVolume, t);
            yield return null;
        }

        music.Stop();
        currentlyPlaying = false;
        music.volume = endVolume;
    }

    private IEnumerator FadeInCoroutine(float durationSeconds, float endVolume)
    {
        float startVolume = 0f;
        float elapsedTime = 0f;

        music.volume = 0f;
        music.Play();

        while (elapsedTime < durationSeconds)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / durationSeconds;
            music.volume = Mathf.Lerp(startVolume, endVolume, t);
            yield return null;
        }

        music.volume = endVolume;
        currentlyPlaying = true;
    }


}
