using UnityEngine;
using System.Collections.Generic;

public enum CellType { Wall, Path, Start, Exit }

public class MazeGenerator : MonoBehaviour
{
    [Header("Configuración")]
    public int width = 15;  // Impar para paredes
    public int height = 15;
    public int seed = 0;    // 0 = aleatorio
    
    [Header("Dificultad Progresiva")]
    public int minSize = 11;
    public int maxSize = 31;
    
    private CellType[,] grid;
    private System.Random rng;
    
    public CellType[,] Generate(int level)
    {
        // Tamaño aumenta con el nivel
        int size = Mathf.Min(minSize + (level * 2), maxSize);
        width = size % 2 == 0 ? size + 1 : size;
        height = width;
        
        // Seed para reproducibilidad
        rng = seed == 0 ? new System.Random() : new System.Random(seed + level);
        
        grid = new CellType[width, height];
        
        // Inicializar todo como paredes
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = CellType.Wall;
        
        // Generar desde (1,1)
        Carve(1, 1);
        
        // Marcar inicio y salida
        grid[1, 1] = CellType.Start;
        grid[width-2, height-2] = CellType.Exit;
        
        return grid;
    }
    
    void Carve(int x, int y)
    {
        grid[x, y] = CellType.Path;
        
        // Direcciones aleatorias
        var directions = new List<Vector2Int> 
        {
            new Vector2Int(0, 2), new Vector2Int(2, 0),
            new Vector2Int(0, -2), new Vector2Int(-2, 0)
        };
        
        // Fisher-Yates shuffle
        for (int i = directions.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            var temp = directions[i];
            directions[i] = directions[j];
            directions[j] = temp;
        }
        
        foreach (var dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;
            
            if (IsValid(nx, ny) && grid[nx, ny] == CellType.Wall)
            {
                grid[x + dir.x/2, y + dir.y/2] = CellType.Path;
                Carve(nx, ny);
            }
        }
    }
    
    bool IsValid(int x, int y) => x > 0 && x < width-1 && y > 0 && y < height-1;
    
    // Obtener posiciones válidas para items
    public List<Vector2Int> GetEmptyPositions()
    {
        var positions = new List<Vector2Int>();
        for (int x = 1; x < width-1; x++)
            for (int y = 1; y < height-1; y++)
                if (grid[x, y] == CellType.Path && !(x==1 && y==1))
                    positions.Add(new Vector2Int(x, y));
        return positions;
    }
}