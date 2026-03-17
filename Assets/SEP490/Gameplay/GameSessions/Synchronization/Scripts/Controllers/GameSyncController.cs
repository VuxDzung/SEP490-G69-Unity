namespace SEP490G69.GameSessions
{
    using SEP490G69.Addons.Networking;
    using SEP490G69.Economy;
    using SEP490G69.Tournament;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System;
    using UnityEngine;
    using SEP490G69.Battle.Cards;
    using SEP490G69.Training;
    using Newtonsoft.Json;
    using LiteDB;

    public class GameSyncController : MonoBehaviour, IGameContext
    {
        private PlayerDataDAO _playerDAO;
        private GameSessionDAO _sessionDAO;
        private PlayerCharacterRepository _characterRepo;
        private TrainingExerciseDAO _exercisesDAO;
        private GameCardsDAO _cardsDAO;
        private GameDeckDAO _deckDAO;
        private GameInventoryDAO _inventoryDAO;
        private GameShopDAO _shopDAO;
        private TournamentProgressDAO _tournamentDAO;

        private WebRequests _webRequests;
        private GameAuthManager _authManager;

        private ContextManager _contextManager;

        private GameAuthManager AuthManager
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

        private void Start()
        {
            LoadDAOs();
        }

        public void SetManager(ContextManager manager)
        {
            _contextManager = manager;

            _webRequests = _contextManager.ResolveGameContext<WebRequests>();
        }

        private void LoadDAOs()
        {
            _playerDAO = new PlayerDataDAO();
            _sessionDAO = new GameSessionDAO();
            _characterRepo = new PlayerCharacterRepository();
            _exercisesDAO = new TrainingExerciseDAO();
            _cardsDAO = new GameCardsDAO();
            _deckDAO = new GameDeckDAO();
            _inventoryDAO = new GameInventoryDAO();
            _shopDAO = new GameShopDAO();
            _tournamentDAO = new TournamentProgressDAO();
        }

        public async Task SyncPlayerData()
        {
            string playerId = AuthManager.GetUserId();
            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);

            ESyncState state = await DecideSyncDirection(playerId, sessionId);

            switch (state)
            {
                case ESyncState.PullingFromCloud:
                    Debug.Log("<color=green>[GameSyncController]</color> Insert from cloud to local");
                    //OverrideLocalData(playerId, sessionId);
                    break;
                case ESyncState.PushingToCloud:
                    Debug.Log("<color=green>[GameSyncController]</color> Insert from local to cloud");
                    //OverrideCloudData(playerId, sessionId);
                    break;
                case ESyncState.Error:
                    break;
                default:
                    Debug.LogError($"Unsupported ESyncStat: {state.ToString()}");
                    break;
            }
        }

        private async Task<ESyncState> DecideSyncDirection(string playerId, string sessionId)
        {
            if (!WebRequests.HasInternetConnection())
            {
                Debug.Log($"<color=red>[GameSessionController]</color> No internet connection is available!");
                return ESyncState.CheckingConnection;
            }

            if (string.IsNullOrEmpty(playerId))
            {
                Debug.LogError("[GameSessionController error] Player id is null/empty");
                return ESyncState.Error;
            }

            Debug.Log($"[GameSessionController] PlayerId: {playerId}");

            if (string.IsNullOrEmpty(sessionId))
            {
                Debug.LogError("[GameSessionController error] Session id is null/empty");
                return ESyncState.Error;
            }

            Debug.Log($"[GameSessionController] SessionId: {sessionId}");

            // Step 2: send a GET request to game backend to get the latest game progression data.

            string queryParams = $"playerId={playerId}&sessionId={sessionId}";

            // Step 1:
            // Request the cloud metadata which contains the latest progression info
            // such as the latest run index and last sync timestamp.
            PlayerMetadataResponse cloud = null;

            await _webRequests.GetEndpointByParam("GetPlayerMetadata", queryParams, (responsePackage) =>
            {
                // Deserialize the response packet here.
                if (responsePackage.Result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    cloud = JsonConvert.DeserializeObject<PlayerMetadataResponse>(responsePackage.Json);
                }
            });

            // If the request failed or backend returns invalid response,
            // we skip the sync to avoid corrupting the local save.
            if (cloud == null || !cloud.Success)
            {
                return ESyncState.Error;
            }
            Debug.Log($"<color=green>[GameSyncController]</color> Metadata result: {cloud.MetadataResult}");
            if (cloud.MetadataResult == (int)EMetadataResult.NoProfile)
            {
                Debug.Log("<color=red>[GameSyncController]</color> No account existed. Need to register!");
                return ESyncState.Error;
            }

            if (cloud.MetadataResult == (int)EMetadataResult.HasProfileNoSession)
            {
                Debug.Log("<color=green>[GameSyncController]</color> Insert to cloud");
                return ESyncState.PushingToCloud;
            }

            PlayerData localData = _playerDAO.GetById(playerId);

            if (localData == null)
            {
                Debug.LogError($"[GameSessionController error] Player local data of player {playerId} does not existed");
                return ESyncState.Error;
            }

            int localRun = localData.CurrentRun;     // Latest run index stored locally
            int cloudRun = cloud.CurrentRun;       // Latest run index stored on backend

            DateTime localUpdated = localData.LastUpdatedTime;
            DateTime cloudLastSync = cloud.LastSyncTime;

            // ---------------------------------------------------------
            // CASE 1: Cloud run index is greater than local run index
            // ---------------------------------------------------------
            //
            // Meaning:
            // The player has progressed further on another device.
            // Example:
            // CloudRun = 7
            // LocalRun = 6
            //
            // This means the player started or finished a newer run on another device.
            // Therefore the local data is outdated.
            //
            // Decision:
            // Download the cloud save and overwrite local data.
            //
            if (cloudRun > localRun)
            {
                return ESyncState.PullingFromCloud;
            }

            // ---------------------------------------------------------
            // CASE 2: Local run index is greater than cloud run index
            // ---------------------------------------------------------
            //
            // Meaning:
            // The player progressed locally while being offline
            // and the cloud has not received the latest run yet.
            //
            // Example:
            // CloudRun = 7
            // LocalRun = 8
            //
            // Decision:
            // Upload the local save to the cloud to update backend data.
            //
            if (localRun > cloudRun)
            {
                return ESyncState.PushingToCloud;
            }

            // ---------------------------------------------------------
            // CASE 3: Both cloud and local are on the same run
            // ---------------------------------------------------------
            //
            // Meaning:
            // Both devices refer to the same run index.
            // Now we must determine which side has the latest progress update.
            //
            if (localRun == cloudRun)
            {
                // -----------------------------------------------------
                // CASE 3.1: Local data updated after the last cloud sync
                // -----------------------------------------------------
                //
                // Meaning:
                // The local client has made progress after the last time
                // the save was synced with the backend.
                //
                // Example:
                // LocalLastUpdated = 12:30
                // CloudLastSync   = 12:10
                //
                // Decision:
                // Upload the local data because it contains newer progress.
                //
                if (localUpdated > cloudLastSync)
                {
                    return ESyncState.PushingToCloud;
                }

                // -----------------------------------------------------
                // CASE 3.2: Cloud data is equal or newer
                // -----------------------------------------------------
                //
                // Meaning:
                // The cloud save already contains the same or newer progress.
                // The local client may be outdated or unchanged.
                //
                // Decision:
                // Download the cloud save and overwrite local data
                // to ensure consistency.
                //
                return ESyncState.PullingFromCloud;
            }

            // ---------------------------------------------------------
            // Fallback safety
            // ---------------------------------------------------------
            //
            // This should theoretically never happen,
            // but we keep this branch as a safety net.
            //
            return ESyncState.Error;
        }

        /// <summary>
        /// Pull cloud data to local.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="sessionId"></param>
        private async void OverrideLocalData(string playerId, string sessionId)
        {
            Debug.Log("<color=green>[GameSessionController.OverrideLocalData]</color> Start override local data, prepare to send request to backend.");
            // Step 1: Get all the data on the cloud.
            string queryParams = $"playerId={playerId}&sessionId={sessionId}";
            GetPlayerGameDataResponse cloudData = null;
            await _webRequests.GetEndpointByParam("GetPlayerGameData", queryParams, (responsePacket) =>
            {
                if (responsePacket.Result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    cloudData = JsonConvert.DeserializeObject<GetPlayerGameDataResponse>(responsePacket.Json);
                }
            });

            if (cloudData == null)
            {
                Debug.Log("<color=red>[GameSessionController.OverrideLocalData error]</color> Error occurs. No response received.");
                return;
            }

            // Step 2: Override the cloud data to local data.

            // Step 3: Navigate back to title screen.
        }

        /// <summary>
        /// Push local data to cloud.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="sessionId"></param>
        private void OverrideCloudData(string playerId, string sessionId)
        {
            // Step 1: Get all the local data.

            PlayerData playerData = _playerDAO.GetById(playerId);
            PlayerTrainingSession sessionData = _sessionDAO.GetById(sessionId);
            SessionCharacterData characterData = _characterRepo.GetCharacterData(sessionData.SessionId, sessionData.RawCharacterId);
            List<SessionTrainingExercise> exercises = _exercisesDAO.GetAllBySessionId(sessionId);
            List<SessionCardData> cards = _cardsDAO.GetAllBySessionId(sessionId);
            SessionPlayerDeck deck = _deckDAO.GetById(sessionId);
            List<ItemData> obtainedItems = _inventoryDAO.GetAllItems(sessionId);
            List<ShopItemData> shopItems = _shopDAO.GetAll(sessionId);
            List<TournamentProgressData> tournaments = _tournamentDAO.GetAllBySessionId(sessionId);

            // Step 2: Send an UPDATE request to the server to override the data on cloud.


            // Step 3: Handle the backend response
            // + Step 3.1: If the override proccess is successful -> display success notification.
            // + Step 3.2: If the override proccess is failure -> display failed notification.


        }
    }
}