using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generador de laberintos usando DFS con backtracking.
/// Crea niveles progresivamente más complejos.
/// </summary>
public class MazeGenerator : MonoBehaviour
{
    [Header("Configuración de Tamaño")]
    [SerializeField] private int baseWidth = 10;
    [SerializeField] private int baseHeight = 10;
    [SerializeField] private int maxSize = 30;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private GameObject trapPrefab;
    
    [Header("Contenedores")]
    [SerializeField] private Transform mazeContainer;
    [SerializeField] private Transform itemsContainer;

    // Grid del laberinto
    private Cell[,] grid;
    private int width;
    private int height;
    private Vector2Int playerSpawn;
    private Vector2Int doorPosition;
    private List<Vector2Int> emptyCells = new List<Vector2Int>();
    
    private enum CellType { Wall, Floor, Door, Trap }
    
    private struct Cell
    {
        public CellType Type;
        public bool Visited;
        public int X, Y;
    }

    public void GenerateLevel(int level)
    {
        ClearMaze();
        CalculateDimensions(level);
        InitializeGrid();
        GenerateMazeDFS();
        PlaceItems(level);
        BuildVisualMaze();
    }

    /// <summary>
    /// Calcula dimensiones basadas en el nivel (dificultad progresiva)
    /// </summary>
    private void CalculateDimensions(int level)
    {
        // Crecimiento progresivo: cada 2 niveles aumenta el tamaño
        int sizeIncrease = (level - 1) / 2;
        width = Mathf.Min(baseWidth + sizeIncrease * 2, maxSize);
        height = Mathf.Min(baseHeight + sizeIncrease * 2, maxSize);
        
        // Asegurar dimensiones impares para algoritmo DFS
        if (width % 2 == 0) width++;
        if (height % 2 == 0) height++;
    }

    private void InitializeGrid()
    {
        grid = new Cell[width, height];
        emptyCells.Clear();
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new Cell 
                { 
                    Type = CellType.Wall, 
                    Visited = false,
                    X = x, 
                    Y = y 
                };
            }
        }
    }

    /// <summary>
    /// Algoritmo DFS con backtracking para generar laberinto perfecto
    /// </summary>
    private void GenerateMazeDFS()
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        
        // Empezar desde (1,1) - siempre impar para mantener paredes
        Vector2Int start = new Vector2Int(1, 1);
        grid[start.x, start.y].Type = CellType.Floor;
        grid[start.x, start.y].Visited = true;
        stack.Push(start);
        
        playerSpawn = start;

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];
                
                // Remover pared entre current y next
                int wallX = (current.x + next.x) / 2;
                int wallY = (current.y + next.y) / 2;
                grid[wallX, wallY].Type = CellType.Floor;
                
                // Marcar siguiente celda
                grid[next.x, next.y].Type = CellType.Floor;
                grid[next.x, next.y].Visited = true;
                stack.Push(next);
            }
            else
            {
                stack.Pop();
            }
        }

        // Encontrar la celda más lejana para la puerta (máxima distancia Manhattan)
        doorPosition = FindFarthestCell(playerSpawn);
        grid[doorPosition.x, doorPosition.y].Type = CellType.Door;
    }

    private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        int[] dx = { 0, 0, 2, -2 };
        int[] dy = { 2, -2, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int nx = cell.x + dx[i];
            int ny = cell.y + dy[i];

            if (nx > 0 && nx < width - 1 && ny > 0 && ny < height - 1)
            {
                if (!grid[nx, ny].Visited)
                {
                    neighbors.Add(new Vector2Int(nx, ny));
                }
            }
        }
        return neighbors;
    }

    private Vector2Int FindFarthestCell(Vector2Int from)
    {
        Vector2Int farthest = from;
        int maxDist = 0;

        for (int x = 1; x < width - 1; x += 2)
        {
            for (int y = 1; y < height - 1; y += 2)
            {
                if (grid[x, y].Type == CellType.Floor)
                {
                    int dist = Mathf.Abs(x - from.x) + Mathf.Abs(y - from.y);
                    if (dist > maxDist)
                    {
                        maxDist = dist;
                        farthest = new Vector2Int(x, y);
                    }
                }
            }
        }
        return farthest;
    }

    /// <summary>
    /// Coloca ítems basado en la dificultad del nivel
    /// </summary>
    private void PlaceItems(int level)
    {
        // Recolectar celdas de piso disponibles
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (grid[x, y].Type == CellType.Floor && 
                    !(x == playerSpawn.x && y == playerSpawn.y))
                {
                    emptyCells.Add(new Vector2Int(x, y));
                }
            }
        }

        // Colocar llave (lejos del inicio, cerca de la puerta)
        PlaceKeyNearDoor();

        // Colocar monedas (más monedas en niveles altos)
        int coinCount = 3 + level * 2;
        PlaceItemsRandomly(coinPrefab, coinCount);

        // Colocar trampas (aumentan con la dificultad)
        int trapCount = Mathf.Min(level, emptyCells.Count / 4);
        PlaceTraps(trapCount);
    }

    private void PlaceKeyNearDoor()
    {
        // Encontrar celda válida cerca de la puerta pero no adyacente
        List<Vector2Int> candidates = new List<Vector2Int>();
        
        for (int x = -3; x <= 3; x++)
        {
            for (int y = -3; y <= 3; y++)
            {
                Vector2Int pos = new Vector2Int(doorPosition.x + x, doorPosition.y + y);
                if (IsValidFloor(pos) && emptyCells.Contains(pos))
                {
                    // No demasiado cerca
                    if (Mathf.Abs(x) + Mathf.Abs(y) > 1)
                    {
                        candidates.Add(pos);
                    }
                }
            }
        }

        if (candidates.Count > 0)
        {
            Vector2Int keyPos = candidates[Random.Range(0, candidates.Count)];
            SpawnItem(keyPrefab, keyPos);
            emptyCells.Remove(keyPos);
        }
    }

    private void PlaceItemsRandomly(GameObject prefab, int count)
    {
        for (int i = 0; i < count && emptyCells.Count > 0; i++)
        {
            int index = Random.Range(0, emptyCells.Count);
            Vector2Int pos = emptyCells[index];
            SpawnItem(prefab, pos);
            emptyCells.RemoveAt(index);
        }
    }

    private void PlaceTraps(int count)
    {
        for (int i = 0; i < count && emptyCells.Count > 0; i++)
        {
            int index = Random.Range(0, emptyCells.Count);
            Vector2Int pos = emptyCells[index];
            
            // No colocar trampas en el camino óptimo (simplificación: evitar cerca del spawn)
            if (Vector2Int.Distance(pos, playerSpawn) > 3)
            {
                grid[pos.x, pos.y].Type = CellType.Trap;
            }
            emptyCells.RemoveAt(index);
        }
    }

    private bool IsValidFloor(Vector2Int pos)
    {
        return pos.x > 0 && pos.x < width - 1 && 
               pos.y > 0 && pos.y < height - 1 && 
               grid[pos.x, pos.y].Type == CellType.Floor;
    }

    private void SpawnItem(GameObject prefab, Vector2Int gridPos)
    {
        if (prefab == null) return;
        
        Vector3 worldPos = GridToWorld(gridPos);
        GameObject item = Instantiate(prefab, worldPos, Quaternion.identity, itemsContainer);
        
        // Asegurar que tenga el componente correcto
        if (prefab == coinPrefab && item.GetComponent<Coin>() == null)
        {
            item.AddComponent<Coin>();
        }
        else if (prefab == keyPrefab && item.GetComponent<Key>() == null)
        {
            item.AddComponent<Key>();
        }
    }

    private void BuildVisualMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = GridToWorld(new Vector2Int(x, y));
                GameObject prefab = null;

                switch (grid[x, y].Type)
                {
                    case CellType.Wall:
                        prefab = wallPrefab;
                        break;
                    case CellType.Floor:
                        prefab = floorPrefab;
                        break;
                    case CellType.Door:
                        prefab = doorPrefab;
                        break;
                    case CellType.Trap:
                        prefab = trapPrefab;
                        break;
                }

                if (prefab != null)
                {
                    GameObject tile = Instantiate(prefab, pos, Quaternion.identity, mazeContainer);
                    
                    // Configurar puerta
                    if (grid[x, y].Type == CellType.Door)
                    {
                        Door door = tile.GetComponent<Door>();
                        if (door == null) door = tile.AddComponent<Door>();
                    }
                }
            }
        }
    }

    private Vector3 GridToWorld(Vector2Int gridPos)
    {
        // Centrar el laberinto en el mundo
        float offsetX = -(width * 0.5f);
        float offsetY = -(height * 0.5f);
        return new Vector3(gridPos.x + offsetX, gridPos.y + offsetY, 0);
    }

    public Vector2 GetPlayerSpawnPosition()
    {
        return GridToWorld(playerSpawn);
    }

    private void ClearMaze()
    {
        // Destruir objetos existentes
        foreach (Transform child in mazeContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }
    }
}