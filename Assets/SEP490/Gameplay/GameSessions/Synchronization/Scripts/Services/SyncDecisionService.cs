namespace SEP490G69.GameSessions
{
    using SEP490G69.Addons.Networking;
    using System;
    using System.Threading.Tasks;

    public class SyncDecisionService
    {
        private readonly SyncApiService _api;
        private readonly SyncLocalService _local;

        public SyncDecisionService(SyncApiService api, SyncLocalService local)
        {
            _api = api;
            _local = local;
        }

        /// <summary>
        /// Decide sync direction between local and cloud.
        ///
        /// FLOW:
        /// 1. Validate connection + input
        /// 2. Fetch cloud metadata (lightweight, no full data)
        /// 3. Compare local vs cloud progression
        /// 4. Return sync direction
        /// </summary>
        public async Task<ESyncState> Decide(string playerId, string sessionId)
        {
            // ---------------------------------------------------------
            // STEP 0: Check internet connection
            // ---------------------------------------------------------
            // If no internet -> cannot sync
            if (!WebRequests.HasInternetConnection())
                return ESyncState.CheckingConnection;

            // ---------------------------------------------------------
            // STEP 1: Validate input
            // ---------------------------------------------------------
            if (string.IsNullOrEmpty(playerId) || string.IsNullOrEmpty(sessionId))
                return ESyncState.Error;

            // ---------------------------------------------------------
            // STEP 2: Fetch cloud metadata
            // ---------------------------------------------------------
            // Metadata includes:
            // - CurrentRun (progress index)
            // - LastSyncTime (last time cloud was updated)
            // - MetadataResult (profile/session state)
            var cloud = await _api.GetMetadata(playerId, sessionId);

            // If API failed -> do nothing to avoid corrupting data
            if (cloud == null || !cloud.Success)
                return ESyncState.Error;

            // ---------------------------------------------------------
            // STEP 3: Handle special backend states
            // ---------------------------------------------------------

            // CASE: Profile exists but no session on cloud
            // -> First time sync or cloud lost session
            // -> Push local data to create cloud session
            if (cloud.MetadataResult == (int)EMetadataResult.HasProfileNoSession)
                return ESyncState.PushingToCloud;

            // ---------------------------------------------------------
            // STEP 4: Load local data
            // ---------------------------------------------------------
            var local = _local.GetPlayer(playerId);

            if (local == null)
                return ESyncState.Error;

            // ---------------------------------------------------------
            // STEP 5: Compare progression
            // ---------------------------------------------------------
            return Compare(local, cloud);
        }

        /// <summary>
        /// Core sync decision logic
        /// </summary>
        private ESyncState Compare(PlayerData local, PlayerMetadataResponse cloud)
        {
            int localRun = local.CurrentRun;
            int cloudRun = cloud.CurrentRun;

            DateTime localUpdated = local.LastUpdatedTime;
            DateTime cloudLastSync = cloud.LastSyncTime;

            // ---------------------------------------------------------
            // CASE 0: Cloud has never been synced before
            // ---------------------------------------------------------
            //
            // Meaning:
            // - CloudLastSyncTime == default
            // - Backend has no reliable timestamp
            //
            // Decision:
            // - If cloud already has progress → pull
            // - Otherwise → push local
            //
            if (cloudLastSync == default)
            {
                return cloudRun > 0
                    ? ESyncState.PullingFromCloud
                    : ESyncState.PushingToCloud;
            }

            // ---------------------------------------------------------
            // CASE 1: Cloud run > Local run
            // ---------------------------------------------------------
            //
            // Meaning:
            // - Player progressed further on another device
            //
            // Example:
            //   CloudRun = 10
            //   LocalRun = 8
            //
            // Decision:
            // → Local is outdated → Pull from cloud
            //
            if (cloudRun > localRun)
                return ESyncState.PullingFromCloud;

            // ---------------------------------------------------------
            // CASE 2: Local run > Cloud run
            // ---------------------------------------------------------
            //
            // Meaning:
            // - Player played offline on this device
            //
            // Example:
            //   CloudRun = 10
            //   LocalRun = 11
            //
            // Decision:
            // -> Cloud is outdated -> Push to cloud
            //
            if (cloudRun > localRun)
                return ESyncState.PushingToCloud;

            // ---------------------------------------------------------
            // CASE 3: Same run index
            // ---------------------------------------------------------
            //
            // Meaning:
            // - Both refer to the same run
            // - Need to compare "who updated later"
            //
            if (localRun == cloudRun)
            {
                // -----------------------------------------------------
                // CASE 3.1: Local updated after last cloud sync
                // -----------------------------------------------------
                //
                // Meaning:
                // - Local has newer changes not yet uploaded
                //
                // Example:
                //   LocalUpdated = 15:30
                //   CloudLastSync = 15:10
                //
                // Decision:
                // → Push local to cloud
                //
                if (localUpdated > cloudLastSync)
                {
                    return ESyncState.PushingToCloud;
                }

                // -----------------------------------------------------
                // CASE 3.2: Cloud is newer or equal
                // -----------------------------------------------------
                //
                // Meaning:
                // - Cloud already has latest data
                // - Local may be stale
                //
                // Decision:
                // → Pull from cloud
                //
                return ESyncState.PullingFromCloud;
            }

            // ---------------------------------------------------------
            // FALLBACK (should never happen)
            // ---------------------------------------------------------
            return ESyncState.Error;
        }
    }
}