using System.Collections;
using UnityEngine;

public class PlayerHealthManager : HealthManager
{

    private SpriteRenderer[] spriteRenderers;
    public Material defaultMat;
    public Material hurtMat;
    public ParticleSystem hitParticle;
    private CamShakeSource camShakeSource;
    


    private bool isDead = false;

    private void Awake()
    {
        this.iFrameDuration = 1.0f;

        isDead = false;
        hitParticle.enableEmission = false;
        this.maxHealth = PlayerData.maxHealth;
        this.currentHealth = PlayerData.currentHealth;
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        camShakeSource = GameObject.FindGameObjectWithTag("CinemachineImpulseSource").GetComponent<CamShakeSource>();
    }

    public override void ApplyDamageIgnoreIFrames(int amount)
    {
        base.ApplyDamageIgnoreIFrames(amount);
        print(maxHealth + "/" + currentHealth);
    }

    public override void Die()
    {
        if (!isDead)
        {
            isDead = true;
            StopAllCoroutines();
            SceneLoader.instance.LoadGameFromPlayerDeath();
        }
    }

    /*private IEnumerator DeathCoroutine()
    {
        
        
        
        
        //yield return SceneLoader.instance.UnloadAllScenes();
        yield return SceneLoader.instance.LoadGameFromPlayerDeath();
    }*/

    protected override void AddHitEffects()
    {
        camShakeSource.AddScreenShake(0.04f);
        StartCoroutine(HitColorCoroutine());
        StartCoroutine(hitParticles());
        GlobalHitstopManager.DoHitstop(0.05f);

        //StartCoroutine(hitStopCoroutine());
    }

    //public IEnumerator hitStopCoroutine()
    //{
    //    Time.timeScale = 0.0f;
    //    yield return new WaitForSecondsRealtime(0.05f);
    //    Time.timeScale = 1.0f;
    //}

    public IEnumerator hitParticles()
    {
        hitParticle.enableEmission = true;
        yield return new WaitForSecondsRealtime(0.5f);
        hitParticle.enableEmission = false;
    }

    public IEnumerator HitColorCoroutine()
    {
        for (int i = 0; i < 2; i++)
        {
            for (int x = 0; x < spriteRenderers.Length; x++)
            {
                spriteRenderers[x].material = hurtMat;
            }

            yield return new WaitForSecondsRealtime(0.15f);
            for (int x = 0; x < spriteRenderers.Length; x++)
            {
                spriteRenderers[x].material = defaultMat;
            }
            yield return new WaitForSecondsRealtime(0.15f);
        }
        for (int i = 0; i < 4; i++)
        {
            for (int x = 0; x < spriteRenderers.Length; x++)
            {
                spriteRenderers[x].material = hurtMat;
            }

            yield return new WaitForSecondsRealtime(0.075f);
            for (int x = 0; x < spriteRenderers.Length; x++)
            {
                spriteRenderers[x].material = defaultMat;
            }
            yield return new WaitForSecondsRealtime(0.075f);
        }
    }
}