using UnityEngine;
using System.Collections;

public class PoisonSpell : BaseSpell
{
    [SerializeField] private float duration;
    [SerializeField] private int damagePerSecond;
    [SerializeField] private ParticleSystem poisonEffect;
    public override void Cast(Vector3 position)
    {
        if (poisonEffect != null)
        {
            ParticleSystem effect = Instantiate(poisonEffect, position, Quaternion.identity);
            Destroy(effect.gameObject, duration);
        }
        StartCoroutine(PoisonEffect(position));
    }

    private IEnumerator PoisonEffect(Vector3 position)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            Collider[] hits = Physics.OverlapSphere(position, radius);
            foreach (var hit in hits)
            {
                Debug.Log("Poison");
                if (hit.TryGetComponent<EnemyBase>(out var enemy))
                {
                    enemy.TakeDamage(damagePerSecond);
                }
            }
            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }
        Destroy(gameObject);
    }
}