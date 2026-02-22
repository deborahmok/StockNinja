using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TextMeshProUGUI bankrollText;     // shows current $ amount
    public TextMeshProUGUI highBankrollText; // shows highest $ ever
    public TextMeshProUGUI timerText;        // shows remaining time
    public TextMeshProUGUI gameOverText;     // optional: "BANKRUPT" text

    [Header("Optional End Screen (Partner)")]
    public GameObject endGamePanel;          // partner can use
    public TextMeshProUGUI earningsText;     // partner can use

    [Header("Systems")]
    public GameObject spawner;               // drag your Spawner object here

    [Header("Bankroll")]
    public float startingBankroll = 100f;
    public float bankruptThreshold = 10f;

    [Header("Timer")]
    public float roundLengthSeconds = 45f;

    private float bankroll;
    private float timeLeft;
    private bool ended = false;

    private const string HIGH_BANKROLL_KEY = "HighBankroll";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        bankroll = startingBankroll;
        timeLeft = roundLengthSeconds;
        ended = false;

        if (gameOverText)
        {
            gameOverText.gameObject.SetActive(false);
            gameOverText.text = "BANKRUPT";
        }
        if (endGamePanel) endGamePanel.SetActive(false);
        AudioManager.Instance?.ResetLastTen();
        RefreshUI();
        RefreshHighScoreUI();
    }

    void Update()
    {
        if (ended) return;

        timeLeft -= Time.deltaTime;
        timeLeft = Mathf.Max(0f, timeLeft);

        if (timerText) timerText.text = $"Time: {timeLeft:0}";
        if (timeLeft <= 11f)
        {
            AudioManager.Instance?.PlayLastTenOnce();
        }
        if (timeLeft <= 0f)
        {
            EndRound(); // stop + show results (partner hooks here)
        }
    }

    // -------------------------
    // Money logic
    // -------------------------

    public void ApplyPercent(float percent)
    {
        if (ended) return;

        bankroll *= (1f + percent / 100f);

        if (bankroll < bankruptThreshold)
        {
            GameOver(); // sets bankroll=0 and ends
            return;
        }

        RefreshUI();
    }

    public void AddFlatCash(float amount)
    {
        if (ended) return;

        bankroll += amount;
        RefreshUI();
    }

    public float GetBankroll() => bankroll;

    // -------------------------
    // End conditions
    // -------------------------

    public void GameOver(bool forceZero = false)
    {
        if (ended) return;
        ended = true;

        if (forceZero)
            bankroll = 0f;

        RefreshUI();

        if (gameOverText)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "BANKRUPT";
        }

        StopGameSystems();

        SaveHighBankrollIfNeeded(bankroll);
        RefreshHighScoreUI();

        ShowEndPanel();
    }

    public void EndRound()
    {
        if (ended) return;
        ended = true;

        StopGameSystems();

        SaveHighBankrollIfNeeded(bankroll);
        RefreshHighScoreUI();

        // Show “market closed” message (not bankruptcy)
        if (gameOverText)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "$TOCK MARKET HA$ CLO$ED TODAY";
        }

        ShowEndPanel();
    }

    void StopGameSystems()
    {
        if (spawner) spawner.SetActive(false);
    }

    // -------------------------
    // High score
    // -------------------------

    void SaveHighBankrollIfNeeded(float finalBankroll)
    {
        int best = PlayerPrefs.GetInt(HIGH_BANKROLL_KEY, 0);
        int finalRounded = Mathf.RoundToInt(finalBankroll);

        if (finalRounded > best)
        {
            PlayerPrefs.SetInt(HIGH_BANKROLL_KEY, finalRounded);
            PlayerPrefs.Save();
        }
    }

    void RefreshHighScoreUI()
    {
        if (!highBankrollText) return;
        int best = PlayerPrefs.GetInt(HIGH_BANKROLL_KEY, 0);
        highBankrollText.text = $"Best: $ {best}";
    }

    // -------------------------
    // UI
    // -------------------------

    void RefreshUI()
    {
        if (bankrollText) bankrollText.text = $"Bankroll: $ {bankroll:0.00}";
        if (timerText) timerText.text = $"Time: {timeLeft:0}";
    }

    void ShowEndPanel()
    {
        if (endGamePanel) endGamePanel.SetActive(true);
        if (earningsText) earningsText.text = $"Today's Earnings: $ {bankroll:0.00}";
    }
    
    public void RestartGame()
    {
        // Safety reset in case time was modified
        Time.timeScale = 1f;

        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}