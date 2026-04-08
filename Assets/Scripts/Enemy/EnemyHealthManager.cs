using System.Collections;
using UnityEngine;

public class EnemyHealthManager : HealthManager
{
    public Material defaultMaterial;
    public Material hurtMaterial;
    public GameObject deathParticlesPrefab;
    private GameObject particleInstance;
    private SpriteRenderer spriteRenderer;
    public bool shouldSaveAcrossRooms = false;
    [SerializeField] private string id;
    public AudioClip hurtSound;
    public GameObject hitParticlesPrefab;


    public override void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (maxHealth < 0) maxHealth = 5;
        currentHealth = maxHealth;


        if (shouldSaveAcrossRooms)
        {
            if (id == null)
            {
                Debug.Log("Id of Enemy is null!");
            }
            else
            {
                var room = SaveSystem.getRoom(gameObject.scene.name);

                //if (room.breakables.TryGetValue(id, out bool broken) && broken)
                //{
                //    Destroy(gameObject);
                //}
            }
        }
    }

    public override void Die()
    {

        if (shouldSaveAcrossRooms)
        {
            if (id == null)
            {
                Debug.Log("Id of Enemy is null!");
            }
            else
            {
                var room = SaveSystem.getRoom(gameObject.scene.name);
                //room.breakables[id] = true;
            }
        }

        print("enemy killed");
        AddParticles(deathParticlesPrefab);
        Destroy(transform.parent.gameObject);

        
    }

    protected override void AddHitEffects()
    {
        StartCoroutine(HitColorCoroutine());
    }

    public IEnumerator HitColorCoroutine()
    {
        AudioSource.PlayClipAtPoint(hurtSound, transform.position, 10.0f);
        AddParticles(hitParticlesPrefab);
        spriteRenderer.material = hurtMaterial;
        yield return new WaitForSecondsRealtime(0.15f);
        spriteRenderer.material = defaultMaterial;
    }

    private void AddParticles(GameObject particleInstance)
    {
        if (particleInstance != null && particleInstance.GetComponent<ParticleSystem>() != null)
        {
            GameObject instance = Instantiate(particleInstance, transform.position, Quaternion.identity);
            instance.GetComponent<ParticleSystem>().Play();
        }
    }

}
