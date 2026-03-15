namespace SEP490G69.GameSessions
{
    using SEP490G69.Addons.Networking;
    using SEP490G69.Battle.Cards;
    using SEP490G69.Economy;
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
            CheckPlayerProfile();
        }

        private void LoadDAOs()
        {
            _sessionCreator = new SingleSessionCreator();
            _characterRepo = new PlayerCharacterRepository(LocalDBInitiator.GetDatabase());
            _playerDAO = new PlayerDataDAO(LocalDBInitiator.GetDatabase());
            _cardsDAO = new GameCardsDAO(LocalDBInitiator.GetDatabase());
            _deckDAO = new GameDeckDAO(LocalDBInitiator.GetDatabase());
            _inventoryDAO = new GameInventoryDAO();
            _shopDAO = new GameShopDAO();
            _tournamentDAO = new TournamentProgressDAO();
            _exercisesDAO = new TrainingExerciseDAO(LocalDBInitiator.GetDatabase());
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

        public async void SyncDataToCloud()
        {
            if (!WebRequests.HasInternetConnection())
            {
                Debug.Log($"<color=red>[GameSessionController]</color> No internet connection is available!");
                return;
            }

            // Step 1: get all local data.
            string playerId = AuthManager.GetUserId();
            if (string.IsNullOrEmpty(playerId))
            {
                Debug.LogError("[GameSessionController error] Player id is null/empty");
                return;
            }
            Debug.Log($"[GameSessionController] PlayerId: {playerId}");

            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);

            if (string.IsNullOrEmpty(sessionId))
            {
                return;
            }

            Debug.Log($"[GameSessionController] SessionId: {sessionId}");

            PlayerData playerData = _playerDAO.GetById(playerId);
            PlayerTrainingSession sessionData = _sessionDAO.GetById(sessionId);
            SessionCharacterData characterData = _characterRepo.GetCharacterData(sessionData.SessionId, sessionData.CharacterId);
            List<SessionTrainingExercise> exercises = _exercisesDAO.GetAllBySessionId(sessionId);
            List<SessionCardData> cards = _cardsDAO.GetAllBySessionId(sessionId);
            SessionPlayerDeck deck = _deckDAO.GetById(sessionId);
            List<ItemData> obtainedItems = _inventoryDAO.GetAllItems(sessionId);
            List<ShopItemData> shopItems = _shopDAO.GetAll(sessionId);
            List<TournamentProgressData> tournaments = _tournamentDAO.GetAllBySessionId(sessionId);

            // Step 2: send a GET request to game backend to get the latest game progression data.

            string queryParams = $"playerId={playerId},sessionId={sessionId}";
            await WebRequests.GetEndpointByParam("GetPlayerProgression", queryParams, (responsePackage) =>
            {
                // Deserialize the response packet here.
            });

            // Step 3: check data integrity and resolve conflict.
            //
            // Terminology:
            // - CloudLastSyncTime: latest sync timestamp from backend.
            // - LocalLastSyncTime: playerData.LastSyncTime.
            // - LocalLastUpdatedTime: playerData.LastUpdatedTime.
            // - CloudRunCount: run count stored in backend.
            // - LocalRunCount: playerData.RunCount.
            //
            // Case 1: CloudLastSyncTime > LocalLastSyncTime
            // -> Cloud data is newer than the local synced state.
            //
            //    1.1 If CloudRunCount > LocalRunCount
            //        -> Player progressed further on another device.
            //        -> Override the local data with cloud data.
            //
            //    1.2 If CloudRunCount == LocalRunCount
            //        -> Same run but cloud has newer update.
            //        -> Override the local data.
            //
            //    1.3 If CloudRunCount < LocalRunCount
            //        -> Local device progressed further.
            //        -> Conflict detected.
            //        -> Resolve by overriding the cloud data with local data.
            //
            // Case 2: CloudLastSyncTime == LocalLastSyncTime
            // -> Both states were synced at the same point.
            //
            //    2.1 If LocalLastUpdatedTime > LocalLastSyncTime
            //        -> Local has new updates after last sync.
            //        -> Push local changes to cloud.
            //
            //    2.2 If LocalLastUpdatedTime <= LocalLastSyncTime
            //        -> No change.
            //        -> Skip sync.
            //
            // Case 3: CloudLastSyncTime < LocalLastSyncTime
            // -> Local device already synced more recently than backend.
            // -> Override the cloud data with local data.
            //
            // Additional validation:
            //
            // - DeviceId mismatch:
            //     If another device updated the same run simultaneously,
            //     use RunCount and LastUpdatedTime to determine priority.
            //
            // - Corrupted session:
            //     If cloud sessionId != local sessionId,
            //     treat it as a new run and upload local session.
        }
    }
}