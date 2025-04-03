
using UnityEngine;

public interface ISpell
{
    void Cast(Vector3 position);
    int Cost {  get; }
    float Radius { get; }
}
