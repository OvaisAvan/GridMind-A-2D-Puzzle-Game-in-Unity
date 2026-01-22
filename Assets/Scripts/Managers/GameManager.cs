using UnityEngine;
using GridMind.Puzzle;
using GridMind.UI;

namespace GridMind.Managers
{
    public enum GameState { MainMenu, Playing, Paused, Won, GameOver }

    /// <summary>
    /// Central authority for game state, level progression, and move counting.
    /// Persists across scenes via DontDestroyOnLoad.
    /// </summary>
    public partial class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Level Settings")]
        [SerializeField] private int totalLevels = 3;

        public GameState CurrentState  { get; private set; } = GameState.MainMenu;
        public int       CurrentLevel  { get; private set; } = 1;
        public int       MoveCount     { get; private set; }
        public int       ParMoves      { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ── Public API ────────────────────────────────────────────────────────

        public void StartGame()
        {
            CurrentLevel = 1;
            LoadCurrentLevel();
        }

        public void LoadCurrentLevel()
        {
            MoveCount = 0;
            ChangeState(GameState.Playing);
            WinCondition.Instance?.ResetForNewLevel();

            LevelData data = LevelLoader.Instance?.LoadLevel(CurrentLevel);
            if (data != null) ParMoves = data.parMoves;

            UIManager.Instance?.UpdateMoveCounter(MoveCount);
            UIManager.Instance?.HideWinPanel();
        }

        public void NextLevel()
        {
            if (CurrentLevel >= totalLevels)
            {
                Debug.Log("[GameManager] All levels complete!");
                UIManager.Instance?.ShowAllClearPanel();
                return;
            }
            CurrentLevel++;
            LoadCurrentLevel();
        }

        public void RestartLevel()
        {
            LoadCurrentLevel();
        }

        public void RegisterMove()
        {
            MoveCount++;
            UIManager.Instance?.UpdateMoveCounter(MoveCount);
        }

        public void TogglePause()
        {
            if (CurrentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
                Time.timeScale = 0f;
            }
            else if (CurrentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
                Time.timeScale = 1f;
            }
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            ChangeState(GameState.MainMenu);
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        private void ChangeState(GameState newState)
        {
            CurrentState = newState;
            Debug.Log($"[GameManager] State → {newState}");
        }
    }
}
