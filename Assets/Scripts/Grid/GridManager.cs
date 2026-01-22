using UnityEngine;
using System.Collections.Generic;
using GridMind.Puzzle;

namespace GridMind.Grid
{
    /// <summary>
    /// Manages the game grid: tile creation, position tracking, and movement validation.
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [Header("Grid Settings")]
        [SerializeField] private int gridWidth = 6;
        [SerializeField] private int gridHeight = 6;
        [SerializeField] private float cellSize = 1.0f;

        [Header("Prefabs")]
        [SerializeField] private GameObject floorTilePrefab;
        [SerializeField] private GameObject wallTilePrefab;
        [SerializeField] private GameObject goalTilePrefab;

        private TileController[,] grid;
        private Vector2Int playerGridPosition;

        public int Width => gridWidth;
        public int Height => gridHeight;
        public float CellSize => cellSize;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        /// <summary>
        /// Initialises the grid from a parsed level data object.
        /// </summary>
        public void InitialiseGrid(LevelData data)
        {
            ClearGrid();

            gridWidth  = data.width;
            gridHeight = data.height;
            grid       = new TileController[gridWidth, gridHeight];

            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    int tileCode = data.tiles[y * gridWidth + x];
                    Vector3 worldPos = GridToWorld(new Vector2Int(x, y));
                    SpawnTile(tileCode, x, y, worldPos);
                }
            }
        }

        private void SpawnTile(int code, int x, int y, Vector3 worldPos)
        {
            GameObject prefab = code switch
            {
                1 => wallTilePrefab,
                2 => goalTilePrefab,
                _ => floorTilePrefab
            };

            if (prefab == null) return;

            GameObject go = Instantiate(prefab, worldPos, Quaternion.identity, transform);
            TileController tc = go.GetComponent<TileController>();
            if (tc != null)
            {
                tc.Initialise(x, y, (TileType)code);
                grid[x, y] = tc;
            }
        }

        /// <summary>
        /// Returns whether a grid cell is walkable (not a wall, within bounds).
        /// </summary>
        public bool IsWalkable(Vector2Int gridPos)
        {
            if (!IsInBounds(gridPos)) return false;
            TileController tile = grid[gridPos.x, gridPos.y];
            return tile != null && tile.TileType != TileType.Wall;
        }

        public bool IsInBounds(Vector2Int pos) =>
            pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight;

        public TileController GetTile(Vector2Int pos) =>
            IsInBounds(pos) ? grid[pos.x, pos.y] : null;

        public Vector3 GridToWorld(Vector2Int gridPos) =>
            new Vector3(gridPos.x * cellSize, gridPos.y * cellSize, 0f);

        public Vector2Int WorldToGrid(Vector3 worldPos) =>
            new Vector2Int(Mathf.RoundToInt(worldPos.x / cellSize),
                           Mathf.RoundToInt(worldPos.y / cellSize));

        public void SetPlayerPosition(Vector2Int pos) => playerGridPosition = pos;
        public Vector2Int GetPlayerPosition()         => playerGridPosition;

        private void ClearGrid()
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            grid = null;
        }
    }
}
