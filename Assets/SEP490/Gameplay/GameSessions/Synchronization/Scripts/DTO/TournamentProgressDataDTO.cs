namespace SEP490G69.GameSessions
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class TournamentProgressDataDTO
    {
        [JsonProperty("id")]
        public string Id { get; set; } // sessionId:tournamentId

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("rawTournamentId")]
        public string RawTournamentId { get; set; }

        [JsonProperty("currentRoundIndex")]
        public int CurrentRoundIndex { get; set; }

        [JsonProperty("participants")]
        public List<TournamentParticipantDTO> Participants { get; set; }

        [JsonProperty("semiFinalParticipants")]
        public List<TournamentParticipantDTO> SemiFinalParticipants { get; set; }

        [JsonProperty("finalParticipants")]
        public List<TournamentParticipantDTO> FinalParticipants { get; set; }

        [JsonProperty("currentRoundParticipants")]
        public List<TournamentParticipantDTO> CurrentRoundParticipants { get; set; }

        [JsonProperty("waitingForPlayerBattle")]
        public bool WaitingForPlayerBattle { get; set; }

        [JsonProperty("isBattleFinished")]
        public bool IsBattleFinished { get; set; }

        [JsonProperty("isPlayerWon")]
        public bool IsPlayerWon { get; set; }

        [JsonProperty("pendingEnemyId")]
        public string PendingEnemyId { get; set; }
    }
}
