namespace SEP490G69.GameSessions
{
    using SEP490G69.PlayerProfile;
    using System.Collections.Generic;
    using UnityEngine;

    public class GameSessionController : MonoBehaviour, ISceneContext
    {
        private IGameSessionCreator _sessionCreator;
        private PlayerCharacterRepository _characterRepo;
        private PlayerDataDAO _playerDAO;

        private GameAuthManager _authManager;
        private PlayerProfileController _profileController;

        private CharacterConfigSO _characterConfig;

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
        private CharacterConfigSO CharacterConfig
        {
            get
            {
                if (_characterConfig == null)
                {
                    _characterConfig = ContextManager.Singleton.GetDataSO<CharacterConfigSO>();
                }
                return _characterConfig;
            }
        }
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

        private void OnEnable()
        {
            ContextManager.Singleton.AddSceneContext(this);
        }
        private void OnDisable()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
        }

        private void Start()
        {
            _sessionCreator = new SingleSessionCreator();
            _characterRepo = new PlayerCharacterRepository(LocalDBInitiator.GetDatabase());

            TestPlayerProfile();
        }

        public bool HasActiveSession()
        {
            return _sessionCreator.GetAllSessions().Count > 0;
        }

        public bool CreateNewSession(string characterId, out string sessionId, out string error)
        {
            error = "";
            sessionId = "";
            if (_sessionCreator == null) return false;

            if (AuthManager == null) return false;

            if (string.IsNullOrEmpty(characterId))
            {
                error = "Empty character id";
                return false;
            }

            string playerId = AuthManager.GetUserId();

            if (_sessionCreator.TryCreateSession(playerId, characterId, out sessionId, out error))
            {
                BaseCharacterSO characterSO = CharacterConfig.GetCharacter(characterId);
                if (characterSO != null)
                {
                    if (_characterRepo.TryCreateNewCharacter(sessionId, characterSO))
                    {
                        Debug.Log($"Create character session data {characterId} for session {sessionId} success");
                        return true;
                    }
                    else
                    {
                        Debug.Log($"Failed to create character session data {characterId} for session {sessionId} success");
                        return false;
                    }
                }
                else
                {
                    Debug.LogError($"No SO data of character with id {characterId}");
                    return false;
                }
            }

            return false;
        }

        public void ContinueSession()
        {
            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
            if (string.IsNullOrEmpty(sessionId))
            {
                List<PlayerTrainingSession> sessions = _sessionCreator.GetAllSessions();
                if (sessions.Count == 0) return;
                sessionId = sessions[0].SessionId;

                PlayerPrefs.SetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID, sessionId);
            }
        }

        public bool DeleteAllSessions()
        {
            if (_sessionCreator == null) return false;

            if (AuthManager == null) return false;

            string playerId = AuthManager.GetUserId();


            return _sessionCreator.TryDeleteAllSessions(playerId);
        }

        /// <summary>
        /// Dung: Delete later.
        /// </summary>
        private async void TestPlayerProfile()
        {
            _playerDAO = new PlayerDataDAO(LocalDBInitiator.GetDatabase());

            if (_playerDAO != null)
            {
                string playerId = PlayerPrefs.GetString(GameConstants.PREF_KEY_PLAYER_ID);
                PlayerData playerData = _playerDAO.GetPlayerById(playerId);

                if (playerData == null)
                {
                    Debug.LogError($"Failed to get player data with id {playerId}");
                    return;
                }
                Debug.Log("==========PLAYER_PROFILE==========");
                Debug.Log($"PlayerId: {playerData.PlayerId}");
                Debug.Log($"PlayerName: {playerData.PlayerName}");
                Debug.Log("==================================");

                Debug.Log("==========PLAYER_CLOUD_PROFILE==========");
                Debug.Log($"PlayerId: {playerData.PlayerId}");
                string playerName = await ProfileController.GetCloudPlayerName(playerId);
                Debug.Log($"PlayerName: {playerData.PlayerName}");
                Debug.Log("==================================");
            }
        }
    }
}