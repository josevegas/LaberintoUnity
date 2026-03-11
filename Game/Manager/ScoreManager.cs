using UnityEngine;

/// <summary>
/// Gestiona la puntuación actual y el high score con persistencia.
/// Usa PlayerPrefs para simplicidad, pero estructurado para migrar fácilmente a JSON.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Configuración")]
    [SerializeField] private string highScoreKey = "MazeRunner_HighScore_v1";
    [SerializeField] private int maxHighScores = 5; // Para futura expansión a top 5

    public int CurrentScore { get; private set; } = 0;
    public int HighScore { get; private set; } = 0;

    // Eventos para UI
    public System.Action<int> OnScoreChanged;
    public System.Action<int> OnHighScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        LoadHighScore();
    }

    private void Start()
    {
        OnScoreChanged?.Invoke(CurrentScore);
        OnHighScoreChanged?.Invoke(HighScore);
    }

    public void AddScore(int points)
    {
        CurrentScore += points;
        OnScoreChanged?.Invoke(CurrentScore);
        
        // Verificar si superó high score durante la partida
        if (CurrentScore > HighScore)
        {
            // Opcional: actualizar high score en tiempo real
        }
    }

    public void AddLevelBonus(int bonus)
    {
        CurrentScore += bonus;
        OnScoreChanged?.Invoke(CurrentScore);
        Debug.Log($"Bonus de nivel: +{bonus}");
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        OnScoreChanged?.Invoke(CurrentScore);
    }

    /// <summary>
    /// Guarda el high score si la puntuación actual es mayor
    /// </summary>
    public void SaveHighScore()
    {
        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            PlayerPrefs.SetInt(highScoreKey, HighScore);
            PlayerPrefs.Save();
            
            OnHighScoreChanged?.Invoke(HighScore);
            Debug.Log($"¡Nuevo High Score: {HighScore}!");
        }
    }

    private void LoadHighScore()
    {
        HighScore = PlayerPrefs.GetInt(highScoreKey, 0);
    }

    /// <summary>
    /// Para debugging: resetear high score
    /// </summary>
    [ContextMenu("Reset High Score")]
    private void ResetHighScore()
    {
        PlayerPrefs.DeleteKey(highScoreKey);
        HighScore = 0;
        OnHighScoreChanged?.Invoke(HighScore);
    }
}