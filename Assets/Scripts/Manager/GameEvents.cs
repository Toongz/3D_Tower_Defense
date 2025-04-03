
using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static Action<int> OnLevelChanged;
    public static Action<int> OnWaveStarted;
    public static Action OnWaveCompleted;
    public static Action<Tower> OnTowerSelected;
    public static Action<Tower> OnTowerUpgraded;
    public static event Action<Tower, float> OnTowerHealthChanged;
    public static Action<Enum.SpellType> OnSpellCasted;

    public static void TowerHealthChanged(Tower tower, float currentHealth)
    {
        OnTowerHealthChanged?.Invoke(tower, currentHealth);
    }

}