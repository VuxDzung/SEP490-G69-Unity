namespace SEP490G69.GameSessions
{
    using SEP490G69.Addons.Localization;
    using SEP490G69.Addons.Localization.Enums;
    using SEP490G69.Shared;
    using System.Linq;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITitleSettingsFrame : GameUIFrame
    {
        [SerializeField] private UILinearSwitcher m_ResolutionSwitcher;
        [SerializeField] private UILinearSwitcher m_GraphicQualitySwitcher;
        [SerializeField] private UILinearSwitcher m_SoundVolSwitcher;
        [SerializeField] private UILinearSwitcher m_MusicVolSwitcher;
        [SerializeField] private UILinearSwitcher m_LanguageSwitcher;
        [SerializeField] private UILinearSwitcher m_FPSLimitSwitcher;
        [SerializeField] private Button m_ApplyBtn;
        [SerializeField] private Button m_BackBtn;

        private int _selectedLangIndex = 0;
        private int _selectedResolutionIndex = 0;
        private int _selectedQualityIndex = 0;
        private int _selectedSoundIndex = 5;
        private int _selectedMusicIndex = 5;
        private int _selectedFPSIndex = 0;
        private LocalizationManager localizationManager;
        private Resolution[] _resolutions;
        public LocalizationManager LocalizationManager
        {
            get
            {
                if (localizationManager == null)
                {
                    localizationManager = ContextManager.Singleton.ResolveGameContext<LocalizationManager>();
                }
                return localizationManager;
            }
        }

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_ResolutionSwitcher.Enable();
            m_GraphicQualitySwitcher.Enable();
            m_SoundVolSwitcher.Enable();
            m_MusicVolSwitcher.Enable();
            m_LanguageSwitcher.Enable();
            m_FPSLimitSwitcher.Enable();
            m_ApplyBtn.onClick.AddListener(Apply);
            m_BackBtn.onClick.AddListener(Back);
            m_ResolutionSwitcher.OnChanged += OnResolutionChanged;
            m_GraphicQualitySwitcher.OnChanged += OnQualityChanged;
            m_SoundVolSwitcher.OnChanged += OnSoundChanged;
            m_MusicVolSwitcher.OnChanged += OnMusicChanged;
            m_FPSLimitSwitcher.OnChanged += OnFPSChanged;

            m_LanguageSwitcher.OnChanged += OnLanguageIndexChanged;
            LoadExistedSettings();
            LoadExistedLanguage();
            LoadLanguages();
            LoadSwitchers();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_ResolutionSwitcher.Disable();
            m_GraphicQualitySwitcher.Disable();
            m_SoundVolSwitcher.Disable();
            m_MusicVolSwitcher.Disable();
            m_LanguageSwitcher.Disable();
            m_FPSLimitSwitcher.Disable();
            m_ApplyBtn.onClick.RemoveListener(Apply);
            m_BackBtn.onClick.RemoveListener(Back);

            m_LanguageSwitcher.OnChanged -= OnLanguageIndexChanged;
            m_ResolutionSwitcher.OnChanged -= OnResolutionChanged;
            m_GraphicQualitySwitcher.OnChanged -= OnQualityChanged;
            m_SoundVolSwitcher.OnChanged -= OnSoundChanged;
            m_MusicVolSwitcher.OnChanged -= OnMusicChanged;
            m_FPSLimitSwitcher.OnChanged -= OnFPSChanged;
        }

        private void Apply()
        {
            ConfirmLanguage();
            ApplyQuality();
            ApplyAudio();
            ApplyFPS();
            ApplyResolution();
            PlayerPrefs.Save();
        }

        private void Back()
        {
            UIManager.HideFrame(FrameId);
        }

        private void LoadLanguages()
        {
            m_LanguageSwitcher.SetContents(GameConstants.LANGUAGES.Select(e => e.ToString()).ToArray(), _selectedLangIndex);
        }
        private void LoadExistedLanguage()
        {
            string langStr = PlayerPrefs.GetString("Language");
            if (!string.IsNullOrEmpty(langStr))
            {
                ELocalizeLanguageType lang = Enum.Parse<ELocalizeLanguageType>(langStr);
                _selectedLangIndex = GameConstants.LANGUAGES.IndexOf(lang);
            }
            else
            {
                Debug.Log("No selected language yet");
            }
        }

        private void OnLanguageIndexChanged(int index)
        {
            Debug.Log($"Index: {index}");
            _selectedLangIndex = index;
        }
        private void OnResolutionChanged(int index)
        {
            _selectedResolutionIndex = index;
        }

        private void OnQualityChanged(int index)
        {
            Debug.Log($"Index: {index}");
            _selectedQualityIndex = index;
        }

        private void OnSoundChanged(int index)
        {
            Debug.Log($"Index: {index}");
            _selectedSoundIndex = index;
        }

        private void OnMusicChanged(int index)
        {
            Debug.Log($"Index: {index}");
            _selectedMusicIndex = index;
        }

        private void OnFPSChanged(int index)
        {
            Debug.Log($"Index: {index}");
            _selectedFPSIndex = index;
        }
        public void ConfirmLanguage()
        {
            ELocalizeLanguageType lang = GameConstants.LANGUAGES[_selectedLangIndex];
            LocalizationManager.SetLanguage(lang);
            // Save language index to data here.
            PlayerPrefs.SetString("Language", GameConstants.LANGUAGES[_selectedLangIndex].ToString());

            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_LOGIN);
        }

        private void LoadSwitchers()
        {
            // Resolution
            _resolutions = Screen.resolutions;
            string[] resolutionOptions = _resolutions
                .Select(r => $"{r.width}x{r.height}")
                .ToArray();

            m_ResolutionSwitcher.SetContents(resolutionOptions, _selectedResolutionIndex);

            // Graphic Quality
            string[] qualities = QualitySettings.names;
            m_GraphicQualitySwitcher.SetContents(qualities, _selectedQualityIndex);

            // Sound & Music (0 → 10)
            string[] volumes = Enumerable.Range(0, 11).Select(v => v.ToString()).ToArray();
            m_SoundVolSwitcher.SetContents(volumes, _selectedSoundIndex);
            m_MusicVolSwitcher.SetContents(volumes, _selectedMusicIndex);

            // FPS Limit
            string[] fpsOptions = GameConstants.FPS_LIMITS
            .Select(f => f <= 0 ? "Unlimited" : f.ToString())
            .ToArray();

            m_FPSLimitSwitcher.SetContents(fpsOptions, _selectedFPSIndex);
            m_FPSLimitSwitcher.SetContents(fpsOptions, _selectedFPSIndex);

            // Language (keep your logic)
            LoadLanguages();
        }

        private void LoadExistedSettings()
        {
            _selectedResolutionIndex = PlayerPrefs.GetInt("Resolution", 0);
            _selectedQualityIndex = PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel());
            _selectedSoundIndex = PlayerPrefs.GetInt("Sound", 5);
            _selectedMusicIndex = PlayerPrefs.GetInt("Music", 5);
            _selectedFPSIndex = PlayerPrefs.GetInt("FPS", 0);

            LoadExistedLanguage();
        }

        private void ApplyQuality()
        {
            QualitySettings.SetQualityLevel(_selectedQualityIndex);
            PlayerPrefs.SetInt("Quality", _selectedQualityIndex);

            UIManager.HideFrame(FrameId);
        }

        private void ApplyAudio()
        {
            float soundVolume = _selectedSoundIndex / 10f;
            float musicVolume = _selectedMusicIndex / 10f;

            var audioManager = ContextManager.Singleton.ResolveGameContext<AudioManager>();

            audioManager.SetSFXVolume(soundVolume);
            audioManager.SetBGMVolume(musicVolume);

            PlayerPrefs.SetInt("Sound", _selectedSoundIndex);
            PlayerPrefs.SetInt("Music", _selectedMusicIndex);
        }

        private void ApplyFPS()
        {
            int fps = GameConstants.FPS_LIMITS[_selectedFPSIndex];

            if (fps > 0)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = fps;
            }
            else
            {
                Application.targetFrameRate = -1;
            }

            PlayerPrefs.SetInt("FPS", _selectedFPSIndex);
            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_LOGIN);
        }

        
        private void ApplyResolution()
        {
            Resolution selected = _resolutions[_selectedResolutionIndex];
            Screen.SetResolution(
            selected.width,
            selected.height,
            Screen.fullScreenMode
            );
            PlayerPrefs.SetInt("Resolution", _selectedResolutionIndex);

            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_LOGIN);
        }

    }
}