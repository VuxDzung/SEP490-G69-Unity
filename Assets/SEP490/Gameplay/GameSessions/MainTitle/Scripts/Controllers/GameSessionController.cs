namespace SEP490G69.GameSessions
{
    using SEP490G69.Addons.Networking;
    using SEP490G69.Battle.Cards;
    using SEP490G69.Economy;
    using SEP490G69.Legacy;
    using SEP490G69.PlayerProfile;
    using SEP490G69.Tournament;
    using SEP490G69.Training;
    using System.Collections.Generic;
    using UnityEngine;

    public class GameSessionController : MonoBehaviour, ISceneContext
    {
        private StarterCardConfigSO _starterCardConfig;

        private IGameSessionCreator _sessionCreator;

        private WebRequests _webRequests;

        private PlayerDataDAO _playerDAO;
        private GameSessionDAO _sessionDAO;
        private PlayerCharacterRepository _characterRepo;
        private TrainingExerciseDAO _exercisesDAO;
        private GameCardsDAO _cardsDAO;
        private GameDeckDAO _deckDAO;
        private GameInventoryDAO _inventoryDAO;
        private GameShopDAO _shopDAO;
        private TournamentProgressDAO _tournamentDAO;
        private GameLegacyDAO _legacyDAO;

        private GameAuthManager _authManager;
        private PlayerProfileController _profileController;
        private CharacterConfigSO _characterConfig;
        private GameDeckController _deckController;
        private GameLegacyController _legacyController;

        #region Lazy properties
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
        private WebRequests WebRequests
        {
            get
            {
                if (_webRequests == null)
                {
                    _webRequests = ContextManager.Singleton.ResolveGameContext<WebRequests>();
                }
                return _webRequests;
            }
        }

        private GameLegacyController LegacyController
        {
            get
            {
                if (_legacyController == null)
                {
                    _legacyController = ContextManager.Singleton.ResolveGameContext<GameLegacyController>();
                }
                return _legacyController;
            }
        }
        #endregion

        #region Unity methods
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

            LoadDAOs();
            //CheckPlayerProfile();
            InitLegacies();
        }

        #endregion

        private void LoadDAOs()
        {
            _sessionCreator = new SingleSessionCreator();
            _characterRepo = new PlayerCharacterRepository();
            _playerDAO = new PlayerDataDAO();
            _cardsDAO = new GameCardsDAO();
            _deckDAO = new GameDeckDAO();
            _inventoryDAO = new GameInventoryDAO();
            _shopDAO = new GameShopDAO();
            _tournamentDAO = new TournamentProgressDAO();
            _exercisesDAO = new TrainingExerciseDAO();
            _legacyDAO = new GameLegacyDAO();
        }

        public bool HasActiveSession()
        {
            string playerId = AuthManager.GetUserId();

            return _sessionCreator.GetAllSessions(playerId).Count > 0;
        }

        private void InitLegacies()
        {
            string playerId = AuthManager.GetUserId();
            if (string.IsNullOrEmpty(playerId))
            {
                Debug.LogError($"[GameSessionController.InitLegacies] Player id is null/empty");
                return;
            }
            LegacyController.Initialize(playerId);
        }

        /// <summary>
        /// Create a brand new session.
        /// A session consists of:
        /// - Session data
        /// - Session character data.
        /// - Player starter cards & starter deck
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="sessionId"></param>
        /// <param name="error"></param>
        /// <returns></returns>
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

            if (string.IsNullOrEmpty(playerId))
            {
                Debug.LogError($"[GameSessionController.CreateNewSession fatal error] Player id is null/empty!");
                error = $"Player id is null/empty!";
                return false;
            }

            if (_sessionCreator.TryCreateSession(playerId, characterId, out sessionId, out error))
            {
                PlayerCharacterDataSO characterSO = CharacterConfig.GetCharacterById(characterId).ConvertAs<PlayerCharacterDataSO>();

                if (characterSO != null)
                {
                    BonusStarterStats bonusStarterStats = new BonusStarterStats();

                    LegacyStatDataHolder vitHolder = LegacyController.GetByType(playerId, EStatusType.Vitality);
                    bonusStarterStats.BonusVit = vitHolder != null ? vitHolder.GetCurrentValue() : 0;

                    LegacyStatDataHolder powHolder = LegacyController.GetByType(playerId, EStatusType.Power);
                    bonusStarterStats.BonusPow = powHolder != null ? powHolder.GetCurrentValue() : 0;

                    LegacyStatDataHolder intHolder = LegacyController.GetByType(playerId, EStatusType.Intelligence);
                    bonusStarterStats.BonusInt = intHolder != null ? intHolder.GetCurrentValue() : 0;

                    LegacyStatDataHolder staHolder = LegacyController.GetByType(playerId, EStatusType.Stamina);
                    bonusStarterStats.BonusSta = staHolder != null ? staHolder.GetCurrentValue() : 0;

                    LegacyStatDataHolder agiHolder = LegacyController.GetByType(playerId, EStatusType.Agi);
                    bonusStarterStats.BonusAgi = agiHolder != null ? agiHolder.GetCurrentValue() : 0;

                    if (_characterRepo.TryCreateNewCharacter(sessionId, characterSO, bonusStarterStats))
                    {
                        Debug.Log($"Create character session data {characterId} for session {sessionId} success");
                        Debug.Log("Start create player's deck");

                        DeckController.SetSessionId(sessionId);

                        // Dung: Add default starter cards (Available for all characters).
                        List<string> starterCards = new List<string>();
                        Dictionary<string, int> starterCardsLookup = new Dictionary<string, int>();

                        foreach (StarterCardData starterCard in _starterCardConfig.StarterCards)
                        {
                            for (int i = 0; i < starterCard.amount; i++)
                            {
                                starterCards.Add(starterCard.cardId);
                            }
                            starterCardsLookup.Add(starterCard.cardId, starterCard.amount);
                        }

                        DeckController.AddManyCards(starterCardsLookup);

                        // Dung: Add charater's starter cards (Unique cards of the character).
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
                                Debug.Log("<color=red>[GameSessionController Error]</color> Max card amount exceeded");
                                break;
                            }
                            if (!DeckController.AddCardToDeck(rawCardId, false))
                            {
                                Debug.Log($"<color=red>[GameSessionController Error]</color> Failed to add card {rawCardId} to deck");
                            }
                            else
                            {
                                if (!DeckController.RemoveObtainedCard(rawCardId, 1))
                                {
                                    Debug.LogError("Failed to decrease card amount");
                                }
                            }
                        }
                        Debug.Log("<color=green>[GameSessionController]</color> Add cards to deck successfully!");
                        DeckController.SaveDeck();

                        PlayerPrefs.SetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID, sessionId);

                        LocalDBOrchestrator.UpdateDBChangeTime();

                        return true;
                    }
                    else
                    {
                        Debug.Log($"<color=red>[GameSessionController]</color> Failed to create character session data {characterId} for session {sessionId} success");
                        return false;
                    }
                }
                else
                {
                    Debug.LogError($"[GameSessionController] No SO data of character with id {characterId}");
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
                Debug.Log("<color=yellow>[GameSessionController]</color> No session available!");
                return;
            }
            string sessionId = sessions[0].SessionId;
            Debug.Log($"Session id: {sessionId}");
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

        public async void CheckPlayerProfile()
        {
            _playerDAO = new PlayerDataDAO();

            if (_playerDAO != null)
            {
                string playerId = PlayerPrefs.GetString(GameConstants.PREF_KEY_PLAYER_ID);
                PlayerData playerData = _playerDAO.GetById(playerId);

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
                //bool success = await ProfileController.SyncPlayerName(playerId, playerData.PlayerName);
                //if (success)
                //{
                //    Debug.Log("Sync success");
                //}
                //else
                //{
                //    Debug.Log("Sync failed!");
                //}
            }
        }
    }
}