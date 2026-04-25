using UnityEngine;

public class MusicFaderCollider : MonoBehaviour
{
    public bool fadeIn = false;


    public float fadeDuration;

    public float endVolume = 0.5f;

    public AudioClip musicToSwitch;

    private void Awake()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (musicToSwitch != null)
            {
                MusicManager.instance.music.clip = musicToSwitch;
            }
            if (fadeIn)
            {
                MusicManager.instance.FadeIn(fadeDuration, endVolume);
            }
            else
            {
                MusicManager.instance.FadeOut(fadeDuration);
            }
        }
    }
}
