using UnityEngine;

public class BreakableObject : HealthManager
{

    public GameObject breakParticlesPrefab;
    public GameObject hitParticlesPrefab;
    public bool saveState = false;
    [SerializeField] private string id;

    public override void Awake()
    {
        if (maxHealth < 0) maxHealth = 5;
        currentHealth = maxHealth;


        if (saveState)
        {
            if (id == null)
            {
                Debug.Log("Id of Breakable Object is null!");
            } else
            {
                var room = SaveSystem.getRoom(gameObject.scene.name);

                if (room.breakables.TryGetValue(id, out bool broken) && broken)
                {
                    Destroy(gameObject);
                }
            }
        }
    }


    public override void Die()
    {

        if (saveState)
        {
            if (id == null)
            {
                Debug.Log("Id of Breakable Object is null!");
            } else
            {
                var room = SaveSystem.getRoom(gameObject.scene.name);
                room.breakables[id] = true;
            }
        }

        print("object broken");
        Destroy(this.gameObject);

        if (breakParticlesPrefab != null && breakParticlesPrefab.GetComponent<ParticleSystem>() != null)
        {
            GameObject instance = Instantiate(breakParticlesPrefab, transform.position, Quaternion.identity);
            instance.GetComponent<ParticleSystem>().Play();
        }
    }

    protected override void AddHitEffects()
    {
        
        if (currentHealth > 0 && hitParticlesPrefab != null && hitParticlesPrefab.GetComponent<ParticleSystem>() != null)
        {
            GameObject instance = Instantiate(hitParticlesPrefab, transform.position, Quaternion.identity);
            instance.GetComponent<ParticleSystem>().Play();
        }
    }
}
