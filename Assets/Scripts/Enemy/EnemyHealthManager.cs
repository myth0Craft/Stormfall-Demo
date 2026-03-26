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
        Destroy(this.gameObject);

        AddParticles();
    }

    protected override void AddHitEffects()
    {
        StartCoroutine(HitColorCoroutine());
    }

    public IEnumerator HitColorCoroutine()
    {
        AudioSource.PlayClipAtPoint(hurtSound, transform.position, 10.0f);
        AddParticles();
        spriteRenderer.material = hurtMaterial;
        yield return new WaitForSecondsRealtime(0.15f);
        spriteRenderer.material = defaultMaterial;
    }

    private void AddParticles()
    {
        if (deathParticlesPrefab != null)
        {
            Destroy(particleInstance);
            particleInstance = Instantiate(
                deathParticlesPrefab,
                transform.position,
                Quaternion.identity
            );
            
        }
    }

}
