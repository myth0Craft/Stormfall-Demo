using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerHealthManager : HealthManager
{

    private SpriteRenderer[] spriteRenderers;
    public Material defaultMat;
    public Material hurtMat;
    public ParticleSystem hitParticle;
    private CamShakeSource camShakeSource;

    private void Awake()
    {
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
        print("died");
    }

    protected override void AddHitEffects()
    {
        camShakeSource.AddScreenShake(0.04f);
        StartCoroutine(HitColorCoroutine());
        StartCoroutine(hitParticles());
        StartCoroutine(hitStopCoroutine());
    }

    public IEnumerator hitStopCoroutine()
    {
        Time.timeScale = 0.0f;
        yield return new WaitForSecondsRealtime(0.05f);
        Time.timeScale = 1.0f;
    }

    public IEnumerator hitParticles()
    {
        hitParticle.enableEmission = true;
        yield return new WaitForSecondsRealtime(0.5f);
        hitParticle.enableEmission = false;
    }

    public IEnumerator HitColorCoroutine()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int x = 0; x < spriteRenderers.Length; x++)
            {
                spriteRenderers[x].material = hurtMat;
            }

            yield return new WaitForSecondsRealtime(0.1f);
            for (int x = 0; x < spriteRenderers.Length; x++)
            {
                spriteRenderers[x].material = defaultMat;
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}