using UnityEngine;

public class MusicFaderCollider : MonoBehaviour
{
    public bool fadeIn = false;


    public float fadeDuration;

    public float endVolume = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

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
