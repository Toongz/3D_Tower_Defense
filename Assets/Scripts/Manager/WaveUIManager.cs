using UnityEngine;
using UnityEngine.UI;

public class WaveUIManager : MonoBehaviour
{
    public static WaveUIManager Instance { get; private set; }
    [SerializeField] private Text waveText;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable() => GameEvents.OnWaveStarted += UpdateWave;
    private void OnDisable() => GameEvents.OnWaveStarted -= UpdateWave;
    public void UpdateWave(int wave)
    {
        if (waveText != null)
        {
            waveText.text = $"Wave: {wave}";
        }
    }
}
