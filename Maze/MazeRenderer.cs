using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeRenderer : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap wallTilemap;
    public Tilemap floorTilemap;
    
    [Header("Tiles 8-Bit")]
    public TileBase wallTile;
    public TileBase floorTile;
    public TileBase startTile;
    public TileBase exitTile;
    
    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject exitPrefab;
    
    public void Render(CellType[,] grid)
    {
        wallTilemap.ClearAllTiles();
        floorTilemap.ClearAllTiles();
        
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                
                switch (grid[x, y])
                {
                    case CellType.Wall:
                        wallTilemap.SetTile(pos, wallTile);
                        break;
                    case CellType.Path:
                        floorTilemap.SetTile(pos, floorTile);
                        break;
                    case CellType.Start:
                        floorTilemap.SetTile(pos, startTile);
                        Instantiate(playerPrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                        break;
                    case CellType.Exit:
                        floorTilemap.SetTile(pos, exitTile);
                        Instantiate(exitPrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                        break;
                }
            }
        }
        
        // Centrar cámara
        Camera.main.transform.position = new Vector3(width/2f, height/2f, -10);
    }
}