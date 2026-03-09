using UnityEngine;
using System.IO;

[System.Serializable]
public class GameData
{
    public int currentLevel = 1;
    public int totalCoins = 0;
    public int totalKeys = 0;
    public bool[] unlockedLevels;
    public float bestTimeLevel1;
    public float bestTimeLevel2;
    // etc...
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    
    private string savePath;
    private GameData currentData;
    
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        savePath = Path.Combine(Application.persistentDataPath, "mazegame.save");
        LoadGame();
    }
    
    public void SaveGame()
    {
        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Guardado en: " + savePath);
    }
    
    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentData = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            currentData = new GameData();
            currentData.unlockedLevels = new bool[10]; // 10 niveles
            currentData.unlockedLevels[0] = true; // Nivel 1 desbloqueado
        }
    }
    
    public GameData GetData() => currentData;
    public void UpdateData(System.Action<GameData> modify) 
    { 
        modify(currentData); 
        SaveGame(); 
    }
}
