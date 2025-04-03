using UnityEngine;

public abstract class BaseSpell : MonoBehaviour, ISpell
{
    [SerializeField] protected float radius = 2f;
    [SerializeField] protected int cost;

    public float Radius => radius;
    public int Cost => cost;

    public abstract void Cast(Vector3 position);

}
