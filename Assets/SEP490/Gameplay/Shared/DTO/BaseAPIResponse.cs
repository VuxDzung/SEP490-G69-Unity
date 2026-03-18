namespace SEP490G69
{
    using Newtonsoft.Json;

    public class BaseAPIResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("error_msg")]
        public string ErrorMsg { get; set; }
    }
}