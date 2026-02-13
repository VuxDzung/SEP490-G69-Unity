namespace SEP490G69.GameSessions
{
    using SEP490G69.Shared;
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
        }

        private void Apply()
        {

        }
        private void Back()
        {
            UIManager.HideFrame(GameConstants.FRAME_ID_TITLE_SETTINGS);
        }
    }
}