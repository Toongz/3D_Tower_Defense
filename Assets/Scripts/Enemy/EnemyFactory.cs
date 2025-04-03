using UnityEngine;
public enum EnemyType
{
    FastEnemy,
    ArmoredEnemy,
    ExplodingEnemy
}
public class EnemyFactory : MonoBehaviour
{
    public static EnemyFactory Instance { get; private set; }
    [SerializeField] private GameObject fastEnemyPrefab;
    [SerializeField] private GameObject armoredEnemyPrefab;
    [SerializeField] private GameObject explodingEnemyPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Creates an enemy of the specified type
    /// </summary>
    public GameObject CreateEnemy(EnemyType type, Transform[] path, int multiplier)
    {
        if (path == null || path.Length == 0)
        {
            Debug.LogError("EnemyFactory: Invalid path provided!");
            return null;
        }

        GameObject prefab = type switch
        {
            EnemyType.FastEnemy => fastEnemyPrefab,
            EnemyType.ArmoredEnemy => armoredEnemyPrefab,
            EnemyType.ExplodingEnemy => explodingEnemyPrefab,
            _ => null
        };

        if (prefab == null)
        {
            Debug.LogError($"EnemyFactory: Prefab missing for type: {type}");
            return null;
        }

        GameObject enemyObject = Instantiate(prefab);
        if (enemyObject.TryGetComponent<EnemyBase>(out var enemy))
        {
            enemy.Initialize(path, multiplier);
            return enemyObject;
        }

        Debug.LogError($"EnemyFactory: Prefab {prefab.name} has no EnemyBase component!");
        Destroy(enemyObject);
        return null;
    }

    private void OnValidate()
    {
        if (fastEnemyPrefab == null) Debug.LogWarning("FastEnemy prefab not assigned!", this);
        if (armoredEnemyPrefab == null) Debug.LogWarning("ArmoredEnemy prefab not assigned!", this);
        if (explodingEnemyPrefab == null) Debug.LogWarning("ExplodingEnemy prefab not assigned!", this);
    }
}
