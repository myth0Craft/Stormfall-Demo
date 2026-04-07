using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaBattleTrigger : MonoBehaviour
{
    public ArenaGate[] arenaGates;
    public CinemachineCamera arenaCam;
    public float secondsBetweenWaves = 1.0f;
    public float startDelay = 0.0f;
    [SerializeField] public EnemyWave[] waves;

    public ArenaBattleStartEffects startEffects;

    private bool arenaBattleActive = false;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !arenaBattleActive)
        {
            StartArenaBattle();
        }
    }

    public void StartArenaBattle()
    {
        arenaBattleActive = true;
        for (int i = 0; i < arenaGates.Length; i++)
        {
            arenaGates[i].BeginArenaBattle();
        }
        arenaCam.Priority = 20;
        
        StartCoroutine(ArenaFightCoroutine());
    }

    public void EndArenaBattle()
    {
        for (int i = 0; i < arenaGates.Length;i++)
        {
            arenaGates[i].EndArenaBattle();
        }
        arenaCam.Priority = 0;
        arenaBattleActive = false;
    }

    public IEnumerator ArenaFightCoroutine()
    {
        yield return new WaitForSecondsRealtime(startDelay);

        if (startEffects != null)
        {
            yield return startEffects.AddArenaBattleStartEffects();

        }

        

        for (int i = 0; i < waves.Length;i++)
        {
            yield return new WaitForSecondsRealtime(secondsBetweenWaves);
            for (int j = 0; j < waves[i].enemies.Length; j++)
            {
                GameObject enemy = Instantiate(
                    waves[i].enemies[j].enemy,
                    waves[i].enemies[j].worldPosition,
                    Quaternion.identity
                );
                SceneManager.MoveGameObjectToScene(enemy, gameObject.scene);
            }

            
        }
        //EndArenaBattle();
        yield return null;
    }
}
