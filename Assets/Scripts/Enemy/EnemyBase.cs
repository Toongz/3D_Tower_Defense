using UnityEngine;
using System;

public interface IEnemy
{
    void Initialize(Transform[] path, int multiplier);
    void TakeDamage(int amount);
    event Action OnReachedBase;
}

public abstract class EnemyBase : MonoBehaviour, IEnemy
{
    public event Action OnReachedBase;
    [SerializeField] protected int baseHealth; 
    [SerializeField] protected float speed;
    [SerializeField] protected int damage;
    [SerializeField] protected int reward;
    [SerializeField] protected int health;
    [SerializeField] protected int resour;
    protected Transform[] waypoints;
    protected int currentWaypointIndex;
    protected bool isActive;
    private Transform cachedTransform;
    protected int difficultyMultiplier = 1;
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }
    private void Awake()
    {
        cachedTransform = transform;
    }


    /// <summary>
    /// Initializes the enemy with path and stat multiplier
    /// </summary>
    public virtual void Initialize(Transform[] path, int multiplier)
    {
        if (path == null || path.Length == 0) return;

        cachedTransform = transform;
        difficultyMultiplier = multiplier;
        waypoints = path;
        //damage *= multiplier;
        //reward *= multiplier;
        health *= multiplier;
        ResetHealth(); // Gọi phương thức reset máu
        isActive = true;
        currentWaypointIndex = 0;
    }

    protected virtual void Update()
    {
        if (!isActive || currentWaypointIndex >= waypoints.Length) return;

        cachedTransform.position = Vector3.MoveTowards(
            cachedTransform.position,
            waypoints[currentWaypointIndex].position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(cachedTransform.position, waypoints[currentWaypointIndex].position) < 0.1f)
        {
            if (++currentWaypointIndex >= waypoints.Length) ReachBase();
        }
    }

    /// <summary>
    /// Applies damage to the enemy and triggers death if health reaches zero
    /// </summary>
    public virtual void TakeDamage(int amount)
    {
        if ((health -= amount) <= 0)
            Die();
    }

    protected virtual void Die()
    {
        if (!isActive) return;
        isActive = false;

        GameManager.Instance.AddScore(reward);
        GameManager.Instance.AddResource(resour);
        WaveManager.Instance.EnemyKilled();
        EnemyPool.Instance.ReturnEnemy<EnemyBase>(gameObject);
    }

    protected virtual void ReachBase()
    {
        if (!isActive) return;
        isActive = false;
        GameManager.Instance.ReduceHealth(damage);
        WaveManager.Instance.EnemyKilled();
        EnemyPool.Instance.ReturnEnemy<EnemyBase>(gameObject);

        OnReachedBase?.Invoke();
    }
    private void OnEnable()
    {
        ResetHealth();
    }

    private void ResetHealth()
    {
        health = baseHealth * difficultyMultiplier;
    }
}
