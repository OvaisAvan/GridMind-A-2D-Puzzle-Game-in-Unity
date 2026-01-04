using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GridMind.Managers;

namespace GridMind.UI
{
    /// <summary>
    /// Manages all in-game HUD elements: move counter, par indicator,
    /// win panel, pause menu, and level label.
    /// Assign all references in the Inspector.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("HUD")]
        [SerializeField] private TMP_Text moveCounterText;
        [SerializeField] private TMP_Text levelLabel;
        [SerializeField] private TMP_Text parLabel;

        [Header("Win Panel")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private TMP_Text   winMovesText;
        [SerializeField] private TMP_Text   winParText;
        [SerializeField] private GameObject starParAchieved;   // shown when player beats par
        [SerializeField] private Button     nextLevelButton;
        [SerializeField] private Button     restartButtonWin;

        [Header("Pause Panel")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button     resumeButton;
        [SerializeField] private Button     restartButtonPause;
        [SerializeField] private Button     mainMenuButtonPause;

        [Header("All Clear Panel")]
        [SerializeField] private GameObject allClearPanel;
        [SerializeField] private Button     mainMenuButtonClear;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            // Wire buttons
            nextLevelButton?.onClick.AddListener(() => { AudioManager.Instance?.PlayUIClick(); GameManager.Instance.NextLevel(); });
            restartButtonWin?.onClick.AddListener(() => { AudioManager.Instance?.PlayUIClick(); GameManager.Instance.RestartLevel(); });
            resumeButton?.onClick.AddListener(() => { AudioManager.Instance?.PlayUIClick(); GameManager.Instance.TogglePause(); });
            restartButtonPause?.onClick.AddListener(() => { AudioManager.Instance?.PlayUIClick(); GameManager.Instance.RestartLevel(); });
            mainMenuButtonPause?.onClick.AddListener(() => { AudioManager.Instance?.PlayUIClick(); GameManager.Instance.GoToMainMenu(); });
            mainMenuButtonClear?.onClick.AddListener(() => { AudioManager.Instance?.PlayUIClick(); GameManager.Instance.GoToMainMenu(); });

            HideWinPanel();
            pausePanel?.SetActive(false);
            allClearPanel?.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                GameManager.Instance?.TogglePause();
        }

        // ── HUD ───────────────────────────────────────────────────────────────

        public void UpdateMoveCounter(int count)
        {
            if (moveCounterText) moveCounterText.text = $"Moves: {count}";
        }

        public void SetLevelLabel(int level, int par)
        {
            if (levelLabel) levelLabel.text = $"Level {level}";
            if (parLabel)   parLabel.text   = $"Par: {par}";
        }

        // ── Win Panel ─────────────────────────────────────────────────────────

        public void ShowWinPanel(int moves, int par)
        {
            winPanel?.SetActive(true);
            if (winMovesText) winMovesText.text = $"Solved in {moves} moves";
            if (winParText)   winParText.text   = $"Par: {par}";
            if (starParAchieved) starParAchieved.SetActive(moves <= par);
        }

        public void HideWinPanel() => winPanel?.SetActive(false);

        // ── Pause / Clear ─────────────────────────────────────────────────────

        public void ShowPausePanel(bool show) => pausePanel?.SetActive(show);

        public void ShowAllClearPanel() => allClearPanel?.SetActive(true);
    }
}
