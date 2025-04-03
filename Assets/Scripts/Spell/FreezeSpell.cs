using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FreezeSpell : BaseSpell
{
    [SerializeField] private float duration = 3f;
    [SerializeField] private float slowFactor = 0.5f;
    [SerializeField] private ParticleSystem freezeEffect;

    public override void Cast(Vector3 position)
    {
        if (freezeEffect != null)
        {
            ParticleSystem effect = Instantiate(freezeEffect, position, Quaternion.identity);
            Destroy(effect.gameObject, 1f);
        }

        // Tách coroutine ra và chạy độc lập
        //StartCoroutine(FreezeEffectCoroutine(position));

        // Hủy game object ngay lập tức
        //Destroy(gameObject);

        // Thay vì StartCoroutine trực tiếp
        GameManager.Instance.StartCoroutine(FreezeEffectCoroutine(position));

        Destroy(gameObject);
    }

    private IEnumerator FreezeEffectCoroutine(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, radius);
        Dictionary<EnemyBase, float> originalSpeeds = new Dictionary<EnemyBase, float>(); 

        // Làm chậm enemy
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<EnemyBase>(out var enemy))
            {
                originalSpeeds[enemy] = enemy.Speed; 
                enemy.Speed *= slowFactor;
            }
        }

        // Chờ đủ duration
        yield return new WaitForSeconds(duration);

        // Khôi phục tốc độ
        foreach (var entry in originalSpeeds)
        {
            if (entry.Key != null) 
            {
                entry.Key.Speed = entry.Value;
            }
        }
    }
}