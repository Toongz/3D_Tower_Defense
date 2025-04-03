using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Serializable]
    public class EnemyWave
    {
        public GameObject enemyPrefab;
        public int baseCount = 5;
    }

    [SerializeField] private List<EnemyWave> enemyTypes = new();
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float difficultyIncrement = 0.2f;
    [SerializeField] private int preSpawnCount = 5;

    private int currentWave;
    private int totalEnemiesThisWave;
    private int enemiesSpawned;
    private int enemiesKilled;
    private bool isWaveActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PreloadEnemies();
        StartNextWave();
    }

    private void Update()
    {
        if (isWaveActive && enemiesKilled >= totalEnemiesThisWave)
        {
            isWaveActive = false;
            Invoke(nameof(StartNextWave), 2f);
        }
    }

    public void StartNextWave()
    {
        currentWave++;
        isWaveActive = true;

        totalEnemiesThisWave = 0;
        foreach (var enemyType in enemyTypes)
        {
            totalEnemiesThisWave += Mathf.RoundToInt(enemyType.baseCount * (1 + difficultyIncrement * (currentWave - 1)));
        }

        enemiesSpawned = 0;
        enemiesKilled = 0;

        WaveUIManager.Instance.UpdateWave(currentWave);
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        int enemyIndex = 0;
        while (enemiesSpawned < totalEnemiesThisWave)
        {
            SpawnEnemy(enemyTypes[enemyIndex]);
            enemyIndex = (enemyIndex + 1) % enemyTypes.Count;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy(EnemyWave enemyWave)
    {
        if (enemyWave?.enemyPrefab == null) return;

        GameObject enemy = EnemyPool.Instance.GetEnemy<EnemyBase>(enemyWave.enemyPrefab);
        enemy.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        int statMultiplier = Mathf.RoundToInt(1 + difficultyIncrement * (currentWave - 1));
        enemy.GetComponent<EnemyBase>().Initialize(waypoints, statMultiplier);
        enemiesSpawned++;
    }

    public void EnemyKilled() => enemiesKilled++;

    private void PreloadEnemies()
    {
        foreach (var enemyType in enemyTypes)
        {
            EnemyPool.Instance.PreloadEnemies<EnemyBase>(enemyType.enemyPrefab, preSpawnCount);
        }
    }

    public int GetCurrentWave() => currentWave;
    public bool IsWaveActive() => isWaveActive;
}
