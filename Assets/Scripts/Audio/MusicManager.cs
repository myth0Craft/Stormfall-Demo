using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using Unity.VisualScripting;

public class MusicManager : MonoBehaviour
{
    public AudioClip titleScreenMusic;
    public AudioClip gameMusic;

    private AudioClip currentClip;

    public AudioSource music;

    public static MusicManager instance;

    public bool inTitleScreen = false;
    public bool currentlyPlaying = false;
    private Coroutine currentFade;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }

        


        if (titleScreenMusic != null && gameMusic != null && music != null)
        {

            if (inTitleScreen)
            {
                music.clip = titleScreenMusic;
            } else
            {
                music.clip = gameMusic;
            }

            if (currentlyPlaying)
            {
                music.Play();
            }
            else
            {
                music.Stop();
            }
            
        }
    }

    public void SetMusic(bool isTitle)
    {
        inTitleScreen = isTitle;

        AudioClip targetClip = inTitleScreen ? titleScreenMusic : gameMusic;

        if (music.clip != targetClip)
        {
            music.clip = targetClip;
        }
    }

    public void FadeIn(float duration, float endVolume)
    {
        if (music.isPlaying && music.volume > 0.01f) return;

        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeInCoroutine(duration, endVolume));
    }



    public void FadeOut(float duration)
    {
        if (!music.isPlaying && music.volume < 0.01f) return;

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
