using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    private int baseMultiplier = 1;
    private int increment = 1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public int GetMultiplier(int wave)
    {
        return baseMultiplier + Mathf.CeilToInt(Mathf.Sqrt(wave) * increment);
    }
   
}
