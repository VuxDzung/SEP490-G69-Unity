using Newtonsoft.Json;

namespace SEP490G69.GameSessions
{

    public class SessionPlayerDeckDTO
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("cardIds")]
        public string[] CardIds { get; set; }
    }
}
