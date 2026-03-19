namespace SEP490G69.GameSessions
{
    using Newtonsoft.Json;
    using System;
    public class PlayerInfoDTO
    {
        [JsonProperty("playerId")]
        public string PlayerId { get; set; } = string.Empty;

        [JsonProperty("playerName")]
        public string PlayerName { get; set; } = string.Empty;

        [JsonProperty("playerEmail")]
        public string PlayerEmail { get; set; } = string.Empty;

        [JsonProperty("legacyPoints")]
        public int LegacyPoints { get; set; }

        [JsonProperty("currentRun")]
        public int CurrentRun { get; set; }

        [JsonProperty("lastSyncedTime")]
        public DateTime LastSyncedTime { get; set; }

        [JsonProperty("lastSyncedDevice")]
        public DeviceInfo LastSyncedDevice { get; set; } = null;
    }
}