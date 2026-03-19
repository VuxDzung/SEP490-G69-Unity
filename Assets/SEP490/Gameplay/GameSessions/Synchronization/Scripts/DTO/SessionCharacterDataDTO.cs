namespace SEP490G69.GameSessions
{
    using Newtonsoft.Json;
    public class SessionCharacterDataDTO
    {
        //-----------------------------------------
        // INDENTIFIER
        //-----------------------------------------
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("rawCharacterId")]
        public string RawCharacterId { get; set; }

        //-----------------------------------------
        // CHARACTER-RUNTIME-STATS
        //-----------------------------------------
        [JsonProperty("currentMaxVitality")]
        public float CurrentMaxVitality { get; set; }

        [JsonProperty("currentPower")]
        public float CurrentPower { get; set; }

        [JsonProperty("currentIntelligence")]
        public float CurrentIntelligence { get; set; }

        [JsonProperty("currentStamina")]
        public float CurrentStamina { get; set; }

        [JsonProperty("currentDef")]
        public float CurrentDef { get; set; }

        [JsonProperty("currentAgi")]
        public float CurrentAgi { get; set; }

        [JsonProperty("currentEnergy")]
        public float CurrentEnergy { get; set; }

        [JsonProperty("currentMood")]
        public float CurrentMood { get; set; }

        [JsonProperty("currentRP")]
        public int CurrentRP { get; set; }

        public object Clone()
        {
            return new SessionCharacterDataDTO
            {
                Id = Id,
                SessionId = SessionId,           // Đã bổ sung
                RawCharacterId = RawCharacterId, // Đã bổ sung
                CurrentMaxVitality = CurrentMaxVitality,
                CurrentPower = CurrentPower,
                CurrentIntelligence = CurrentIntelligence,
                CurrentStamina = CurrentStamina,
                CurrentDef = CurrentDef,
                CurrentAgi = CurrentAgi,
                CurrentEnergy = CurrentEnergy,
                CurrentMood = CurrentMood,
                CurrentRP = CurrentRP,
            };
        }
    }
}
