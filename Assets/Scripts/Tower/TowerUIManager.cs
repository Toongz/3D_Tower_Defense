
using UnityEngine;
using UnityEngine.UI;

public class TowerUIManager : MonoBehaviour
{
    public static TowerUIManager Instance { get; private set; }
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private Text towerInfoText;
    [SerializeField] private GameObject towerInteract;
    [SerializeField] private Button showInforButton;
    [SerializeField] private Button repairButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Text upgradeCostText;

    private Tower selectedTower;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        uiPanel.SetActive(false);
    }


    /// <summary>
    /// Shows the UI for the selected tower
    /// </summary>
    public void ShowUI(Tower tower)
    {
        selectedTower = tower;
        towerInteract.SetActive(true);
        showInforButton.onClick.RemoveAllListeners();
        showInforButton.onClick.AddListener(() => ShowInfor(tower));

        repairButton.onClick.RemoveAllListeners();
        repairButton.onClick.AddListener(RepairTower);

        UpdateRepairButton(); // Cập nhật trạng thái ngay khi chọn tower

    }

    public void ShowInfor(Tower tower)
    {
        // Đảm bảo tower được cập nhật là tower hiện tại
        selectedTower = tower;

        // Lấy dữ liệu mức hiện tại và mức tiếp theo
        TowerLevelData current = tower.GetCurrentLevelData();
        TowerLevelData next = tower.GetNextLevelData();
        BulletData bulletData = BulletFactory.Instance.GetBulletData(current.level - 1);

        // Cập nhật thông tin chi tiết của tower
        towerInfoText.text = $"Level: {current.level}" +
            $"\nRange: {current.attackRange}" +
              $"\nDamage: {bulletData?.damage ?? 0}" + // Hiển thị damage từ BulletData
            $"\nFire Rate: {current.fireRate}" +
            $"\nHealth: {tower.Health}/{current.health}";

        // Xử lý nâng cấp
        if (next != null)
        {
            upgradeCostText.text = $"{next.upgradeCost} Gold";
            upgradeButton.interactable = GameManager.Instance.PlayerGold >= next.upgradeCost;
            upgradeButton.gameObject.SetActive(true);
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(UpgradeTower);
        }
        else
        {
            upgradeCostText.text = "Max Level";
            upgradeButton.interactable = false;
            upgradeButton.gameObject.SetActive(true);
        }

        // Luôn cập nhật trạng thái nút repair
        UpdateRepairButton();

        // Hiển thị UI
        uiPanel.SetActive(true);
        towerInteract.SetActive(false);
    }

    /// <summary>
    /// Hides the tower UI
    /// </summary>
    public void HideUI()
    {
         uiPanel.SetActive(false);
        towerInteract.SetActive(true);
    }
    public void HideInteract() => towerInteract.SetActive(false);


    private void UpgradeTower()
    {
        if (selectedTower?.Upgrade() == true)
        {
            ShowUI(selectedTower);
            HideUI();
            HideInteract();

        }
    }
    /// <summary>
    /// Repairs the selected tower to full health
    /// </summary>
    private void RepairTower()
    {
        if (selectedTower == null) return;

        TowerLevelData currentLevel = selectedTower.GetCurrentLevelData();
        BulletData bulletData = BulletFactory.Instance.GetBulletData(currentLevel.level - 1);
        int repairCost = CalculateRepairCost(selectedTower.Health, currentLevel.health);

        if (GameManager.Instance.SpendGold(repairCost))
        {
            selectedTower.RepairToFull();
            UpdateRepairButton(); // Cập nhật sau khi sửa
            if (towerInteract.activeSelf)
            {
                towerInfoText.text = $"Level: {currentLevel.level}" +
                    $"\nRange: {currentLevel.attackRange}" +
                    $"\nFire Rate: {currentLevel.fireRate}" +
                    $"\nDamage: {bulletData?.damage ?? 0}" + // Hiển thị damage từ BulletData
                    $"\nHealth: {selectedTower.Health}/{currentLevel.health}";
            }
        }
    }

    /// <summary>
    /// Calculates the repair cost based on missing health
    /// </summary>
    private int CalculateRepairCost(int currentHealth, int maxHealth)
    {
        const float COST_PER_HP = 5f;
        int missingHealth = maxHealth - currentHealth;
        int cost = Mathf.CeilToInt(missingHealth * COST_PER_HP);
        return cost;
    }
    private void UpdateRepairButton()
    {
        // Kiểm tra nếu không có tower được chọn hoặc UI không hiển thị
        if (selectedTower == null || !towerInteract.activeSelf)
        {
            repairButton.interactable = false;
            return;
        }

        TowerLevelData current = selectedTower.GetCurrentLevelData();
        int repairCost = CalculateRepairCost(selectedTower.Health, current.health);

        // Cập nhật text của nút repair
        Text repairButtonText = repairButton.GetComponentInChildren<Text>();
        if (repairButtonText != null)
        {
            repairButtonText.text = $"Repair ({repairCost} Gold)";
        }

        // Kiểm tra điều kiện enable nút
        bool isFullHealth = selectedTower.Health >= current.health;
        bool hasEnoughGold = GameManager.Instance.PlayerGold >= repairCost;

        // Chỉ enable nút khi chưa đầy máu và đủ tiền
        repairButton.interactable = !isFullHealth && hasEnoughGold;
    }
    private void UpdateUpgradeButton(int gold)
    {
        if (selectedTower != null && towerInteract.activeSelf)
        {
            TowerLevelData next = selectedTower.GetNextLevelData();
            if (next != null)
                upgradeButton.interactable = gold >= next.upgradeCost;
        }
    }
    private void OnEnable()
    {
        GameManager.Instance.OnGoldChanged += UpdateUpgradeButton;
        GameEvents.OnTowerHealthChanged += OnTowerHealthChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGoldChanged -= UpdateUpgradeButton;
        GameEvents.OnTowerHealthChanged -= OnTowerHealthChanged;
    }
    private void OnTowerHealthChanged(Tower tower, float healthPercentage)
    {
        if (tower == selectedTower && towerInteract)
        {
            UpdateRepairButton();
            TowerLevelData current = selectedTower.GetCurrentLevelData();
            BulletData bulletData = BulletFactory.Instance.GetBulletData(current.level - 1);
            Debug.Log(bulletData.damage);
            towerInfoText.text = $"Level: {current.level }" +
                $"\nRange: {current.attackRange}" +
                $"\nFire Rate: {current.fireRate}" +
                $"\nDamage: {bulletData?.damage ?? 0}" + // Hiển thị damage từ BulletData
                $"\nHealth: {tower.Health}/{current.health}";
        }
        if (tower == selectedTower && healthPercentage <= 0f)
        {
            selectedTower = null; 
            towerInteract.SetActive(false);
            uiPanel.SetActive(false);
        }
    }
}
