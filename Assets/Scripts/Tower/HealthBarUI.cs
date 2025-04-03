using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Tower tower;
    [SerializeField] private Image imageBar;

    private void Start()
    {
        GameEvents.OnTowerHealthChanged += UpdateHealthBar;
    }

    private void OnDestroy()
    {
        GameEvents.OnTowerHealthChanged -= UpdateHealthBar; 
    }

    private void UpdateHealthBar(Tower affectedTower, float currentHealth)
    {
        if (tower == affectedTower) 
        {
            imageBar.fillAmount = currentHealth;
        }
    }
}
