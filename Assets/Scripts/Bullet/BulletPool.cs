using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }
    private Dictionary<string, List<GameObject>> pool = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public GameObject GetBullet(BulletData bulletData, Transform spawnPosition)
    {
        if (bulletData == null) return null;

        string key = bulletData.name;
        if (!pool.TryGetValue(key, out var bulletList))
        {
            bulletList = new List<GameObject>();
            pool[key] = bulletList;
        }
        foreach (var bullet in bulletList)
        {
            if (!bullet.activeInHierarchy)
            {
                bullet.transform.position = spawnPosition.position;
                bullet.SetActive(true);
                return bullet;
            }
        }
        GameObject newBullet = Instantiate(bulletData.prefabBullet, transform);
        newBullet.transform.position = spawnPosition.position;
        newBullet.name = key;
        bulletList.Add(newBullet);
        return newBullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        if (bullet == null) return;

        bullet.SetActive(false);
        string key = bullet.name;

        if (!pool.TryGetValue(key, out var bulletList))
        {
            bulletList = new List<GameObject>();
            pool[key] = bulletList;
        }
        bullet.transform.SetParent(transform);
        if (!bulletList.Contains(bullet))
        {
            bulletList.Add(bullet);
        }
    }

    public void ClearPool()
    {
        foreach (var bulletList in pool.Values)
        {
            foreach (var bullet in bulletList)
            {
                if (bullet != null)
                {
                    Destroy(bullet);
                }
            }
            bulletList.Clear();
        }
        pool.Clear();
    }

    private void OnDestroy() => ClearPool();
}