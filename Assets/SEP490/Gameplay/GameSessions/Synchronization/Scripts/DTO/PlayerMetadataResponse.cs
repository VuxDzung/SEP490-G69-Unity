namespace SEP490G69.GameSessions
{
    using Newtonsoft.Json;
    using System;

    [System.Serializable]
    public class PlayerMetadataResponse 
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("metadata_result")]
        public int MetadataResult { get; set; }

        #region Success fields
        [JsonProperty("player_id")]
        public string PlayerId { get; set; } = string.Empty;

        [JsonProperty("player_name")]
        public string PlayerName { get; set; } = string.Empty;

        [JsonProperty("latest_session_id")]
        public string LatestSessionId { get; set; } = string.Empty;

        [JsonProperty("current_run")]
        public int CurrentRun { get; set; }

        [JsonProperty("last_sync_time")]
        public DateTime LastSyncTime { get; set; }
        #endregion

        #region Error fields
        [JsonProperty("error_msg")]
        public string ErrorMsg { get; set; } = string.Empty;
        #endregion
    }
}