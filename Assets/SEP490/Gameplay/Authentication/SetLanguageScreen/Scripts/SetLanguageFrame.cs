namespace SEP490G69.Shared
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using SEP490G69.Addons.Localization.Enums;
    using System.Linq;
    using SEP490G69.Addons.Localization;
    using System;

    public class SetLanguageFrame : GameUIFrame
    {
        [SerializeField] private Button m_Confirm;
        [SerializeField] private UILinearSwitcher m_LanguageSwitcher;

        private LocalizationManager localizationManager;

        private List<ELocalizeLanguageType> _languages = new List<ELocalizeLanguageType>
        {
            ELocalizeLanguageType.English,
            ELocalizeLanguageType.Vietnamese,
        };

        private int _selectedIndex = 0;

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
            m_LanguageSwitcher.Enable();
            m_Confirm.onClick.AddListener(ConfirmLanguage);
            m_LanguageSwitcher.OnChanged += OnLanguageIndexChanged;
            LoadExistedLanguage();
            LoadLanguages();
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_LanguageSwitcher.Disable();
            m_LanguageSwitcher.OnChanged -= OnLanguageIndexChanged;
            m_Confirm.onClick.RemoveListener(ConfirmLanguage);
        }

        private void LoadLanguages()
        {
            m_LanguageSwitcher.SetContents(_languages.Select(e => e.ToString()).ToArray(), _selectedIndex);
        }
        private void LoadExistedLanguage()
        {
            string langStr = PlayerPrefs.GetString("Language");
            if (!string.IsNullOrEmpty(langStr))
            {
                ELocalizeLanguageType lang = Enum.Parse<ELocalizeLanguageType>(langStr);
                LocalizationManager.SetLanguage(lang);
                UIManager.ShowFrame(GameConstants.FRAME_ID_LOGIN);
            }
        }

        private void OnLanguageIndexChanged(int index)
        {
            Debug.Log($"Index: {index}");
            _selectedIndex = index;
        }

        public void ConfirmLanguage()
        {
            ELocalizeLanguageType lang = _languages[_selectedIndex];
            LocalizationManager.SetLanguage(lang);
            // Save language index to data here.
            PlayerPrefs.SetString("Language", _languages[_selectedIndex].ToString());

            UIManager.HideFrame(FrameId);
            UIManager.ShowFrame(GameConstants.FRAME_ID_LOGIN);
        }
    }
}