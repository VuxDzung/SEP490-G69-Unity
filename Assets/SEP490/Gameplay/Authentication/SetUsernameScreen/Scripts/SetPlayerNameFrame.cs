namespace SEP490G69.Shared
{
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.GameSessions;
    using SEP490G69.PlayerProfile;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SetPlayerNameFrame : GameUIFrame
    {
        [SerializeField] private TMP_InputField m_PlayerNameInput;
        [SerializeField] private Button m_NextBtn;

        private PlayerProfileController _profileController;
        private GameAuthManager _authManager;

        private PlayerProfileController ProfileController
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
        private GameAuthManager AuthManager
        {
            get
            {
                if ( _authManager == null)
                {
                    _authManager = ContextManager.Singleton.ResolveGameContext<GameAuthManager>();
                }
                return _authManager;
            }
        }

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_NextBtn.onClick.AddListener(Next);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_NextBtn.onClick.RemoveListener(Next);
        }

        public void Next()
        {
            string playerName = m_PlayerNameInput.text;

            if (string.IsNullOrEmpty(playerName)) return;

            string playerId = AuthManager.GetUserId();

            if (string.IsNullOrEmpty(playerId))
            {
                return;
            }

            ProfileController.UpdatePlayerName(playerId, playerName);

            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_TITLE);
        }
    }
}