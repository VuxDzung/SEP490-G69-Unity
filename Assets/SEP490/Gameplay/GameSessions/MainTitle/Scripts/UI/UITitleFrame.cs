namespace SEP490G69.GameSessions
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITitleFrame : GameUIFrame
    {
        [SerializeField] private Button m_NewGameBtn;
        [SerializeField] private Button m_ContinueBtn;
        [SerializeField] private Button m_SettingsBtn;
        [SerializeField] private Button m_CreditBtn;
        [SerializeField] private Button m_QuitBtn;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_NewGameBtn.onClick.AddListener(NewGame);
            m_ContinueBtn.onClick.AddListener(Continue);
            m_SettingsBtn.onClick.AddListener(ViewSettings);
            m_CreditBtn.onClick.AddListener(ViewCredit);
            m_QuitBtn.onClick.AddListener(QuitGame);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_NewGameBtn.onClick.RemoveListener(NewGame);
            m_ContinueBtn.onClick.RemoveListener(Continue);
            m_SettingsBtn.onClick.RemoveListener(ViewSettings);
            m_CreditBtn.onClick.RemoveListener(ViewCredit);
            m_QuitBtn.onClick.RemoveListener(QuitGame);
        }

        public void NewGame()
        {

        }
        public void Continue()
        {

        }
        public void ViewSettings()
        {
            UIManager.ShowFrame(GameConstants.FRAME_ID_TITLE_SETTINGS);
        }
        public void ViewCredit()
        {
            UIManager.ShowFrame(GameConstants.FRAME_ID_CREDIT);
        }
        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            return;
#endif
            Application.Quit();
        }
    }
}