using System.Collections.Generic;
using UnityEngine;

public class ExplodingEnemy : EnemyBase
{
    public float explosionRadius = 3f;
    public int explosionDamage = 5;
    protected override void Die()
    {
        if (!isActive) return;
        isActive = false;

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        HashSet<Tower> damagedTowers = new HashSet<Tower>();    
        foreach (var hit in colliders)
        {
            if (hit.TryGetComponent(out Tower tower) && !damagedTowers.Contains(tower))
            {
                damagedTowers.Add(tower);
                tower.TakeDamage(explosionDamage);
            }
        }
        WaveManager.Instance.EnemyKilled();


        gameObject.SetActive(false);
        EnemyPool.Instance.ReturnEnemy<ExplodingEnemy>(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.1f);

        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}