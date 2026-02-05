using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class PlayerAttackDamageObject : MonoBehaviour
{

    [SerializeField] PlayerMovement playerMovement;

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
            camShakeSource.AddScreenShake(0.02f);
        }
        if (enemyHealth != null)
        {
            enemyHealth.ApplyDamage();
            camShakeSource.AddScreenShake(0.04f);
            StartCoroutine(hitStopCoroutine());
        }
        //}
    }

    

    public IEnumerator hitStopCoroutine()
    {
        Time.timeScale = 0.0f;
        yield return new WaitForSecondsRealtime(0.05f);
        Time.timeScale = 1.0f;
    }
}
