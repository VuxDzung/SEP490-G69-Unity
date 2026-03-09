namespace SEP490G69.GameSessions
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.PlayerProfile;
    using SEP490G69.Tournament;
    using SEP490G69.Training;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GameSessionController : MonoBehaviour, ISceneContext
    {
        [SerializeField] private bool m_DeleteAll = true;

        private StarterCardConfigSO _starterCardConfig;

        private IGameSessionCreator _sessionCreator;
        private PlayerCharacterRepository _characterRepo;
        private PlayerDataDAO _playerDAO;

        private GameAuthManager _authManager;
        private PlayerProfileController _profileController;
        private CharacterConfigSO _characterConfig;
        private GameDeckController _deckController;

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
        private GameDeckController DeckController
        {
            get
            {
                if (_deckController == null)
                {
                    _deckController = ContextManager.Singleton.ResolveGameContext<GameDeckController>();
                }
                return _deckController;
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
            _starterCardConfig = ContextManager.Singleton.GetDataSO<StarterCardConfigSO>();

            _sessionCreator = new SingleSessionCreator();
            _characterRepo = new PlayerCharacterRepository(LocalDBInitiator.GetDatabase());

            CheckPlayerProfile();

            if (m_DeleteAll) DELETE_ALL();
        }

        public bool HasActiveSession()
        {
            string playerId = AuthManager.GetUserId();

            return _sessionCreator.GetAllSessions(playerId).Count > 0;
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
                PlayerCharacterDataSO characterSO = CharacterConfig.GetCharacterById(characterId).ConvertAs<PlayerCharacterDataSO>();

                if (characterSO != null)
                {
                    if (_characterRepo.TryCreateNewCharacter(sessionId, characterSO))
                    {
                        Debug.Log($"Create character session data {characterId} for session {sessionId} success");
                        Debug.Log("Start create player's deck");
                        DeckController.SetSessionId(sessionId);

                        // Dung: Add default starter cards.
                        List<string> starterCards = new List<string>();

                        foreach (StarterCardData starterCard in _starterCardConfig.StarterCards)
                        {
                            for (int i = 0; i < starterCard.amount; i++)
                            {
                                starterCards.Add(starterCard.cardId);
                            }
                            DeckController.AddObtainedCard(starterCard.cardId, starterCard.amount);
                        }

                        // Dung: Add charater's starter cards.
                        foreach (string rawCardId in characterSO.StartingCardIds)
                        {
                            starterCards.Add(rawCardId);
                            DeckController.AddObtainedCard(rawCardId);
                        }

                        // Add to deck.
                        foreach (string rawCardId in starterCards)
                        {
                            if (DeckController.GetDeckCardCount() >= GameDeckController.MAX_DECK_COUNT)
                            {
                                Debug.Log("<color=red>[GameSessionController]</color> Max card amount exceeded");
                                break;
                            }
                            if (!DeckController.AddCardToDeck(rawCardId, false))
                            {
                                Debug.Log($"<color=red>Error:</color> Failed to add card {rawCardId} to deck");
                            }
                            else
                            {
                                if (!DeckController.RemoveObtainedCard(rawCardId, 1))
                                {
                                    Debug.LogError("Failed to decrease card amount");
                                }
                            }
                        }
                        Debug.Log("<color=green>Add cards to deck successfully!</color>");
                        DeckController.SaveDeck();
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
            string playerId = AuthManager.GetUserId();

            List<PlayerTrainingSession> sessions = _sessionCreator.GetAllSessions(playerId);
            if (sessions.Count == 0)
            {
                Debug.Log("No session available!");
                return;
            }
            string sessionId = sessions[0].SessionId;

            PlayerPrefs.SetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID, sessionId);
            DeckController.SetSessionId(sessionId);
            ContextManager.Singleton.ResolveGameContext<GameInventoryManager>().SetSessionId(sessionId);
        }

        /// <summary>
        /// Delete all player's active session
        /// A session includes:
        /// - Character data.
        /// - Item data.
        /// - Card data
        /// - Tournament progression.
        /// - Session data.
        /// </summary>
        /// <returns></returns>
        public bool DeleteAllSessions()
        {
            if (_sessionCreator == null) return false;

            if (AuthManager == null) return false;

            string playerId = AuthManager.GetUserId();

            ClearAllPlayerPrefs();

            return _sessionCreator.TryDeleteAllSessions(playerId);
        }

        private void ClearAllPlayerPrefs()
        {
            PlayerPrefs.SetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID, "");
        }

        private async void CheckPlayerProfile()
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
                Debug.Log($"PlayerName: {playerName}");
                Debug.Log("==================================");

                if (playerName.Equals(playerData.PlayerName))
                {
                    return;
                }
                Debug.Log("Local player name does not match with cloud's player name. Sync now!");
                bool success = await ProfileController.SyncPlayerName(playerId, playerData.PlayerName);
                if (success)
                {
                    Debug.Log("Sync success");
                }
                else
                {
                    Debug.Log("Sync failed!");
                }
            }
        }
        private GameSessionDAO _dao;
        private PlayerCharacterDAO _characterDAO;
        private TournamentProgressDAO _tournamentDAO;
        private TrainingExerciseDAO _trainingDAO;

        private void DELETE_ALL()
        {
            string playerId = AuthManager.GetUserId();
            _dao = new GameSessionDAO(LocalDBInitiator.GetDatabase());
            _characterDAO = new PlayerCharacterDAO(LocalDBInitiator.GetDatabase());
            _tournamentDAO = new TournamentProgressDAO();
            _trainingDAO = new TrainingExerciseDAO(LocalDBInitiator.GetDatabase());
            List<PlayerTrainingSession> sessions = _dao.GetAllBydPlayerId(playerId);

            var playerSessions = sessions.Where(s => s.PlayerId == playerId).ToList();

            if (playerSessions.Count == 0)
                return;

            bool allDeleted = true;

            foreach (var session in playerSessions)
            {
                // Step 1: delete all characters.
                SessionCharacterData characterData = _characterDAO.GetCharacterById(session.SessionId, session.CharacterId);

                if (characterData != null)
                {
                    _characterDAO.TryDeleteCharacter(characterData.Id);
                }

                // Step 2: Delete all tournament progress
                if (!_tournamentDAO.DeleteAllBySessionId(session.SessionId))
                {
                    Debug.LogError("Failed to delete all progress by session. Delete all by default (Testing only)");
                    _tournamentDAO.DeleteAll();
                    allDeleted = false;
                }

                // Step 3: Delete all training exercises.
                if (!_trainingDAO.DeleteAllBySessionId(session.SessionId))
                {
                    Debug.LogError("Failed to clear all old training exercises. Delete all by default.");
                    _trainingDAO.DeleteAll();
                    allDeleted = false;
                }

                if (!_dao.DeleteById(session.SessionId))
                {
                    allDeleted = false;
                }

                if (allDeleted)
                {
                    Debug.Log("Delete all succss");
                }
                else
                {
                    Debug.Log("Delete all failed");
                }
            }
        }
    }
}