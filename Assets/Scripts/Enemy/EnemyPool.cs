using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }
    private Dictionary<Type, List<GameObject>> pool = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public GameObject GetEnemy<T>(GameObject prefab) where T : EnemyBase
    {
        if (prefab == null)
        {
            Debug.LogError("EnemyPool: Prefab is null!");
            return null;
        }

        Type enemyType = typeof(T);
        if (pool.TryGetValue(enemyType, out var list))
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].activeInHierarchy)
                {
                    GameObject enemy = list[i];
                    enemy.SetActive(true);
                    return enemy;
                }
            }
        }
        GameObject newEnemy = Instantiate(prefab, transform); 
        if (!pool.ContainsKey(enemyType))
        {
            pool[enemyType] = new List<GameObject>();
        }
        pool[enemyType].Add(newEnemy);
        return newEnemy;
    }

    public void ReturnEnemy<T>(GameObject obj) where T : EnemyBase
    {
        if (obj == null) return;

        Type enemyType = typeof(T);
        if (!pool.TryGetValue(enemyType, out var list))
        {
            list = new List<GameObject>();
            pool[enemyType] = list;
        }

        if (!list.Contains(obj))
        {
            list.Add(obj);
            obj.transform.SetParent(transform); 
        }

        obj.SetActive(false);
    }

    public void PreloadEnemies<T>(GameObject prefab, int amount) where T : EnemyBase
    {
        if (prefab == null || amount <= 0) return;

        Type enemyType = typeof(T);
        if (!pool.TryGetValue(enemyType, out var list))
        {
            list = new List<GameObject>();
            pool[enemyType] = list;
        }

        for (int i = 0; i < amount; i++)
        {
            GameObject enemy = Instantiate(prefab, transform); 
            enemy.SetActive(false);
            list.Add(enemy);
        }
    }

    public void ClearPool()
    {
        foreach (var list in pool.Values)
        {
            foreach (var obj in list)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            list.Clear();
        }
        pool.Clear();
    }

    private void OnDestroy() => ClearPool();
}