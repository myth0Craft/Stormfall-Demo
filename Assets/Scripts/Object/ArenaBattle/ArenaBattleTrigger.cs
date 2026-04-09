using System.Collections;
using System.Collections.Generic;
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

    public bool arenaBattleComplete = false;
    [SerializeField] private string id;

    public ArenaBattleStartEffects startEffects;

    private bool arenaBattleActive = false;

    public void Awake()
    {
        if (id == null)
        {
            Debug.Log("Id of Arena trigger is null at scene " + gameObject.scene.name);
        }
        else
        {
            var room = SaveSystem.getRoom(gameObject.scene.name);

            if (room.pickups.TryGetValue(id, out bool completed) && completed)
            {
                arenaBattleComplete = true;
                
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !arenaBattleActive && !arenaBattleComplete)
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
        arenaBattleComplete = true;
        for (int i = 0; i < arenaGates.Length;i++)
        {
            arenaGates[i].EndArenaBattle();
        }
        arenaCam.Priority = 0;
        arenaBattleActive = false;

        if (id == null)
        {
            Debug.Log("Id of Arena trigger is null at scene " + gameObject.scene.name);
        }
        else
        {
            var room = SaveSystem.getRoom(gameObject.scene.name);
            room.pickups[id] = true;
        }
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

            List<GameObject> enemiesThisWave = new List<GameObject>();
            List<ParticleSystem> spawnParticlesThisWave = new List<ParticleSystem>();
            yield return new WaitForSecondsRealtime(secondsBetweenWaves);


            //Loads particle instances defined in the wave config at each enemy postion defined in the enemy config.
            if (waves[i].spawnParticles != null)
            {
                for (int j = 0; j < waves[i].enemies.Length; j++)
                {
                    GameObject spawnParticles = Instantiate(
                        waves[i].spawnParticles,
                        waves[i].enemies[j].worldPosition,
                        Quaternion.identity
                    );
                    SceneManager.MoveGameObjectToScene(spawnParticles, gameObject.scene);
                    if (spawnParticles.GetComponent<ParticleSystem>() != null)
                    {
                        spawnParticlesThisWave.Add(spawnParticles.GetComponent<ParticleSystem>());
                    }
                }
            }

            foreach (var particle in spawnParticlesThisWave)
            {
                particle.Play();
            }


            yield return new WaitForSecondsRealtime(waves[i].spawnDurationSeconds);

            //Stops particles after specified enemy spawning duration defined in the wave config.
            foreach (var particle in spawnParticlesThisWave)
            {
                particle.Stop();
            }

            for (int j = 0; j < waves[i].enemies.Length; j++)
            {
                GameObject enemy = Instantiate(
                    waves[i].enemies[j].enemy,
                    waves[i].enemies[j].worldPosition,
                    Quaternion.identity
                );
                SceneManager.MoveGameObjectToScene(enemy, gameObject.scene);
                enemiesThisWave.Add(enemy);
            }

            yield return new WaitUntil(() =>
            {
                enemiesThisWave.RemoveAll(e => e == null);
                return enemiesThisWave.Count == 0;
            });


        }
        EndArenaBattle();
    }
}
