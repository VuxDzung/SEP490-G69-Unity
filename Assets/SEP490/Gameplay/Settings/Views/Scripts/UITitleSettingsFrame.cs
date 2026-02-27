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
        private LocalizationManager localizationManager;
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

            m_LanguageSwitcher.OnChanged += OnLanguageIndexChanged;
            LoadExistedLanguage();
            LoadLanguages();
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
        }

        private void Apply()
        {
            ConfirmLanguage();
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

        public void ConfirmLanguage()
        {
            ELocalizeLanguageType lang = GameConstants.LANGUAGES[_selectedLangIndex];
            LocalizationManager.SetLanguage(lang);
            // Save language index to data here.
            PlayerPrefs.SetString("Language", GameConstants.LANGUAGES[_selectedLangIndex].ToString());

            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_LOGIN);
        }
    }
}