
using System;
using UnityEngine;



public class BuildableArea : MonoBehaviour
{

    [SerializeField] private TowerData towerData;
    private GameObject currentTower;

    /// <summary>
    /// Attempts to build a tower at this location
    /// </summary>
    public bool BuildTower()
    {
        if (!CanBuildTower())
        {
            Debug.Log("Cannot build: Tower exists or invalid tower data!");
            return false;
        }

        TowerLevelData firstLevel = towerData.Levels[0];
        if (!GameManager.Instance.SpendGold(firstLevel.upgradeCost))
        {
            Debug.Log("Not enough gold to build!");
            return false;
        }

        currentTower = Instantiate(firstLevel.towerPrefab, transform.position, Quaternion.identity);
        currentTower.transform.SetParent(transform);

        if (currentTower.TryGetComponent<Tower>(out var towerComponent))
        {
            towerComponent.Initialize(towerData, 0);
            return true;
        }

        Destroy(currentTower);
        Debug.LogError("Tower prefab missing Tower component!");
        return false;
    }


    public bool CanBuildTower()
    {
        if (towerData == null || towerData.Levels.Length == 0)
            return false;

        // Kiểm tra có Tower nào trong phạm vi không (kể cả không phải currentTower)
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);
        foreach (var collider in colliders)
        {
            if (collider.GetComponent<Tower>() != null)
                return false;
        }

        return true;
    }
    public bool HasTower() => currentTower != null;

}

