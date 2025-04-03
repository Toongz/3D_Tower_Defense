using UnityEngine;

[System.Serializable]
public class TowerLevelData
{
    public int level;
    public int health;
    public int upgradeCost;
    public float attackRange;
    public float fireRate;
    public GameObject towerPrefab;
}

[CreateAssetMenu(fileName = "TowerData", menuName = "Game/TowerData")]
public class TowerData : ScriptableObject
{
    [SerializeField] private TowerLevelData[] levels = new TowerLevelData[0];
    public TowerLevelData[] Levels => levels;

    private void OnValidate()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i].upgradeCost < 0) levels[i].upgradeCost = 0;
            if (levels[i].fireRate <= 0) levels[i].fireRate = 1f;
            if (levels[i].attackRange < 0) levels[i].attackRange = 0;
        }
    }
}