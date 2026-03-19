namespace SEP490G69.GameSessions
{
    using Newtonsoft.Json;
    using System;

    public class UpdateLastSyncTimeRequest
    {
        [JsonProperty("player_id")]
        public string PlayerId { get; set; }
        [JsonProperty("last_sync_time")]
        public DateTime LastSyncTime { get; set; }
        [JsonProperty("sync_device")]
        public DeviceInfo SyncDevice { get; set; }
    }
}