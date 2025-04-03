
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<int> OnGoldChanged;
    public event Action<int> OnScoreChanged;
    public event Action<int> OnHealthChanged;
    public event Action OnGameOver;

    [SerializeField] private int playerHealth;
    [SerializeField] private int playerGold;
    [SerializeField] private int score;
    [SerializeField] private float goldPerSecond; 
    [SerializeField] private float goldIncreaseInterval;
    [SerializeField] private Interstitial interstitialAd; 
    [SerializeField] private Banner bannerAd;

    public int PlayerHealth => playerHealth;
    public int PlayerGold => playerGold;
    public int Score => score;

    private float goldTimer; 
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        UIManager.Instance.UpdateHealth(playerHealth);
        goldTimer = goldIncreaseInterval;
        interstitialAd?.LoadAd();
        bannerAd?.LoadBanner();
    }


    private void Update()
    {
        goldTimer -= Time.deltaTime;
        if (goldTimer <= 0)
        {
            AddResource(Mathf.FloorToInt(goldPerSecond)); 
            goldTimer = goldIncreaseInterval; 
        }

    }
    public void ReduceHealth(int amount)
    {
        playerHealth -= amount;
        OnHealthChanged?.Invoke(playerHealth);
        UIManager.Instance.UpdateHealth(playerHealth);
        if (playerHealth <= 0)
        {
            Debug.Log("Game Over");
            OnGameOver?.Invoke();
            ShowGameOverAds(); 
            UIManager.Instance.ShowGameOver();
        }

    }
    private void ShowGameOverAds()
    {
        if (bannerAd != null)
        {
            bannerAd.ShowBannerAd(); 
        }
        if (interstitialAd != null)
        {
            interstitialAd.ShowAd(); 
        }
    }
    public bool SpendGold(int amount)
    {
        if (playerGold >= amount)
        {
            playerGold -= amount;
            OnGoldChanged?.Invoke(playerGold);
            return true;
        }
        else
        {
            UIManager.Instance.ShowGoldAlert("Not enough gold!");
            return false;
        }
    }

    public void AddScore(int points)
    {
        score += points;
        OnScoreChanged?.Invoke(score);
        UIManager.Instance.UpdateScore(score);
    }

    public void AddResource(int golds)
    {
        playerGold += golds;
        OnGoldChanged?.Invoke(playerGold);
        UIManager.Instance.UpdateGold(playerGold);
    }
}
