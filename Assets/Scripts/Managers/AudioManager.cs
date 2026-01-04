using UnityEngine;

namespace GridMind.Managers
{
    /// <summary>
    /// Centralised audio manager. Assign clips in the Inspector.
    /// Supports global SFX volume and music mute toggle persisted via PlayerPrefs.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Music")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioClip   menuMusic;
        [SerializeField] private AudioClip   gameplayMusic;

        [Header("SFX")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioClip   stepClip;
        [SerializeField] private AudioClip   boxSlideClip;
        [SerializeField] private AudioClip   boxLandClip;
        [SerializeField] private AudioClip   winClip;
        [SerializeField] private AudioClip   uiClickClip;

        [Header("Defaults")]
        [SerializeField] [Range(0f, 1f)] private float defaultMusicVolume = 0.4f;
        [SerializeField] [Range(0f, 1f)] private float defaultSfxVolume   = 0.8f;

        private const string PrefMusicVol = "GridMind_MusicVol";
        private const string PrefSfxVol   = "GridMind_SfxVol";
        private const string PrefMusicMute = "GridMind_MusicMute";

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadPrefs();
        }

        // ── Music ─────────────────────────────────────────────────────────────

        public void PlayMenuMusic()     => SwitchMusic(menuMusic);
        public void PlayGameplayMusic() => SwitchMusic(gameplayMusic);

        private void SwitchMusic(AudioClip clip)
        {
            if (musicSource == null || clip == null) return;
            if (musicSource.clip == clip && musicSource.isPlaying) return;
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }

        public void SetMusicMute(bool muted)
        {
            if (musicSource) musicSource.mute = muted;
            PlayerPrefs.SetInt(PrefMusicMute, muted ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetMusicVolume(float v)
        {
            if (musicSource) musicSource.volume = v;
            PlayerPrefs.SetFloat(PrefMusicVol, v);
            PlayerPrefs.Save();
        }

        public void SetSfxVolume(float v)
        {
            if (sfxSource) sfxSource.volume = v;
            PlayerPrefs.SetFloat(PrefSfxVol, v);
            PlayerPrefs.Save();
        }

        // ── SFX ───────────────────────────────────────────────────────────────

        public void PlayStep()     => PlaySfx(stepClip);
        public void PlayBoxSlide() => PlaySfx(boxSlideClip);
        public void PlayBoxLand()  => PlaySfx(boxLandClip);
        public void PlayWin()      => PlaySfx(winClip);
        public void PlayUIClick()  => PlaySfx(uiClickClip);

        private void PlaySfx(AudioClip clip)
        {
            if (sfxSource == null || clip == null) return;
            sfxSource.PlayOneShot(clip);
        }

        // ── Prefs ─────────────────────────────────────────────────────────────

        private void LoadPrefs()
        {
            float musicVol  = PlayerPrefs.GetFloat(PrefMusicVol, defaultMusicVolume);
            float sfxVol    = PlayerPrefs.GetFloat(PrefSfxVol,   defaultSfxVolume);
            bool  musicMute = PlayerPrefs.GetInt(PrefMusicMute, 0) == 1;

            if (musicSource)
            {
                musicSource.volume = musicVol;
                musicSource.mute   = musicMute;
            }
            if (sfxSource) sfxSource.volume = sfxVol;
        }
    }
}
