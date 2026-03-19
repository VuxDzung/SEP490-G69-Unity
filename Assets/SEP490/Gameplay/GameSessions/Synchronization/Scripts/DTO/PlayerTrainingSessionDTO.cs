namespace SEP490G69.GameSessions
{
    using Newtonsoft.Json;
    public class PlayerTrainingSessionDTO
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("playerId")]
        public string PlayerId { get; set; }

        [JsonProperty("rawCharacterId")]
        public string RawCharacterId { get; set; }

        [JsonProperty("currentWeek")]
        public int CurrentWeek { get; set; } = 0;

        [JsonProperty("currentGoldAmount")]
        public int CurrentGoldAmount { get; set; } = 0;

        [JsonProperty("activeTournamentId")]
        public string ActiveTournamentId { get; set; } = string.Empty;
    }
}
