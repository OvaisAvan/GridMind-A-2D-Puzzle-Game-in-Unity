using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GridMind.Managers;

namespace GridMind.UI
{
    /// <summary>
    /// Controls the main menu scene: level selection, settings panel, credits.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject levelSelectPanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("Level Select")]
        [SerializeField] private Transform  levelButtonContainer;
        [SerializeField] private GameObject levelButtonPrefab;
        [SerializeField] private int        totalLevels = 3;

        [Header("Settings")]
        [SerializeField] private Slider  musicSlider;
        [SerializeField] private Slider  sfxSlider;
        [SerializeField] private Toggle  musicMuteToggle;

        [Header("Main Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button levelSelectButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            AudioManager.Instance?.PlayMenuMusic();

            ShowPanel(mainPanel);

            // Wire main buttons
            playButton?.onClick.AddListener(OnPlayClicked);
            levelSelectButton?.onClick.AddListener(() => { SfxClick(); ShowPanel(levelSelectPanel); BuildLevelButtons(); });
            settingsButton?.onClick.AddListener(() => { SfxClick(); ShowPanel(settingsPanel); });
            quitButton?.onClick.AddListener(() => { SfxClick(); Application.Quit(); });

            // Wire settings
            musicSlider?.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
            sfxSlider?.onValueChanged.AddListener(AudioManager.Instance.SetSfxVolume);
            musicMuteToggle?.onValueChanged.AddListener(AudioManager.Instance.SetMusicMute);
        }

        private void OnPlayClicked()
        {
            SfxClick();
            GameManager.Instance.StartGame();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }

        private void BuildLevelButtons()
        {
            foreach (Transform child in levelButtonContainer) Destroy(child.gameObject);

            for (int i = 1; i <= totalLevels; i++)
            {
                int levelIndex = i; // capture
                bool unlocked  = SaveSystem.Instance?.IsLevelUnlocked(levelIndex) ?? (i == 1);

                GameObject go     = Instantiate(levelButtonPrefab, levelButtonContainer);
                TMP_Text   label  = go.GetComponentInChildren<TMP_Text>();
                Button     btn    = go.GetComponent<Button>();

                if (label) label.text = unlocked ? $"Level {i}" : "🔒";
                if (btn)
                {
                    btn.interactable = unlocked;
                    btn.onClick.AddListener(() =>
                    {
                        SfxClick();
                        GameManager.Instance.CurrentLevelOverride(levelIndex);
                        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
                    });
                }
            }
        }

        private void ShowPanel(GameObject target)
        {
            mainPanel?.SetActive(target == mainPanel);
            levelSelectPanel?.SetActive(target == levelSelectPanel);
            settingsPanel?.SetActive(target == settingsPanel);
        }

        private void SfxClick() => AudioManager.Instance?.PlayUIClick();

        // Back buttons call this
        public void GoBack() { SfxClick(); ShowPanel(mainPanel); }
    }
}
