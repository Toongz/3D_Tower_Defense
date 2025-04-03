using UnityEngine;
using UnityEngine.EventSystems;

public class Tower : MonoBehaviour
{
    [SerializeField] private TowerData towerData;

    private int currentLevelIndex;
    [SerializeField] private int health;
    private float fireCooldown;
    private Transform cachedTransform;
    private bool isActive;
    private float lastDamageTime = 0f;
    public int Health { get { return health; } }
    private void Awake()
    {
        cachedTransform = transform;
        isActive = true;
    }
    public void Initialize(TowerData data, int levelIndex)
    {
        towerData = data;
        currentLevelIndex = levelIndex;
        health = GetCurrentLevelData().health;
        isActive = true;
    }

    private void Update()
    {
        Interact();
        if (!isActive || (fireCooldown -= Time.deltaTime) > 0) return;

        if (FindClosestEnemy() is EnemyBase target)
        {
            Shoot(target);
            fireCooldown = 1f / GetCurrentLevelData().fireRate;
        }
    }

    private EnemyBase FindClosestEnemy()
    {
        float range = GetCurrentLevelData().attackRange;
        Collider[] hits = Physics.OverlapSphere(cachedTransform.position, range);
        EnemyBase closest = null;
        float closestSqrDist = range * range;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<EnemyBase>(out var enemy))
            {
                float sqrDist = (cachedTransform.position - enemy.transform.position).sqrMagnitude;
                if (sqrDist < closestSqrDist)
                {
                    closest = enemy;
                    closestSqrDist = sqrDist;
                }
            }
        }
        return closest;
    }

    private void Shoot(EnemyBase target)
    {
        BulletData bulletData = BulletFactory.Instance.GetBulletData(currentLevelIndex);
        GameObject bullet = BulletPool.Instance.GetBullet(bulletData, cachedTransform);
        bullet.GetComponent<Bullet>().Initialize(target, bulletData);
    }
    public bool Upgrade()
    {
        if (!isActive || currentLevelIndex + 1 >= towerData.Levels.Length) return false;

        TowerLevelData nextLevel = towerData.Levels[currentLevelIndex + 1];
        if (!GameManager.Instance.SpendGold(nextLevel.upgradeCost)) return false;

        currentLevelIndex++;
        ApplyUpgrade(nextLevel);
        GameEvents.OnTowerUpgraded?.Invoke(this);
        return true;
    }

    private void Interact()
    {
        if (!Input.GetMouseButtonDown(0) || EventSystem.current.IsPointerOverGameObject()) return;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent<Tower>(out var tower))
            {
                GameEvents.OnTowerSelected.Invoke(tower);
                TowerUIManager.Instance.ShowUI(tower);
            }
            else
            {
                GameEvents.OnTowerSelected?.Invoke(null); // Khi không phải Tower
            }

        }
    }
    private void ApplyUpgrade(TowerLevelData levelData)
    {
        if (levelData.towerPrefab != null)
        {
            GameObject newTower = Instantiate(levelData.towerPrefab, cachedTransform.position, Quaternion.identity);
            // Đặt parent nếu tháp cũ có parent (BuildableArea)
            if (transform.parent != null)
            {
                newTower.transform.SetParent(transform.parent);
            }
            Tower newTowerComponent = newTower.GetComponent<Tower>();
            newTowerComponent.Initialize(towerData, currentLevelIndex);
            isActive = false;
            Destroy(gameObject);
        }
    }

    public TowerLevelData GetCurrentLevelData() => towerData.Levels[currentLevelIndex];

    public TowerLevelData GetNextLevelData() =>
        currentLevelIndex + 1 < towerData.Levels.Length ? towerData.Levels[currentLevelIndex + 1] : null;

    public void TakeDamage(int damage)
    {
        if (Time.time - lastDamageTime < 0.1f) return;
        lastDamageTime = Time.time;

        if (!isActive) return;

        health -= damage;
        float healthPercentage = (float)health / GetCurrentLevelData().health;

        

        if (health <= 0)
        {
            isActive = false;
            Destroy(gameObject);
        }
        GameEvents.TowerHealthChanged(this, healthPercentage);
    }
    public void RepairToFull()
    {
        if (!isActive) return;
        health = GetCurrentLevelData().health;
        float healthPercentage = 1f; // 100%
        GameEvents.TowerHealthChanged(this, healthPercentage);
    }

}
