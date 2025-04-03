using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static UIManager Instance { get; private set; }
    [SerializeField] private Text scoreText;
    [SerializeField] private Text scoreGameOverText;
    [SerializeField] private Text goldText;
    [SerializeField] private Text healthText;
    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private Text tooltipText;
    [SerializeField] private GameObject tooltip;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private RectTransform tooltipRectTransform;
    [SerializeField] private Canvas canvas; 
    [SerializeField] private float tooltipPadding = 10f;
    [SerializeField] private Button timeScaleButton; 
    [SerializeField] private Text timeScaleButtonText;
    [SerializeField] private Text alertGold;
    private bool isPause = false;
    private bool isHoveringTooltip = false;
    private int currentTimeScaleIndex = 0;
    private readonly float[] timeScales = { 1f, 2f, 4f }; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        tooltip.SetActive(false);

        if (tooltipRectTransform == null)
        {
            tooltipRectTransform = tooltip.GetComponent<RectTransform>();
        }

        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                canvas = FindObjectOfType<Canvas>();
            }
        }

        if (timeScaleButton != null)
        {
            timeScaleButton.onClick.AddListener(ToggleTimeScale);
            UpdateTimeScaleButtonText();
        }
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (isPause)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

    }
    public void PauseGame()
    {
        isPause = true;
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPause = false;
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    private void ToggleTimeScale()
    {
        currentTimeScaleIndex = (currentTimeScaleIndex + 1) % timeScales.Length;
        Time.timeScale = timeScales[currentTimeScaleIndex];
        UpdateTimeScaleButtonText();
    }

    private void UpdateTimeScaleButtonText()
    {
        if (timeScaleButtonText != null)
        {
            timeScaleButtonText.text = $"x{timeScales[currentTimeScaleIndex]}";
        }
    }
    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnScoreChanged += UpdateScore;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnScoreChanged -= UpdateScore;
    }
    public void ShowGoldAlert(string message)
    {
        if (alertGold != null)
        {
            alertGold.text = message;
            alertGold.gameObject.SetActive(true);

            Invoke("HideGoldAlert", 2f);
        }
    }

    private void HideGoldAlert()
    {
        if (alertGold != null)
        {
            alertGold.gameObject.SetActive(false);
        }
    }
    public void ShowTooltip(string content, Vector3 position)
    {
        tooltipText.text = content;
        UpdateTooltipPosition(position);
        tooltip.SetActive(true);
    }

    public void UpdateTooltipPosition(Vector3 position)
    {
        if (tooltip == null || !tooltip.activeSelf || tooltipRectTransform == null)
            return;

        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                position,
                canvas.worldCamera,
                out Vector2 localPoint);

            tooltipRectTransform.localPosition = localPoint;
        }
        else
        {
            Vector2 tooltipSize = tooltipRectTransform.sizeDelta;

            float rightEdgeX = position.x + tooltipSize.x;
            float bottomEdgeY = position.y - tooltipSize.y;

            if (rightEdgeX > Screen.width - tooltipPadding)
            {
                position.x = Screen.width - tooltipSize.x - tooltipPadding;
            }

            if (bottomEdgeY < tooltipPadding)
            {
                position.y = tooltipSize.y + tooltipPadding;
            }

            tooltipRectTransform.position = position;
        }
    }

    public void HideTooltip()
    {
        if (!isHoveringTooltip)
        {
            tooltip.SetActive(false);
        }
    }

    public void ForceHideTooltip()
    {
        tooltip.SetActive(false);
    }

    public bool IsTooltipVisible()
    {
        return tooltip != null && tooltip.activeSelf;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHoveringTooltip = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHoveringTooltip = false;
        HideTooltip();
    }
    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }
    public void UpdateGold(int gold)
    {
        if (goldText != null)
            goldText.text = $"Gold: {gold}";
    }
    public void UpdateHealth(int health)
    {
        if (healthText != null)
            healthText.text = $"Health: {health}";
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main");
    }
    public void ShowGameOver() 
    {
        Time.timeScale = 0f;
        scoreGameOverText.text = "Score : " + GameManager.Instance.Score;
        panelGameOver?.SetActive(true);

    }
}
