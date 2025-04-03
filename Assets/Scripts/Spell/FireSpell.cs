using UnityEngine;

public class FireSpell : BaseSpell
{
    [SerializeField] private int damage = 10;
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private float destroyDelay = 0.5f;

    
    public override void Cast(Vector3 position)
    {
        if (explosionEffect != null)
        {
            ParticleSystem effect = Instantiate(explosionEffect, position, Quaternion.identity);
            Destroy(effect.gameObject, destroyDelay);
        }

        Collider[] hitEnemies = Physics.OverlapSphere(position, radius);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.TryGetComponent<EnemyBase>(out var enemyBase))
            {
            Debug.Log("Take damage");
                enemyBase.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
        
    }

    private void OnValidate()
    {
        if (damage < 0) damage = 0;
        if (radius <= 0) radius = 2f;
        if (destroyDelay < 0) destroyDelay = 0.5f;
    }
}
