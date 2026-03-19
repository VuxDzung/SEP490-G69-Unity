namespace SEP490G69
{
    using UnityEngine;

    public class StartNewGameHandler : NarrativeActionHandlerBase
    {
        private GameAuthManager _authManager;
        protected GameAuthManager AuthManager
        {
            get
            {
                if (_authManager == null)
                {
                    _authManager = _contextManager.ResolveGameContext<GameAuthManager>();
                }
                return _authManager;
            }
        }

        private readonly PlayerDataDAO _playerDAO;

        public StartNewGameHandler(ContextManager contextManager) : base(contextManager)
        {
            _playerDAO = new PlayerDataDAO();
        }

        public override string ActionId => GameConstants.ACTION_START_NEW_GAME;

        public override void Execute(DialogEvent ev)
        {
            string playerId = AuthManager.GetUserId();

            PlayerData playerData = _playerDAO.GetById(playerId); // Error here

            if (playerData == null)
            {
                Debug.LogError($"[UITitleFrame.StartNew error] Player data of player {playerId}");
                return;
            }

            if (playerData.LegacyPoints > 0.5f)
            {
                GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_LEGACY_UPGRADE);
            }
            else
            {
                GameUIManager.Singleton.ShowFrame(GameConstants.FRAME_ID_CHAR_SELECT);
            }
        }
    }
}