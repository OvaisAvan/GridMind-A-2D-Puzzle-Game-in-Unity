using UnityEngine;
using System;

namespace GridMind.Managers
{
    [Serializable]
    public class LevelSaveData
    {
        public int  bestMoves;
        public bool achievedPar;
        public bool unlocked;
    }

    /// <summary>
    /// Lightweight save system using PlayerPrefs (JSON per level).
    /// Call SaveLevelResult() after win; call GetLevelData() to read.
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        private const string KeyPrefix = "GridMind_Level_";

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Always unlock level 1
            UnlockLevel(1);
        }

        // ── Write ─────────────────────────────────────────────────────────────

        public void SaveLevelResult(int levelIndex, int moves, bool achievedPar)
        {
            LevelSaveData existing = GetLevelData(levelIndex);
            bool improved = existing.bestMoves == 0 || moves < existing.bestMoves;

            LevelSaveData updated = new LevelSaveData
            {
                bestMoves    = improved ? moves : existing.bestMoves,
                achievedPar  = existing.achievedPar || achievedPar,
                unlocked     = true
            };

            string json = JsonUtility.ToJson(updated);
            PlayerPrefs.SetString(Key(levelIndex), json);
            PlayerPrefs.Save();

            // Unlock next level
            UnlockLevel(levelIndex + 1);

            Debug.Log($"[SaveSystem] Level {levelIndex} saved. Best: {updated.bestMoves} moves. Par: {updated.achievedPar}");
        }

        public void UnlockLevel(int levelIndex)
        {
            LevelSaveData data = GetLevelData(levelIndex);
            if (data.unlocked) return;
            data.unlocked = true;
            PlayerPrefs.SetString(Key(levelIndex), JsonUtility.ToJson(data));
            PlayerPrefs.Save();
        }

        // ── Read ──────────────────────────────────────────────────────────────

        public LevelSaveData GetLevelData(int levelIndex)
        {
            string json = PlayerPrefs.GetString(Key(levelIndex), "");
            if (string.IsNullOrEmpty(json))
                return new LevelSaveData { unlocked = levelIndex == 1 };
            return JsonUtility.FromJson<LevelSaveData>(json);
        }

        public bool IsLevelUnlocked(int levelIndex) => GetLevelData(levelIndex).unlocked;

        // ── Utility ───────────────────────────────────────────────────────────

        public void ResetAllData()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            UnlockLevel(1);
            Debug.Log("[SaveSystem] All save data cleared.");
        }

        private static string Key(int index) => $"{KeyPrefix}{index}";
    }
}
