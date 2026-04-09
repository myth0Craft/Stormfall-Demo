using System;
using UnityEngine;

[System.Serializable]
public class EnemyWave
{
    public string waveName;
    public EnemyConfig[] enemies;
    public float spawnDurationSeconds = 1.0f;
    public GameObject spawnParticles;
}
