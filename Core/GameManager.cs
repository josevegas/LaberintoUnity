using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Sistemas")]
    public MazeGenerator mazeGenerator;
    public MazeRenderer mazeRenderer;
    
    [Header("Configuración Niveles")]
    public int itemsPerLevel = 5;
    public GameObject[] itemPrefabs; // 0=Coin, 1=Key, 2=PowerUp
    
    private int currentLevel;
    private int levelCoins;
    private int levelKeys;
    private bool levelCompleted = false;
    
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        currentLevel = SaveSystem.Instance.GetData().currentLevel;
        StartLevel(currentLevel);
    }
    
    public void StartLevel(int level)
    {
        currentLevel = level;
        levelCoins = 0;
        levelKeys = 0;
        levelCompleted = false;
        
        // Generar laberinto
        var grid = mazeGenerator.Generate(level);
        mazeRenderer.Render(grid);
        
        // Spawn items
        SpawnItems(mazeGenerator.GetEmptyPositions());
        
        UIManager.Instance.UpdateHUD(currentLevel, levelCoins, levelKeys);
    }
    
    void SpawnItems(List<Vector2Int> positions)
    {
        // Mezclar posiciones
        for (int i = 0; i < positions.Count; i++)
        {
            int j = Random.Range(i, positions.Count);
            var temp = positions[i];
            positions[i] = positions[j];
            positions[j] = temp;
        }
        
        // Spawn items
        for (int i = 0; i < Mathf.Min(itemsPerLevel, positions.Count); i++)
        {
            Vector3 pos = new Vector3(positions[i].x + 0.5f, positions[i].y + 0.5f, 0);
            
            // 70% moneda, 20% llave, 10% powerup
            float rand = Random.value;
            int prefabIndex = rand < 0.7f ? 0 : (rand < 0.9f ? 1 : 2);
            
            Instantiate(itemPrefabs[prefabIndex], pos, Quaternion.identity);
        }
    }
    
    public void AddCoins(int amount)
    {
        levelCoins += amount;
        UIManager.Instance.UpdateHUD(currentLevel, levelCoins, levelKeys);
    }
    
    public void AddKey()
    {
        levelKeys++;
        UIManager.Instance.UpdateHUD(currentLevel, levelCoins, levelKeys);
    }
    
    public void CompleteLevel()
    {
        if (levelCompleted) return;
        levelCompleted = true;
        
        // Guardar progreso
        SaveSystem.Instance.UpdateData(data => {
            data.currentLevel = currentLevel + 1;
            data.totalCoins += levelCoins;
            data.totalKeys += levelKeys;
            if (currentLevel < data.unlockedLevels.Length)
                data.unlockedLevels[currentLevel] = true;
        });
        
        StartCoroutine(LevelCompleteSequence());
    }
    
    IEnumerator LevelCompleteSequence()
    {
        UIManager.Instance.ShowLevelComplete();
        yield return new WaitForSeconds(2f);
        
        if (currentLevel < 10) // Máximo 10 niveles
            StartLevel(currentLevel + 1);
        else
            UIManager.Instance.ShowGameComplete();
    }
    
    public void ActivatePowerUp(PowerUp.Type type)
    {
        StartCoroutine(PowerUpEffect(type));
    }
    
    IEnumerator PowerUpEffect(PowerUp.Type type)
    {
        switch (type)
        {
            case PowerUp.Type.Speed:
                Time.timeScale = 1.5f;
                yield return new WaitForSeconds(5f);
                Time.timeScale = 1f;
                break;
            case PowerUp.Type.Vision:
                // Expandir campo de visión
                Camera.main.orthographicSize = 10f;
                yield return new WaitForSeconds(5f);
                Camera.main.orthographicSize = 6f;
                break;
        }
    }
}