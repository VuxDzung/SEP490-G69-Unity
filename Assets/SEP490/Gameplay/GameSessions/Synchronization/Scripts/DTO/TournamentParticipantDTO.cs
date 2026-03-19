namespace SEP490G69.GameSessions
{
    using Newtonsoft.Json;

    public class TournamentParticipantDTO 
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("characterId")]
        public string CharacterId { get; set; }

        [JsonProperty("isPlayer")]
        public bool IsPlayer { get; set; }

        [JsonProperty("slotIndex")]
        public int SlotIndex { get; set; }

        [JsonProperty("totalStats")]
        public float TotalStats { get; set; }

        [JsonProperty("isEliminated")]
        public bool IsEliminated { get; set; }
    }
}