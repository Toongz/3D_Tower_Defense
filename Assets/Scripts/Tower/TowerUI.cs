
using UnityEngine;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour
{
    [SerializeField] private Text upgradeCostText;

    /// <summary>
    /// Updates the UI with tower upgrade information
    /// </summary>
    public void UpdateUI(Tower tower)
    {
        if (tower == null || upgradeCostText == null)
        {
            Debug.LogWarning("TowerUI: Invalid tower or UI component!");
            return;
        }

        TowerLevelData nextLevelData = tower.GetNextLevelData();
        upgradeCostText.text = nextLevelData != null
            ? $"{nextLevelData.upgradeCost} Gold"
            : "Max Level";
    }
}
