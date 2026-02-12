namespace SEP490G69.Authentication.DTO
{
    using Newtonsoft.Json;

    public class LoginByExternalIdRequest 
    {
        [JsonProperty("player_id")]
        public string PlayerId { get; set; }

        [JsonProperty("player_name")]
        public string PlayerName { get; set; }

        [JsonProperty("device_id")]
        public string DeviceId { get; set; }

        [JsonProperty("device_type")]
        public string DeviceType { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}