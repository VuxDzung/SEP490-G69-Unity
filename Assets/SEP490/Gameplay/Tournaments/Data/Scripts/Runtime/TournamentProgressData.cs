namespace SEP490G69.Tournament
{
    using LiteDB;
    using System.Collections.Generic;

    public class TournamentProgressData
    {
        [BsonId]
        public string Id { get; set; } // sessionId:tournamentId
        public string SessionId { get; set; }
        public string RawTournamentId { get; set; }

        public int CurrentRoundIndex { get; set; }

        public List<TournamentParticipantData> Participants { get; set; }
        public List<TournamentParticipantData> SemiFinalParticipants { get; set; }
        public List<TournamentParticipantData> FinalParticipants { get; set; }
        public List<TournamentParticipantData> CurrentRoundParticipants { get; set; }

        public bool WaitingForPlayerBattle { get; set; }

        public bool IsBattleFinished { get; set; }
        public bool IsPlayerWon { get; set; }

        public string PendingEnemyId { get; set; }
    }
}