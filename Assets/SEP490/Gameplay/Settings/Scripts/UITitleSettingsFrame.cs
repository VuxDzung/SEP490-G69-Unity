namespace SEP490G69.GameSessions
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UITitleSettingsFrame : GameUIFrame
    {
        [SerializeField] private Button m_ApplyBtn;
        [SerializeField] private Button m_BackBtn;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_ApplyBtn.onClick.AddListener(Apply);
            m_BackBtn.onClick.AddListener(Back);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
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