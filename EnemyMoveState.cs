public class EnemyMoveState : IEnemyState
{
    public void Execute(Enemy enemy)
    {
        enemy.MoveAlongPath();
    }
}
