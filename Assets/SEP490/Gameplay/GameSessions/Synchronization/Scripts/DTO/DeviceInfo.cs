using Newtonsoft.Json;

namespace SEP490G69.GameSessions
{
    public class DeviceInfo
    {
        [JsonProperty("playerId")]
        public string PlayerId { get; set; } = string.Empty;
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; } = string.Empty;
        [JsonProperty("deviceType")]
        public string DeviceType { get; set; } = string.Empty;
    }
}