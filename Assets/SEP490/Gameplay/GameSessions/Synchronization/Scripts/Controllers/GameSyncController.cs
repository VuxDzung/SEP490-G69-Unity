namespace SEP490G69.GameSessions
{
    using SEP490G69.Addons.Networking;
    using System.Threading.Tasks;
    using System;
    using UnityEngine;

    public class GameSyncController : MonoBehaviour, IGameContext
    {
        private ContextManager _context;

        private SyncDecisionService _decisionService;
        private SyncApiService _apiService;
        private SyncLocalService _localService;
        private GameAuthManager _auth;

        private bool _isSyncing;

        public void SetManager(ContextManager manager)
        {
            _context = manager;

            _auth = _context.ResolveGameContext<GameAuthManager>();

            var web = _context.ResolveGameContext<WebRequests>();

            _apiService = new SyncApiService(web);
            _localService = new SyncLocalService();
            _decisionService = new SyncDecisionService(_apiService, _localService);
        }

        public async Task SyncPlayerData()
        {
            if (_isSyncing) return;

            _isSyncing = true;

            try
            {
                string playerId = _auth.GetUserId();

                string deviceId = _auth.GetDeviceId();

                if (playerId == deviceId)
                {
                    Debug.Log("<color=red>[GameSyncController.SyncPlayerData warning] You are using a Guest account. By our rule, guest account cannot sync data accross devices.</color>");
                    return;
                }

                string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);

                var state = await _decisionService.Decide(playerId, sessionId);

                switch (state)
                {
                    case ESyncState.PullingFromCloud:
                        await PullFromCloud(playerId, sessionId);
                        break;

                    case ESyncState.PushingToCloud:
                        await PushToCloud(playerId, sessionId);
                        break;

                    case ESyncState.Idle:
                        Debug.Log("Sync check completed. Nothing changed.");
                        break;

                    default:
                        Debug.LogWarning($"[Sync] Skip with state {state}");
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

        private async Task PullFromCloud(string playerId, string sessionId)
        {
            var cloudData = await _apiService.GetFullData(playerId, sessionId);
            if (cloudData == null) return;

            await _localService.OverrideLocal(playerId, sessionId, cloudData, _apiService);
        }

        private async Task PushToCloud(string playerId, string sessionId)
        {
            var localData = _localService.BuildLocalPayload(playerId, sessionId);
            
            if (localData == null)
            {
                return;
            }

            await _apiService.OverrideCloud(localData, _localService);
        }
    }
}