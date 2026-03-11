using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gestiona todos los elementos de UI del juego.
/// Usa TextMeshPro para mejor renderizado de pixel art.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;

    [Header("Textos HUD")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private Image keyIcon;

    [Header("Textos Game Over")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI newHighScoreText;

    [Header("Configuración Visual")]
    [SerializeField] private Color keyActiveColor = Color.yellow;
    [SerializeField] private Color keyInactiveColor = Color.gray;

    private void Start()
    {
        // Suscribirse a eventos de ScoreManager
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateScore;
            ScoreManager.Instance.OnHighScoreChanged += UpdateHighScore;
        }
        
        // Inicializar UI
        gameOverPanel?.SetActive(false);
        pausePanel?.SetActive(false);
        hudPanel?.SetActive(true);
        
        UpdateKeyStatus(false);
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateScore;
            ScoreManager.Instance.OnHighScoreChanged -= UpdateHighScore;
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"SCORE: {score:D6}";
    }

    public void UpdateHighScore(int highScore)
    {
        if (highScoreText != null)
            highScoreText.text = $"HI: {highScore:D6}";
    }

    public void UpdateLevel(int level)
    {
        if (levelText != null)
            levelText.text = $"LEVEL: {level:D2}";
    }

    public void UpdateLives(int lives)
    {
        if (livesText != null)
            livesText.text = $"LIVES: {lives}";
    }

    public void UpdateKeyStatus(bool hasKey)
    {
        if (keyIcon != null)
        {
            keyIcon.color = hasKey ? keyActiveColor : keyInactiveColor;
            keyIcon.gameObject.SetActive(true); // Siempre visible, cambia color
        }
    }

    public void ShowGameOver(int finalScore)
    {
        hudPanel?.SetActive(false);
        gameOverPanel?.SetActive(true);
        
        if (finalScoreText != null)
            finalScoreText.text = $"FINAL SCORE: {finalScore}";
        
        // Verificar si es high score
        bool isNewHigh = finalScore >= ScoreManager.Instance.HighScore;
        if (newHighScoreText != null)
            newHighScoreText.gameObject.SetActive(isNewHigh);
    }

    public void HideGameOver()
    {
        gameOverPanel?.SetActive(false);
        hudPanel?.SetActive(true);
    }

    public void TogglePause(bool isPaused)
    {
        pausePanel?.SetActive(isPaused);
    }
}