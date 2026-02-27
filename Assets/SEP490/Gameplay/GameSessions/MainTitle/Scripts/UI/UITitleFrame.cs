namespace SEP490G69.GameSessions
{
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.Shared;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITitleFrame : GameUIFrame
    {
        [SerializeField] private Button m_NewGameBtn;
        [SerializeField] private Button m_ContinueBtn;
        [SerializeField] private Button m_ProfileBtn;
        [SerializeField] private Button m_SettingsBtn;
        [SerializeField] private Button m_CreditBtn;
        [SerializeField] private Button m_QuitBtn;

        [SerializeField] private Button m_SignoutBtn;

        private GameAuthManager _authManager;
        private GameSessionController _sessionController;

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
        private GameSessionController SessionController
        {
            get
            {
                if (_sessionController == null)
                {
                    bool hasSession = ContextManager.Singleton.TryResolveSceneContext<GameSessionController>(out _sessionController); 
                }
                return _sessionController;
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
            m_ProfileBtn.onClick.AddListener(ShowPlayerProfile);
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
            m_ProfileBtn.onClick.RemoveListener(ShowPlayerProfile);
        }

        public void NewGame()
        {
            if (!SessionController.HasActiveSession())
            {
                UIManager.ShowFrame(GameConstants.FRAME_ID_CHAR_SELECT);
            }
            else
            {
                Debug.Log("Has active session");
                UIManager.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP).AsFrame<UIMessagePopup>().SetContent("title_noti", "msg_has_active_session", true, true, () => {
                    // Create new session anyway.
                    if (SessionController.DeleteAllSessions())
                    {
                        UIManager.ShowFrame(GameConstants.FRAME_ID_CHAR_SELECT);
                    }
                    else
                    {
                        Debug.LogError($"Failed to delete session of player {AuthManager.GetUserId()}");
                    }
                }, () => { 
                    // Nothing happen.
                });
            }
        }

        public void Continue()
        {
            try
            {
                SessionController.ContinueSession();
                SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_MAIN_MENU);
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
            }
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

        private void ShowPlayerProfile()
        {
            UIManager.ShowFrame(GameConstants.FRAME_ID_PLAYER_PROFILE);
        }
    }
}