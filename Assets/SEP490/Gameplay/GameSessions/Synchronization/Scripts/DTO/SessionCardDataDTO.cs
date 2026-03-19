using Newtonsoft.Json;

namespace SEP490G69.GameSessions
{
    public class SessionCardDataDTO
    {
        /// <summary>
        /// Session card id apply the following format
        /// <SESSION_ID>:<RAW_CARD_ID>
        /// </summary>

        [JsonProperty("sessionCardId")]
        public string SessionCardId { get; set; }

        [JsonProperty("rawCardId")]
        public string RawCardId { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("obtainedAmount")]
        public int ObtainedAmount { get; set; }
    }
}
