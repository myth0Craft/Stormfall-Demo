using System.Collections;
using UnityEngine;

public class PlayerCollisionEnterParticleTrigger : MonoBehaviour
{
    private BoxCollider2D col;
    public GameObject particleGO;
    public float effectLength;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(ParticleCoroutine(effectLength));
        }
    }

    private IEnumerator ParticleCoroutine(float secondsUntilDestroy)
    {
        GameObject particleInstance = Instantiate(particleGO, new Vector3(PlayerMovement.instance.transform.position.x, PlayerMovement.instance.transform.position.y - 0.5f, 0f), Quaternion.identity);
        particleGO.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(secondsUntilDestroy);
        Destroy(particleInstance);

    }
}
