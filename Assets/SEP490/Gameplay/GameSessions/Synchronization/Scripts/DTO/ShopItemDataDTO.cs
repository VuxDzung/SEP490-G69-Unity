using Newtonsoft.Json;

namespace SEP490G69.GameSessions
{
    public class ShopItemDataDTO
    {
        #region Identifiers
        [JsonProperty("sessionItemId")]
        public string SessionItemId { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("rawItemId")]
        public string RawItemId { get; set; }
        #endregion

        [JsonProperty("remainAmount")]
        public int RemainAmount { get; set; }
    }
}
