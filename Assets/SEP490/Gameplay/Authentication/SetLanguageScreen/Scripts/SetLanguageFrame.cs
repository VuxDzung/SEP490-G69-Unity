namespace SEP490G69.Shared
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using SEP490G69.Addons.Localization.Enums;
    using System.Linq;

    public class SetLanguageFrame : GameUIFrame
    {
        [SerializeField] private Button m_Confirm;
        [SerializeField] private UILinearSwitcher m_LanguageSwitcher;

        private List<ELocalizeLanguageType> _languages = new List<ELocalizeLanguageType>
        {
            ELocalizeLanguageType.English,
            ELocalizeLanguageType.Vietnamese,
        };

        private int _selectedIndex = 0;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_LanguageSwitcher.Enable();
            m_Confirm.onClick.AddListener(ConfirmLanguage);
            m_LanguageSwitcher.OnChanged += OnLanguageIndexChanged;
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

        private void OnLanguageIndexChanged(int index)
        {
            _selectedIndex = index;
        }

        public void ConfirmLanguage()
        {
            // Save language index to data here.
        }
    }
}