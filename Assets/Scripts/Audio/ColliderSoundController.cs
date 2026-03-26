using UnityEngine;

public class ColliderSoundController : MonoBehaviour
{
    private AudioSource audioSource;


    public AudioClip soundToPlayOnEnter;

    public float enterSoundVolume = 1.0f;

    public float enterSoundPitch = 1.0f;

    

    public AudioClip soundToPlayOnLeave;

    public float exitSoundVolume = 1.0f;

    public float exitSoundPitch = 1.0f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerData.inWater = true;
            if (soundToPlayOnEnter != null)
            {
                audioSource.pitch = Random.Range(0.6f * enterSoundPitch, 1.4f * enterSoundPitch);
                audioSource.PlayOneShot(soundToPlayOnEnter, enterSoundVolume * 0.5f);
                
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerData.inWater = false;
            if (soundToPlayOnLeave != null)
            {
                audioSource.pitch = Random.Range(0.6f * exitSoundVolume, 1.4f * exitSoundVolume);
                audioSource.PlayOneShot(soundToPlayOnEnter, exitSoundVolume);
            }

        }
    }
}
