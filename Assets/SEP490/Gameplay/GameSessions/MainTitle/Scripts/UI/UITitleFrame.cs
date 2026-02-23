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

        [SerializeField] private Button m_SignoutBtn;

        private GameAuthManager _authManager;

        private GameAuthManager AuthManager
        {
            get
            {
                if (_authManager == null)
                {
                    _authManager = ContextManager.Singleton.ResolveGameContext<GameAuthManager>();
                }
                return _authManager;
            }
        }

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_NewGameBtn.onClick.AddListener(NewGame);
            m_ContinueBtn.onClick.AddListener(Continue);
            m_SettingsBtn.onClick.AddListener(ViewSettings);
            m_CreditBtn.onClick.AddListener(ViewCredit);
            m_QuitBtn.onClick.AddListener(QuitGame);
            m_SignoutBtn.onClick.AddListener(SignOut);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_NewGameBtn.onClick.RemoveListener(NewGame);
            m_ContinueBtn.onClick.RemoveListener(Continue);
            m_SettingsBtn.onClick.RemoveListener(ViewSettings);
            m_CreditBtn.onClick.RemoveListener(ViewCredit);
            m_QuitBtn.onClick.RemoveListener(QuitGame);
            m_SignoutBtn.onClick.RemoveListener(SignOut);
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

        private void SignOut()
        {
            AuthManager.Logout();
        }
    }
}