using UnityEngine;
using System;
using GridMind.Grid;
using GridMind.Player;

namespace GridMind.Puzzle
{
    [Serializable]
    public class LevelData
    {
        public string levelName;
        public int    width;
        public int    height;
        public int[]  tiles;          // row-major, TileType codes
        public int[]  playerStart;    // [x, y]
        public int[][]boxPositions;   // [[x,y], ...]
        public int    parMoves;
    }

    /// <summary>
    /// Loads a LevelData from a JSON TextAsset located in Resources/Levels/.
    /// Spawns boxes and positions the player according to the level definition.
    /// </summary>
    public class LevelLoader : MonoBehaviour
    {
        public static LevelLoader Instance { get; private set; }

        [Header("Prefabs")]
        [SerializeField] private GameObject boxPrefab;
        [SerializeField] private GameObject playerPrefab;

        [Header("Parents")]
        [SerializeField] private Transform boxParent;
        [SerializeField] private Transform entityParent;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        /// <summary>
        /// Loads level by 1-based index (e.g. level_01, level_02 …).
        /// </summary>
        public LevelData LoadLevel(int levelIndex)
        {
            string path       = $"Levels/level_{levelIndex:D2}";
            TextAsset asset   = Resources.Load<TextAsset>(path);

            if (asset == null)
            {
                Debug.LogError($"[LevelLoader] Could not find level at Resources/{path}.json");
                return null;
            }

            LevelData data = JsonUtility.FromJson<LevelData>(asset.text);
            SpawnLevel(data);
            return data;
        }

        private void SpawnLevel(LevelData data)
        {
            // Grid tiles
            GridManager.Instance.InitialiseGrid(data);

            // Clear old boxes
            BoxManager.Instance.Clear();
            foreach (Transform child in boxParent) Destroy(child.gameObject);

            // Spawn boxes
            if (data.boxPositions != null)
            {
                foreach (int[] bp in data.boxPositions)
                {
                    Vector2Int gp  = new Vector2Int(bp[0], bp[1]);
                    Vector3    wp  = GridManager.Instance.GridToWorld(gp);
                    Instantiate(boxPrefab, wp, Quaternion.identity, boxParent);
                }
            }

            // Position player
            PlacePlayer(data);
        }

        private void PlacePlayer(LevelData data)
        {
            Vector2Int startPos = new Vector2Int(data.playerStart[0], data.playerStart[1]);

            // Try to find existing player
            PlayerController existing = FindObjectOfType<PlayerController>();
            if (existing != null)
            {
                existing.Teleport(startPos);
                return;
            }

            // Spawn fresh
            Vector3 wp = GridManager.Instance.GridToWorld(startPos);
            Instantiate(playerPrefab, wp, Quaternion.identity, entityParent);
        }
    }
}
