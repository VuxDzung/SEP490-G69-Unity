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
    using UnityEngine.Networking;

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

        private bool _isSyncing = false;

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
            if (_isSyncing)
            {
                return;
            }

            _isSyncing=true;

            try
            {
                string playerId = AuthManager.GetUserId();
                string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);

                ESyncState state = await DecideSyncDirection(playerId, sessionId);

                switch (state)
                {
                    case ESyncState.PullingFromCloud:
                        Debug.Log("<color=green>[GameSyncController]</color> Override local data");
                        await OverrideLocalData(playerId, sessionId);
                        break;
                    case ESyncState.PushingToCloud:
                        Debug.Log("<color=green>[GameSyncController]</color> Override cloud data.");
                        await OverrideCloudData(playerId, sessionId);
                        break;
                    case ESyncState.Error:
                        break;
                    default:
                        Debug.LogError($"Unsupported ESyncStat: {state.ToString()}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                _isSyncing = false;
            }
        }

        private async Task<ESyncState> DecideSyncDirection(string playerId, string sessionId)
        {
            if (!WebRequests.HasInternetConnection())
            {
                Debug.Log($"<color=red>[GameSyncController]</color> No internet connection is available!");
                return ESyncState.CheckingConnection;
            }

            if (string.IsNullOrEmpty(playerId))
            {
                Debug.LogError("[GameSyncController error] Player id is null/empty");
                return ESyncState.Error;
            }

            Debug.Log($"[GameSyncController] PlayerId: {playerId}");

            if (string.IsNullOrEmpty(sessionId))
            {
                Debug.LogError("[GameSyncController error] Session id is null/empty");
                return ESyncState.Error;
            }

            Debug.Log($"[GameSyncController] SessionId: {sessionId}");

            // Step 2: send a GET request to game backend to get the latest game progression data.

            string queryParams = $"playerId={playerId}&sessionId={sessionId}";

            // Step 1:
            // Request the cloud metadata which contains the latest progression info
            // such as the latest run index and last sync timestamp.
            PlayerMetadataResponse cloud = await GetPlayerMetadata(queryParams);

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
                Debug.LogError($"[GameSyncController error] Player local data of player {playerId} does not existed");
                return ESyncState.Error;
            }

            int localRun = localData.CurrentRun;     // Latest run index stored locally
            int cloudRun = cloud.CurrentRun;       // Latest run index stored on backend

            DateTime localUpdated = localData.LastUpdatedTime;
            DateTime cloudLastSync = cloud.LastSyncTime;

            if (cloudLastSync == default)
            {
                return cloudRun > 0 ? ESyncState.PullingFromCloud : ESyncState.PushingToCloud;
            }

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
            return ESyncState.Error;
        }

        private async Task<PlayerMetadataResponse> GetPlayerMetadata(string queryParams)
        {
            var tcs = new TaskCompletionSource<PlayerMetadataResponse>();

            await _webRequests.GetEndpointByParam("GetPlayerMetadata", queryParams, (response) =>
            {
                if (response.Result == UnityWebRequest.Result.Success)
                {
                    var data = JsonConvert.DeserializeObject<PlayerMetadataResponse>(response.Json);
                    tcs.SetResult(data);
                }
                else
                {
                    tcs.SetResult(null);
                }
            });

            return await tcs.Task;
        }

        /// <summary>
        /// Pull cloud data to local.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="sessionId"></param>
        private async Task OverrideLocalData(string playerId, string sessionId)
        {
            Debug.Log("<color=green>[GameSyncController.OverrideLocalData]</color> Start override local data, prepare to send request to backend.");

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
                Debug.Log("<color=red>[GameSyncController.OverrideLocalData error]</color> Error occurs. No response received.");
                return;
            }

            if (cloudData.Success == false)
            {
                Debug.Log("<color=red>[GameSyncController.OverrideLocalData error]</color> Error occurs. Failed to fetch cloud's data. See next log for more details!");
                Debug.Log($"<color=red>[GameSyncController.OverrideLocalData error details]</color> Error: {cloudData.ErrorMsg}");
                return;
            }

            if (cloudData.Session == null)
            {
                Debug.LogError("[GameSyncController.OverrideLocalData error] Cloud session is null");
                return;
            }

            if (string.IsNullOrEmpty(cloudData.Session.SessionId))
            {
                Debug.LogError("[GameSyncController.OverrideLocalData error] Cloud session id is null");
                return;
            }

            // Step 2: Override the cloud data to local data.

            // Override player data
            string playerName = cloudData.PlayerName;
            int legacyPoints = cloudData.LegacyPoints;
            int currentRun = cloudData.CurrentRun;

            PlayerData playerData = _playerDAO.GetById(playerId);
            if (playerData == null)
            {
                Debug.LogError($"[GameSyncController.OverrideLocalData error] Player data with id {playerId} is not in the local database");
                return;
            }

            playerData.PlayerName = playerName;
            playerData.LegacyPoints = legacyPoints;
            playerData.CurrentRun = currentRun;

            DateTime lastSyncTime = DateTime.UtcNow;

            playerData.LastSyncTime = lastSyncTime;

            // - SEND A PUT REQUEST TO BACKEND TO UPDATE THE LAST SYNC TIME
            UpdateLastSyncTimeRequest syncTimeRequest = new UpdateLastSyncTimeRequest();
            syncTimeRequest.PlayerId = playerId;
            syncTimeRequest.LastSyncTime = lastSyncTime;
            string syncTimeRequestJson = JsonConvert.SerializeObject(syncTimeRequest);

            BaseAPIResponse syncTimeRes = null;

            await _webRequests.PutJsonByEndpointAsync("UpdateLastSyncTime", syncTimeRequestJson, (responsePacket) =>
            {
                if (responsePacket.Result == UnityWebRequest.Result.Success)
                {
                    syncTimeRes = JsonConvert.DeserializeObject<BaseAPIResponse>(responsePacket.Json);
                }
            });

            if (syncTimeRes == null)
            {
                Debug.LogError($"[GameSyncController.OverrideLocalData error] Failed to update last sync time due to API error");
                return;
            }

            if (syncTimeRes.Success == false)
            {
                Debug.LogError($"[GameSyncController.OverrideLocalData error] Failed to update last sync time! See next log for more details");
                Debug.LogError($"[GameSyncController.OverrideLocalData error details] Error details: {syncTimeRes.ErrorMsg}");
                return;
            }

            string oldSessionId = sessionId;
            string newSessionId = cloudData.Session.SessionId;

            // Clear all current local session data and insert the current cloud data.
            bool success = LocalDBInitiator.Execute(db =>
            {
                db.BeginTrans();

                if (!_playerDAO.Update(playerData))
                {
                    db.Rollback();
                    return false;
                }

                bool clearSuccess = ClearCurrentSessionData(db, oldSessionId);
                if (!clearSuccess)
                {
                    db.Rollback();
                    return false;
                }

                bool insertSuccess = InsertAll(db, cloudData);
                if (!insertSuccess)
                {
                    db.Rollback();
                    return false;
                }

                db.Commit();
                return true;
            });

            if (!success)
            {
                Debug.LogError("[GameSyncController.OverrideLocalData error] Database transaction failed!");
                return;
            }

            // Step 3: Navigate back to title screen.
        }

        /// <summary>
        /// Push local data to cloud.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="sessionId"></param>
        private async Task OverrideCloudData(string playerId, string sessionId)
        {
            // Step 1: Get all the local data.

            PlayerData playerData = _playerDAO.GetById(playerId);
            if (playerData == null)
            {
                Debug.LogError($"[GameSyncController.OverrideCloudData error] Player data with id {playerId} is null");
                return;
            }

            PlayerTrainingSession sessionData = _sessionDAO.GetById(sessionId);

            if (sessionData == null)
            {
                Debug.LogError($"[GameSyncController.OverrideCloudData error] sessionData with id {sessionId} is null");
                return;
            }

            SessionCharacterData characterData = _characterRepo.GetCharacterData(sessionData.SessionId, sessionData.RawCharacterId);
            List<SessionTrainingExercise> exercises = _exercisesDAO.GetAllBySessionId(sessionId) ?? new List<SessionTrainingExercise>();
            List<SessionCardData> cards = _cardsDAO.GetAllBySessionId(sessionId) ?? new List<SessionCardData>();
            SessionPlayerDeck deck = _deckDAO.GetById(sessionId);
            List<ItemData> obtainedItems = _inventoryDAO.GetAllItems(sessionId) ?? new List<ItemData>();
            List<ShopItemData> shopItems = _shopDAO.GetAll(sessionId) ?? new List<ShopItemData>();
            List<TournamentProgressData> tournaments = _tournamentDAO.GetAllBySessionId(sessionId) ?? new List<TournamentProgressData>();

            OverrideCloudDataRequest request = new OverrideCloudDataRequest
            {
                PlayerData = playerData,
                Session = sessionData,
                Character = characterData,
                Exercises = exercises,
                Cards = cards,
                Deck = deck,
                ObtainedItems = obtainedItems,
                ShopItems = shopItems,
                TournamentProgressions = tournaments
            };

            // Step 2: Send an UPDATE request to the server to override the data on cloud.

            string requestJsonBody = JsonConvert.SerializeObject(request);  

            OverrideCloudDataResponse response = null;

            await _webRequests.PutJsonByEndpointAsync("OverrideCloudData", requestJsonBody, (responsePacket) =>
            {
                if (responsePacket.Result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    response = JsonConvert.DeserializeObject<OverrideCloudDataResponse>(responsePacket.Json);
                }
            });

            if (response == null)
            {
                Debug.LogError($"[GameSyncController.OverrideCloudData error] Failed to receive override response from cloud!");
                return;
            }

            // Step 3: Handle the backend response
            // + Step 3.1: If the override proccess is successful -> display success notification.
            // + Step 3.2: If the override proccess is failure -> display failed notification.
            if (response.Success)
            {
                playerData.LastSyncTime = DateTime.UtcNow;
                _playerDAO.Update(playerData);

                Debug.Log("<color=green>[GameSyncController.OverrideCloudData]</color> Push success");
            }
            else
            {
                Debug.LogError("[GameSyncController.OverrideCloudData error] Push failed");
            }
        }
    
        private bool ClearCurrentSessionData(LiteDatabase db, string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return false;
            }

            if (!_sessionDAO.DeleteById(db, sessionId))
            {
                return false;
            }

            if (!_characterRepo.DeleteManyBySessionId(db, sessionId))
            {
                return false;
            }

            if (!_exercisesDAO.DeleteAllBySessionId(db, sessionId))
            {
                return false;
            }

            if (!_inventoryDAO.DeleteManyBySessionId(db, sessionId))
            {
                return false;
            }

            if (!_shopDAO.DeleteManyBySessionId(db, sessionId))
            {
                return false;
            }

            if (!_cardsDAO.DeleteAllBySessionId(db, sessionId))
            {
                return false;
            }

            if (!_deckDAO.Delete(db, sessionId))
            {
                return false;
            }

            if (!_tournamentDAO.DeleteAllBySessionId(db, sessionId))
            {
                return false;
            }

            return true;
        }

        private bool InsertAll(LiteDatabase db, GetPlayerGameDataResponse response)
        {
            if (response.Session != null)
            {
                if (!_sessionDAO.Insert(db, response.Session))
                {
                    return false;
                }
            }

            if (response.Character != null)
            {
                if (!_characterRepo.Insert(db, response.Character))
                {
                    return false;
                }
            }

            if (response.Exercises != null && response.Exercises.Count > 0)
            {
                if (!_exercisesDAO.InsertMany(db, response.Exercises))
                {
                    return false;
                }
            }

            if (response.ObtainedItems != null && response.ObtainedItems.Count > 0)
            {
                if (!_inventoryDAO.InsertMany(db, response.ObtainedItems))
                {
                    return false;
                }
            }

            if (response.ShopItems != null && response.ShopItems.Count > 0)
            {
                if (!_shopDAO.InsertMany(db, response.ShopItems))
                {
                    return false;
                }
            }

            if (response.Cards != null && response.Cards.Count > 0)
            {
                if (!_cardsDAO.InsertMany(db, response.Cards))
                {
                    return false;
                }
            }

            if (response.Deck != null)
            {
                if (!_deckDAO.Insert(db, response.Deck))
                {
                    return false;
                }
            }

            if (response.TournamentProgressions != null && response.TournamentProgressions.Count > 0)
            {
                if (!_tournamentDAO.InsertMany(db, response.TournamentProgressions))
                {
                    return false;
                }
            }
            return true;
        }
    }
}