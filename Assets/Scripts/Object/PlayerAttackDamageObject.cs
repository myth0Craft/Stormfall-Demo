using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerAttackDamageObject : MonoBehaviour
{

    public GameObject sparkParticles;
    private GameObject currentSparkInstance;

    private CinemachineImpulseSource impulseSource;

    private CamShakeSource camShakeSource;

    void Awake()
    {
        camShakeSource = GameObject.FindGameObjectWithTag("CinemachineImpulseSource").GetComponent<CamShakeSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        /*if (other.CompareTag("BreakableObj"))
        {*/
        BreakableObject health = other.GetComponent<BreakableObject>();
        EnemyHealthManager enemyHealth = other.GetComponent <EnemyHealthManager>();
        if (health != null)
        {
            health.ApplyDamage();
            camShakeSource.AddScreenShake(0.04f);
        }
        if (enemyHealth != null)
        {
            enemyHealth.ApplyDamage();
            camShakeSource.AddScreenShake(0.08f);

            GlobalHitstopManager.DoHitstop(0.05f);
            //StartCoroutine(hitStopCoroutine());
            if (currentSparkInstance != null)
            {
                Destroy(currentSparkInstance.gameObject);
            }

            currentSparkInstance = Instantiate(
                sparkParticles,
                transform.position,
                Quaternion.identity
            );
            StartCoroutine(DestroySparkParticleCoroutine());
            
        }
        //}
    }

    public IEnumerator DestroySparkParticleCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(currentSparkInstance.gameObject);
    }

    

    //public IEnumerator hitStopCoroutine()
    //{
    //    Time.timeScale = 0.0f;
    //    yield return new WaitForSecondsRealtime(0.05f);
    //    Time.timeScale = 1.0f;
    //}
}
