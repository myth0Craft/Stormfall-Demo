using System.Collections;
using UnityEngine;

public class EnemyHealthManager : HealthManager, IKnockbackable
{
    public Material defaultMaterial;
    public Material hurtMaterial;
    public GameObject deathParticlesPrefab;
    private GameObject particleInstance;
    protected SpriteRenderer spriteRenderer;
    public bool shouldSaveAcrossRooms = false;
    [SerializeField] protected string id;
    public AudioClip hurtSound;
    public GameObject hitParticlesPrefab;
    protected BoxCollider2D hitCollider;
    public GameObject gameObjectToDestroy;
    public float xKnockbackForce = 0;
    public float yKnockbackForce = 0;
    public Rigidbody2D body;


    public override void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitCollider = GetComponent<BoxCollider2D>();
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
        if (gameObjectToDestroy != null)
        {
            Destroy(gameObjectToDestroy);
        }


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

    protected void AddParticles(GameObject particleInstance)
    {
        if (particleInstance != null && particleInstance.GetComponent<ParticleSystem>() != null)
        {
            GameObject instance = Instantiate(particleInstance, transform.position, Quaternion.identity);
            instance.GetComponent<ParticleSystem>().Play();
        }
    }

    public void AddKnockback()
    {
        bool facingRight = PlayerMovement.instance.getFacingDirection();

        float xMultiplier = facingRight ? 1 : -1;

        if (body != null)
        {
            Debug.Log("Added force");
            body.AddForce(new Vector2(xKnockbackForce * xMultiplier, yKnockbackForce), ForceMode2D.Impulse);
        }
    }
}
