using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;

    [Header("Game")]
    public int startingLives = 3;

    private int score = 0;
    private int lives;

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
        lives = startingLives;
        RefreshUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        RefreshUI();
    }

    public void LoseLife(int amount = 1)
    {
        lives -= amount;
        lives = Mathf.Max(0, lives);
        RefreshUI();

        if (lives <= 0)
        {
            Debug.Log("GAME OVER");
            // later: show Game Over UI + stop spawner
        }
    }

    void RefreshUI()
    {
        if (scoreText) scoreText.text = $"Score: {score}";
        if (livesText) livesText.text = $"Lives: {lives}";
    }
}