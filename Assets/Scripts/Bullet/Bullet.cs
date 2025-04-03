using UnityEngine;

public interface IBullet
{
    void Initialize(EnemyBase target, BulletData bulletData);
    void ReturnToPool();
}

public class Bullet : MonoBehaviour, IBullet
{
    private EnemyBase target;
    private BulletData data;
    private int damage;
    private Vector3 direction;
    private bool isActive;

    public void Initialize(EnemyBase targetEnemy, BulletData bulletData)
    {
        if (bulletData == null) return;

        target = targetEnemy;
        data = bulletData;
        damage = bulletData.damage;
        transform.rotation = Quaternion.identity;
        isActive = true;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!isActive || target == null || data == null)
        {
            ReturnToPool();
            return;
        }

        direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * data.speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);

        if ((transform.position - target.transform.position).sqrMagnitude < 0.25f)
        {
            OnHitTarget();
        }
    }

    private void OnHitTarget()
    {
        target?.TakeDamage(damage);
        ReturnToPool();
    }
    public void ReturnToPool()
    {
        if (!isActive) return;
        isActive = false;
        gameObject.SetActive(false);
        BulletPool.Instance.ReturnBullet(gameObject);
    }
}