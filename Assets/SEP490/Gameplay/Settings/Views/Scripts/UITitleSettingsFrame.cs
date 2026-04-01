namespace SEP490G69.GameSessions
{
    using SEP490G69.Addons.Localization.Enums;
    using System.Linq;
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class UITitleSettingsFrame : GameUIFrame
    {
        [SerializeField] private TMP_Dropdown m_ResolutionSwitcher;
        [SerializeField] private TMP_Dropdown m_GraphicQualitySwitcher;
        [SerializeField] private UISlider m_SoundVolSwitcher;
        [SerializeField] private UISlider m_MusicVolSwitcher;
        [SerializeField] private TMP_Dropdown m_LanguageSwitcher;
        [SerializeField] private TMP_Dropdown m_FPSLimitSwitcher;
        [SerializeField] private Button m_ApplyBtn;
        [SerializeField] private Button m_BackBtn;
        [SerializeField] private string m_PrevFrameId;

        private int _selectedLangIndex = 0;
        private int _selectedResolutionIndex = 0;
        private int _selectedQualityIndex = 0;
        private float _sfxVolume = 5;
        private float _bgmVolume = 5;
        private int _selectedFPSIndex = 0;
        private Resolution[] _resolutions;


        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_SoundVolSwitcher.Enable();
            m_MusicVolSwitcher.Enable();

            m_ApplyBtn.onClick.AddListener(Apply);
            m_BackBtn.onClick.AddListener(Back);

            m_ResolutionSwitcher.onValueChanged.AddListener(OnResolutionChanged);
            m_GraphicQualitySwitcher.onValueChanged.AddListener(OnQualityChanged);
            m_SoundVolSwitcher.onValueChanged += OnSoundChanged;
            m_MusicVolSwitcher.onValueChanged += OnMusicChanged;
            m_FPSLimitSwitcher.onValueChanged.AddListener(OnFPSChanged);

            m_LanguageSwitcher.onValueChanged.AddListener(OnLanguageIndexChanged);

            LoadExistedSettings();
            LoadExistedLanguage();
            LoadLanguages();
            LoadSwitchers();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_SoundVolSwitcher.Disable();
            m_MusicVolSwitcher.Disable();

            m_ApplyBtn.onClick.RemoveListener(Apply);
            m_BackBtn.onClick.RemoveListener(Back);

            m_ResolutionSwitcher.onValueChanged.RemoveListener(OnResolutionChanged);
            m_GraphicQualitySwitcher.onValueChanged.RemoveListener(OnQualityChanged);
            m_SoundVolSwitcher.onValueChanged -= OnSoundChanged;
            m_MusicVolSwitcher.onValueChanged -= OnMusicChanged;
            m_FPSLimitSwitcher.onValueChanged.RemoveListener(OnFPSChanged);

            m_LanguageSwitcher.onValueChanged.RemoveListener(OnLanguageIndexChanged);
        }

        private void Apply()
        {
            ConfirmLanguage();
            ApplyQuality();
            ApplyAudio();
            ApplyFPS();
            ApplyResolution();
            PlayerPrefs.Save();

            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(m_PrevFrameId);
        }

        private void Back()
        {
            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(m_PrevFrameId);
        }

        private void LoadLanguages()
        {
            m_LanguageSwitcher.ClearOptions();
            foreach (var lang in GameConstants.LANGUAGES.Select(e => e.ToString()).ToArray())
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
                {
                    text = lang,
                };
                m_LanguageSwitcher.options.Add(option);
            }
        }

        private void LoadExistedLanguage()
        {
            string langStr = PlayerPrefs.GetString("Language");
            if (!string.IsNullOrEmpty(langStr))
            {
                ELocalizeLanguageType lang = Enum.Parse<ELocalizeLanguageType>(langStr);
                _selectedLangIndex = GameConstants.LANGUAGES.IndexOf(LocalizeManager.CurrentLanguage);
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

        private void OnSoundChanged(float index)
        {
            _sfxVolume = index;
            m_SoundVolSwitcher.SetValue(_sfxVolume, 1f);
        }

        private void OnMusicChanged(float index)
        {
            _bgmVolume = index;
            m_MusicVolSwitcher.SetValue(_sfxVolume, 1f);
        }

        private void OnFPSChanged(int index)
        {
            Debug.Log($"Index: {index}");
            _selectedFPSIndex = index;
        }

        public void ConfirmLanguage()
        {
            ELocalizeLanguageType lang = GameConstants.LANGUAGES[_selectedLangIndex];
            LocalizeManager.SetLanguage(lang);

            PlayerPrefs.SetString(GameConstants.PREF_KEY_LANGUAGE, GameConstants.LANGUAGES[_selectedLangIndex].ToString());
        }

        private void LoadSwitchers()
        {
            // Resolution
            _resolutions = Screen.resolutions;
            string[] resolutionOptions = _resolutions
                .Select(r => $"{r.width}x{r.height}")
                .ToArray();

            m_ResolutionSwitcher.ClearOptions();

            foreach (var res in resolutionOptions)
            {
                m_ResolutionSwitcher.options.Add(new TMP_Dropdown.OptionData
                {
                    text = res,
                });
            }
            m_ResolutionSwitcher.value = _selectedResolutionIndex;

            // Graphic Quality
            string[] qualities = QualitySettings.names;
            m_GraphicQualitySwitcher.ClearOptions();
            foreach (var qual in qualities)
            {
                m_GraphicQualitySwitcher.options.Add(new TMP_Dropdown.OptionData
                {
                    text = qual,
                });
            }
            m_GraphicQualitySwitcher.value = _selectedQualityIndex;

            // Sound & Music (0 → 10)
            string[] volumes = Enumerable.Range(0, 11).Select(v => v.ToString()).ToArray();
            m_SoundVolSwitcher.SetValue(_sfxVolume, 1f);
            m_MusicVolSwitcher.SetValue(_bgmVolume, 1f);

            // FPS Limit
            string[] fpsOptions = GameConstants.FPS_LIMITS
            .Select(f => f <= 0 ? "Unlimited" : f.ToString())
            .ToArray();

            m_FPSLimitSwitcher.ClearOptions();

            foreach (var fps in fpsOptions)
            {
                m_FPSLimitSwitcher.options.Add(new TMP_Dropdown.OptionData
                {
                    text = fps,
                });
            }
            m_FPSLimitSwitcher.value = _selectedFPSIndex;

            // Language (keep your logic)
            LoadLanguages();
        }

        private void LoadExistedSettings()
        {
            _selectedResolutionIndex = PlayerPrefs.GetInt(GameConstants.PREF_KEY_RESOLUTION, 0);
            Debug.Log($"Selected resolution: {_selectedResolutionIndex}");
            _selectedQualityIndex = PlayerPrefs.GetInt(GameConstants.PREF_KEY_QUALITY, QualitySettings.GetQualityLevel());
            _sfxVolume = PlayerPrefs.GetFloat(GameConstants.PREF_KEY_SOUND, 1f);
            _bgmVolume = PlayerPrefs.GetFloat(GameConstants.PREF_KEY_MUSIC, 1f);
            _selectedFPSIndex = PlayerPrefs.GetInt(GameConstants.PREF_KEY_FPS, 0);

            LoadExistedLanguage();
        }

        private void ApplyQuality()
        {
            QualitySettings.SetQualityLevel(_selectedQualityIndex);
            PlayerPrefs.SetInt(GameConstants.PREF_KEY_QUALITY, _selectedQualityIndex);
        }

        private void ApplyAudio()
        {
            float soundVolume = _sfxVolume;
            float musicVolume = _bgmVolume;

            var audioManager = ContextManager.Singleton.ResolveGameContext<AudioManager>();

            audioManager.SetSFXVolume(soundVolume);
            audioManager.SetBGMVolume(musicVolume);

            PlayerPrefs.SetFloat(GameConstants.PREF_KEY_SOUND, _sfxVolume);
            PlayerPrefs.SetFloat(GameConstants.PREF_KEY_MUSIC, _bgmVolume);
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

            PlayerPrefs.SetInt(GameConstants.PREF_KEY_FPS, _selectedFPSIndex);
        }

        
        private void ApplyResolution()
        {
            Resolution selected = _resolutions[_selectedResolutionIndex];
            Screen.SetResolution(
            selected.width,
            selected.height,
            Screen.fullScreenMode
            );
            PlayerPrefs.SetInt(GameConstants.PREF_KEY_RESOLUTION, _selectedResolutionIndex);
        }
    }
}