namespace SEP490G69.GameSessions
{
    using Newtonsoft.Json;
    public class SessionTrainingExerciseDTO
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("exerciseId")]
        public string ExerciseId { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }
    }
}
