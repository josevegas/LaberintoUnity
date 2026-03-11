using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager - Controlador principal del flujo del juego.
/// Gestiona estados, niveles, vidas y coordina entre sistemas.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private MazeGenerator mazeGenerator;
    [SerializeField] private PlayerController player;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private ScoreManager scoreManager;

    [Header("Configuración de Nivel")]
    [SerializeField] private int initialLives = 3;
    [SerializeField] private float levelStartDelay = 0.5f;

    // Estado del juego
    public int CurrentLevel { get; private set; } = 1;
    public int CurrentLives { get; private set; }
    public bool IsGameActive { get; private set; } = false;
    public bool HasKey { get; private set; } = false;

    // Eventos para otros sistemas
    public System.Action OnLevelStart;
    public System.Action OnLevelComplete;
    public System.Action OnGameOver;
    public System.Action OnPlayerDeath;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CurrentLives = initialLives;
        StartLevel(1);
    }

    /// <summary>
    /// Inicia un nivel específico con configuración de dificultad
    /// </summary>
    public void StartLevel(int level)
    {
        CurrentLevel = level;
        HasKey = false;
        IsGameActive = false;
        
        // Generar laberinto con dificultad progresiva
        mazeGenerator.GenerateLevel(level);
        
        // Posicionar jugador en el spawn
        Vector2 spawnPos = mazeGenerator.GetPlayerSpawnPosition();
        player.transform.position = spawnPos;
        player.ResetState();
        
        // Actualizar UI
        uiManager.UpdateLevel(level);
        uiManager.UpdateKeyStatus(false);
        uiManager.UpdateLives(CurrentLives);
        
        Invoke(nameof(ActivateGame), levelStartDelay);
        OnLevelStart?.Invoke();
    }

    private void ActivateGame()
    {
        IsGameActive = true;
    }

    /// <summary>
    /// Avanza al siguiente nivel
    /// </summary>
    public void CompleteLevel()
    {
        IsGameActive = false;
        scoreManager.AddLevelBonus(CurrentLevel * 100);
        OnLevelComplete?.Invoke();
        
        // Pequeña pausa antes del siguiente nivel
        Invoke(nameof(NextLevel), 1f);
    }

    private void NextLevel()
    {
        StartLevel(CurrentLevel + 1);
    }

    /// <summary>
    /// Maneja la muerte del jugador
    /// </summary>
    public void PlayerDied()
    {
        CurrentLives--;
        uiManager.UpdateLives(CurrentLives);
        OnPlayerDeath?.Invoke();
        
        if (CurrentLives <= 0)
        {
            GameOver();
        }
        else
        {
            // Respawn en el inicio del nivel actual
            Invoke(nameof(RespawnPlayer), 0.5f);
        }
    }

    private void RespawnPlayer()
    {
        // Perder la llave al morir (opcional: comenta si quieres mantenerla)
        HasKey = false;
        uiManager.UpdateKeyStatus(false);
        
        Vector2 spawnPos = mazeGenerator.GetPlayerSpawnPosition();
        player.Respawn(spawnPos);
        IsGameActive = true;
    }

    private void GameOver()
    {
        IsGameActive = false;
        OnGameOver?.Invoke();
        uiManager.ShowGameOver(scoreManager.CurrentScore);
        
        // Guardar high score
        scoreManager.SaveHighScore();
        
        // Opción de reinicio
        Invoke(nameof(RestartGame), 3f);
    }

    private void RestartGame()
    {
        scoreManager.ResetScore();
        CurrentLives = initialLives;
        StartLevel(1);
        uiManager.HideGameOver();
    }

    // Métodos públicos para interacción con ítems
    public void CollectCoin(int value)
    {
        scoreManager.AddScore(value);
    }

    public void CollectKey()
    {
        HasKey = true;
        uiManager.UpdateKeyStatus(true);
    }

    public bool UseKey()
    {
        if (HasKey)
        {
            HasKey = false;
            uiManager.UpdateKeyStatus(false);
            return true;
        }
        return false;
    }

    public void RegisterDoorOpened()
    {
        CompleteLevel();
    }
}