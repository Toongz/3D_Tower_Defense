using UnityEngine;
using System;

public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance { get; private set; }
    public event Action<int> OnBulletLevelChanged;

    private int currentLevel;

    public int CurrentLevel => currentLevel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable() => GameEvents.OnLevelChanged += UpdateLevel;
    private void OnDisable() => GameEvents.OnLevelChanged -= UpdateLevel;
    private void UpdateLevel(int newLevel)
    {
        currentLevel = newLevel;
        OnBulletLevelChanged?.Invoke(newLevel);
    }
}