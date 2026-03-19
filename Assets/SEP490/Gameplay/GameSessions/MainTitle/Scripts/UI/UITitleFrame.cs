namespace SEP490G69.GameSessions
{
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.PlayerProfile;
    using SEP490G69.Shared;
    using System;
    using TMPro;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITitleFrame : GameUIFrame
    {
        [SerializeField] private TextMeshProUGUI m_WelcomeTmp;
        [SerializeField] private Button m_NewGameBtn;
        [SerializeField] private Button m_ContinueBtn;
        [SerializeField] private Button m_ProfileBtn;
        [SerializeField] private Button m_SettingsBtn;
        [SerializeField] private Button m_CreditBtn;
        [SerializeField] private Button m_QuitBtn;

        [SerializeField] private Button m_SignoutBtn;

        [SerializeField] private float m_FadeDuration = 1.5f;
        [SerializeField] private float m_DelayFadeOutDur = 2f;
        [SerializeField] private float m_ZoomInCamOrthSize = 1f;

        private GameAuthManager _authManager;
        private GameSessionController _sessionController;
        private PlayerDataDAO _playerDAO = new PlayerDataDAO();

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
        private PlayerProfileController _profileController;
        protected PlayerProfileController ProfileController
        {
            get
            {
                if (_profileController == null)
                {
                    _profileController = ContextManager.Singleton.ResolveGameContext<PlayerProfileController>();
                }
                return _profileController;
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

            ShowWelcomeMsg();
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
                StartNew();
            }
            else
            {
                Debug.Log("Has active session");
                UIManager.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP).AsFrame<UIMessagePopup>().SetContent("title_noti", "msg_has_active_session", true, true, () =>
                {
                    // Create new session anyway.
                    if (SessionController.DeleteAllSessions())
                    {
                        StartNew();
                    }
                    else
                    {
                        Debug.LogError($"Failed to delete session of player {AuthManager.GetUserId()}");
                    }
                }, () =>
                {
                    // Nothing happen.
                });
            }
        }

        public void Continue()
        {
            try
            {
                if (SessionController.HasActiveSession())
                {
                    PerformCinematic(() =>
                    {
                        SessionController.ContinueSession();
                        SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_MAIN_MENU);
                    });
                }
            }
            catch (System.Exception e)
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
#else
            Application.Quit();
#endif
        }

        private void SignOut()
        {
            AuthManager.Logout();
        }

        private void ShowPlayerProfile()
        {
            UIManager.ShowFrame(GameConstants.FRAME_ID_PLAYER_PROFILE);
        }

        private void StartNew()
        {
            CinematicCameraController.Instance.StartZoomIn(m_ZoomInCamOrthSize, m_FadeDuration);
            PerformCinematic(() =>
            {
                UIManager.HideFrame(FrameId);

                string playerId = AuthManager.GetUserId();

                PlayerData playerData = _playerDAO.GetById(playerId); // Error here

                if (playerData == null)
                {
                    Debug.LogError($"[UITitleFrame.StartNew error] Player data of player {playerId}");
                    return;
                }

                if (playerData.LegacyPoints > 0.5f)
                {
                    UIManager.ShowFrame(GameConstants.FRAME_ID_LEGACY_UPGRADE);
                }
                else
                {
                    UIManager.ShowFrame(GameConstants.FRAME_ID_CHAR_SELECT);
                }

                CinematicCameraController.Instance.SetOrthSize(GameConstants.DEFAULT_CAM_ORTH_SIZE);
            });
        }

        private void PerformCinematic(Action onAction)
        {
            CinematicCameraController.Instance.StartZoomIn(m_ZoomInCamOrthSize, m_FadeDuration - 0.1f);
            FadingController.Singleton.FadeIn2Out(m_FadeDuration, m_DelayFadeOutDur, Color.black, "", () =>
            {
                onAction?.Invoke();
            });
        }

        private void ShowWelcomeMsg()
        {
            string authAction = PlayerPrefs.GetString(GameConstants.PREF_KEY_AUTH_ACTION);
            if (string.IsNullOrEmpty(authAction))
            {
                return;
            }

            string welcomeMsg = "";

            switch (authAction)
            {
                case "Login":
                    welcomeMsg = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_UI_MESSAGE, "msg_welcome_back");
                    break;
                case "SignUp":
                    welcomeMsg = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_UI_MESSAGE, "msg_welcome");
                    break;

            }
            string playerId = PlayerPrefs.GetString(GameConstants.PREF_KEY_PLAYER_ID);

            if (string.IsNullOrEmpty(playerId))
            {
                return;
            }

            string playerName = ProfileController.GetPlayerName(playerId);

            welcomeMsg = welcomeMsg + " " + playerName;

            m_WelcomeTmp.text = welcomeMsg;
        }
    }
}