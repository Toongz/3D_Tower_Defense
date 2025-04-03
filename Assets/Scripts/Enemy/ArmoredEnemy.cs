using UnityEngine;

public class ArmoredEnemy : EnemyBase
{
    private int armor = 3;

    public override void TakeDamage(int amount)
    {
        int reducedDamage = Mathf.Max(amount - armor, 1); 
        base.TakeDamage(reducedDamage);
    }
}
