using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Game/BulletData")]

public  class BulletData : ScriptableObject
{
    public GameObject prefabBullet;
    public float speed;
    public int damage;
}