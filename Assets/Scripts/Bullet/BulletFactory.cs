
using UnityEngine;
using System;

public class BulletFactory : MonoBehaviour
{
    public static BulletFactory Instance { get; private set; }
    public event Action<BulletData> OnBulletDataCreated;

    [SerializeField] private BulletData[] bulletLevels = new BulletData[0];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public BulletData GetBulletData(int level)
    {
        if (bulletLevels == null || bulletLevels.Length == 0)
        {
            Debug.LogError("BulletFactory: No bullet levels defined!");
            return null;
        }

        level = Mathf.Clamp(level, 0, bulletLevels.Length - 1);
        BulletData data = bulletLevels[level];
        OnBulletDataCreated?.Invoke(data);
        return data;
    }
}
