using Newtonsoft.Json;

namespace SEP490G69.GameSessions
{
    public class ItemDataDTO
    {
        #region Identifier
        [JsonProperty("sessionItemId")]
        public string SessionItemId { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("rawItemId")]
        public string RawItemId { get; set; }
        #endregion

        /// <summary>
        /// Stack item amount
        /// </summary>

        [JsonProperty("remainAmount")]
        public int RemainAmount { get; set; }
    }
}
