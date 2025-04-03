using UnityEngine;

public class FastEnemy : EnemyBase
{
    public override void Initialize(Transform[] path, int multiplier)
    {
        base.Initialize(path, multiplier);
        speed = 5f; 
        health = 3 * multiplier; 
    }
}
